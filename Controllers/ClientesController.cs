using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Programacion2ClientesAPI.Data;
using Programacion2ClientesAPI.Models;

namespace Programacion2ClientesAPI.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes(CancellationToken cancellationToken)
    {
        return await context.Clientes.AsNoTracking()
            .OrderBy(cliente => cliente.Id_cliente).ToListAsync(cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetCliente(int id, CancellationToken cancellationToken)
    {
        var cliente = await context.Clientes.AsNoTracking()
            .FirstOrDefaultAsync(cliente => cliente.Id_cliente == id, cancellationToken);
        if (cliente is null)
            return NotFound(new { mensaje = "Cliente no encontrado." });

        return cliente;
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente, CancellationToken cancellationToken)
    {
        if (cliente.Id_cliente != 0)
            return BadRequest(new { mensaje = "El ID se genera automáticamente; omítalo o envíe 0." });

        context.Clientes.Add(cliente);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (EsCuiDuplicado(exception))
        {
            return Conflict(new { mensaje = "Ya existe un cliente con ese CUI." });
        }

        return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id_cliente }, cliente);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutCliente(int id, Cliente cliente, CancellationToken cancellationToken)
    {
        if (id != cliente.Id_cliente)
            return BadRequest(new { mensaje = "El ID de la ruta debe coincidir con Id_cliente." });

        var actual = await context.Clientes.FindAsync([id], cancellationToken);
        if (actual is null)
            return NotFound(new { mensaje = "Cliente no encontrado." });

        context.Entry(actual).CurrentValues.SetValues(cliente);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound(new { mensaje = "El cliente fue eliminado durante la actualización." });
        }
        catch (DbUpdateException exception) when (EsCuiDuplicado(exception))
        {
            return Conflict(new { mensaje = "Ya existe un cliente con ese CUI." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCliente(int id, CancellationToken cancellationToken)
    {
        var cliente = await context.Clientes.FindAsync([id], cancellationToken);
        if (cliente is null)
            return NotFound(new { mensaje = "Cliente no encontrado." });

        context.Clientes.Remove(cliente);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound(new { mensaje = "El cliente ya fue eliminado." });
        }

        return NoContent();
    }

    private static bool EsCuiDuplicado(DbUpdateException exception)
    {
        return exception.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 };
    }
}
