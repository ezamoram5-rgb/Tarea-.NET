# Verificación de Blazor

Fecha: 27 de septiembre de 2026. Entorno: Windows, SDK .NET 10.0.400.

## Comprobaciones realizadas

- Compilación Release de la solución: sin errores ni advertencias.
- Script de regresión de la API: 23 comprobaciones aprobadas con base temporal.
- Navegador: carga inicial de la tabla vacía.
- Formulario vacío: muestra mensajes de campos obligatorios, incluida la fecha.
- Alta desde el modal: la API devuelve 201; aparece el cliente sin recargar la página.
- Edición desde el modal: se conserva el ID, cambia la dirección y se actualiza la tabla.
- Eliminación con confirmación: desaparece el cliente y el contador vuelve a cero.
- Las pruebas del navegador utilizaron datos ficticios y una base aislada fuera del repositorio.

## Pruebas manuales adicionales sugeridas

1. Registrar dos clientes con el mismo CUI: el segundo debe mostrar el error de la API dentro del modal.
2. Probar CUI de longitud incorrecta, teléfono inválido y fecha futura: no debe guardarse.
3. Editar y cancelar: los datos de la tabla deben permanecer iguales.
4. Detener la API y pulsar Actualizar: debe aparecer un error y permitir reintentar al iniciar la API.
5. Reducir el ancho de la ventana: el formulario se organiza en una columna y la tabla permite desplazamiento horizontal.

## Repetir la verificación de la API

```powershell
pwsh -File scripts/Verificar-API.ps1
```

El script ahora indica explícitamente la solución a compilar para evitar ambigüedad al convivir con el proyecto Blazor.
