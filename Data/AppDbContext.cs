using Microsoft.EntityFrameworkCore;
using Programacion2ClientesAPI.Models;

namespace Programacion2ClientesAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().HasIndex(cliente => cliente.CUI).IsUnique();
    }
}
