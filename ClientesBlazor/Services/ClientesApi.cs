using System.Net.Http.Json;
using System.Text.Json;
using ClientesBlazor.Models;

namespace ClientesBlazor.Services;

public sealed class ClientesApi(HttpClient http)
{
    public async Task<List<Cliente>> ListarAsync() =>
        await http.GetFromJsonAsync<List<Cliente>>("api/clientes")
        ?? throw new HttpRequestException("La API devolvió una respuesta vacía.");

    public async Task GuardarAsync(Cliente cliente)
    {
        using var response = cliente.Id_cliente == 0
            ? await http.PostAsJsonAsync("api/clientes", cliente)
            : await http.PutAsJsonAsync($"api/clientes/{cliente.Id_cliente}", cliente);
        await ComprobarAsync(response);
    }

    public async Task EliminarAsync(int id)
    {
        using var response = await http.DeleteAsync($"api/clientes/{id}");
        await ComprobarAsync(response);
    }

    private static async Task ComprobarAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var message = $"La API rechazó la operación ({(int)response.StatusCode}).";
        try
        {
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = json.RootElement;
            if (root.TryGetProperty("mensaje", out var text))
                message = text.GetString() ?? message;
            else if (root.TryGetProperty("errors", out var errors))
                message = string.Join(" ", errors.EnumerateObject()
                    .SelectMany(p => p.Value.EnumerateArray()).Select(v => v.GetString()));
        }
        catch (JsonException) { }
        throw new ApiException(message);
    }
}

public sealed class ApiException(string message) : Exception(message);
