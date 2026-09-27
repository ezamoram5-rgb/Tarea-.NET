param([int]$Puerto = 5187)

$ErrorActionPreference = 'Stop'
$raiz = Split-Path $PSScriptRoot -Parent
$baseUrl = "http://127.0.0.1:$Puerto"
$temporal = Join-Path ([IO.Path]::GetTempPath()) ('ClientesAPI-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $temporal | Out-Null
$db = Join-Path $temporal 'pruebas.db'
$dll = Join-Path $raiz 'bin/Release/net10.0/Programacion2ClientesAPI.dll'
$script:servidor = $null
$script:comprobaciones = 0
$conexionAnterior = $env:ConnectionStrings__DefaultConnection
$entornoAnterior = $env:ASPNETCORE_ENVIRONMENT

function Comprobar([bool]$condicion, [string]$nombre) {
    if (-not $condicion) { throw "FALLO: $nombre" }
    $script:comprobaciones++
    Write-Output "OK: $nombre"
}

function Solicitar([string]$metodo, [string]$ruta, $cuerpo = $null) {
    $parametros = @{
        Uri = "$baseUrl$ruta"
        Method = $metodo
        SkipHttpErrorCheck = $true
        TimeoutSec = 15
    }
    if ($null -ne $cuerpo) {
        $parametros.ContentType = 'application/json; charset=utf-8'
        $parametros.Body = $cuerpo | ConvertTo-Json -Depth 8
    }
    return Invoke-WebRequest @parametros
}

function Iniciar {
    $script:servidor = Start-Process -FilePath 'dotnet' -ArgumentList @("`"$dll`"", '--urls', $baseUrl) -WorkingDirectory $raiz -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $temporal 'salida.log') -RedirectStandardError (Join-Path $temporal 'error.log')
    for ($intento = 0; $intento -lt 60; $intento++) {
        if ($script:servidor.HasExited) {
            throw (Get-Content (Join-Path $temporal 'error.log') -Raw)
        }
        try {
            $respuesta = Solicitar 'GET' '/api/clientes'
            if ($respuesta.StatusCode -eq 200) { return }
        } catch { }
        Start-Sleep -Milliseconds 500
    }
    throw 'La API no inició a tiempo.'
}

function Detener {
    if ($script:servidor -and -not $script:servidor.HasExited) {
        Stop-Process -Id $script:servidor.Id
        $script:servidor.WaitForExit()
    }
}

try {
    dotnet build $raiz -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw 'La compilación falló.' }
    $env:ConnectionStrings__DefaultConnection = "Data Source=$db"
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    Iniciar
    $lista = Solicitar 'GET' '/api/clientes'
    Comprobar ($lista.StatusCode -eq 200 -and $lista.Content.Trim() -eq '[]') 'GET devuelve una lista vacía al comenzar'
    $datos = @{
        CUI = '1000000000001'
        NIT = '1234567-8'
        Nombres = 'Ana'
        Apellidos = 'Prueba'
        Direccion = 'Dirección ficticia de prueba'
        Telefono = '55555555'
        Fecha_Nacimiento = '2000-01-15'
    }
    $creado = Solicitar 'POST' '/api/clientes' $datos
    Comprobar ($creado.StatusCode -eq 201) 'POST devuelve 201'
    $cliente = $creado.Content | ConvertFrom-Json
    $id = $cliente.Id_cliente
    Comprobar ($id -gt 0 -and $creado.Headers.Location[0].EndsWith("/api/clientes/$id")) 'POST genera el ID y la cabecera Location'
    $consulta = Solicitar 'GET' "/api/clientes/$id"
    Comprobar ($consulta.StatusCode -eq 200 -and ($consulta.Content | ConvertFrom-Json).Direccion -eq $datos.Direccion) 'GET por ID conserva los datos y las tildes'
    $lista = Solicitar 'GET' '/api/clientes'
    Comprobar (@($lista.Content | ConvertFrom-Json).Count -eq 1) 'GET lista el cliente registrado'
    Comprobar ((Solicitar 'POST' '/api/clientes' $datos).StatusCode -eq 409) 'CUI duplicado devuelve 409'
    $datos.Id_cliente = $id
    $datos.Nombres = 'Ana María'
    Comprobar ((Solicitar 'PUT' "/api/clientes/$id" $datos).StatusCode -eq 204) 'PUT devuelve 204'
    Comprobar (((Solicitar 'GET' "/api/clientes/$id").Content | ConvertFrom-Json).Nombres -eq 'Ana María') 'PUT guarda los cambios'
    Detener
    Iniciar
    Comprobar (((Solicitar 'GET' "/api/clientes/$id").Content | ConvertFrom-Json).Nombres -eq 'Ana María') 'Los datos persisten después de reiniciar'
    Comprobar ((Solicitar 'PUT' '/api/clientes/999999' $datos).StatusCode -eq 400) 'PUT rechaza un ID diferente al de la ruta'
    $datos.Id_cliente = 999999
    Comprobar ((Solicitar 'PUT' '/api/clientes/999999' $datos).StatusCode -eq 404) 'PUT de cliente inexistente devuelve 404'
    Comprobar ((Solicitar 'POST' '/api/clientes' $datos).StatusCode -eq 400) 'POST rechaza un ID asignado manualmente'
    $datos.Remove('Id_cliente')
    $datos.CUI = '1000000000002'
    $segundo = Solicitar 'POST' '/api/clientes' $datos
    $segundoId = ($segundo.Content | ConvertFrom-Json).Id_cliente
    $datos.Id_cliente = $segundoId
    $datos.CUI = '1000000000001'
    Comprobar ((Solicitar 'PUT' "/api/clientes/$segundoId" $datos).StatusCode -eq 409) 'PUT rechaza el CUI de otro cliente'
    Comprobar ((Solicitar 'DELETE' "/api/clientes/$segundoId").StatusCode -eq 204) 'DELETE elimina el segundo cliente'
    $datos.Remove('Id_cliente')
    $datos.CUI = '123'
    Comprobar ((Solicitar 'POST' '/api/clientes' $datos).StatusCode -eq 400) 'CUI inválido devuelve 400'
    $datos.CUI = '1000000000003'
    $datos.Fecha_Nacimiento = (Get-Date).AddYears(1).ToString('yyyy-MM-dd')
    Comprobar ((Solicitar 'POST' '/api/clientes' $datos).StatusCode -eq 400) 'Fecha futura devuelve 400'
    $datos.Remove('Fecha_Nacimiento')
    Comprobar ((Solicitar 'POST' '/api/clientes' $datos).StatusCode -eq 400) 'Fecha omitida devuelve 400'
    Comprobar ((Solicitar 'POST' '/api/clientes' @{}).StatusCode -eq 400) 'Campos obligatorios ausentes devuelven 400'
    $swagger = Solicitar 'GET' '/swagger/v1/swagger.json'
    Comprobar ($swagger.StatusCode -eq 200 -and $swagger.Content.Contains('/api/clientes')) 'La documentación Swagger está disponible'
    Comprobar ((Solicitar 'DELETE' "/api/clientes/$id").StatusCode -eq 204) 'DELETE devuelve 204'
    Comprobar ((Solicitar 'GET' "/api/clientes/$id").StatusCode -eq 404) 'GET de cliente eliminado devuelve 404'
    Comprobar ((Solicitar 'DELETE' "/api/clientes/$id").StatusCode -eq 404) 'DELETE de cliente inexistente devuelve 404'
    Comprobar ((Solicitar 'GET' '/api/clientes').Content.Trim() -eq '[]') 'La lista queda vacía tras eliminar los clientes'
    Write-Output "Resultado: $script:comprobaciones comprobaciones correctas."
}
finally {
    Detener
    $env:ConnectionStrings__DefaultConnection = $conexionAnterior
    $env:ASPNETCORE_ENVIRONMENT = $entornoAnterior
    Write-Output "Registros de esta prueba: $temporal"
}
