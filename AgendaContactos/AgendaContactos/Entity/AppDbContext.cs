using AgendaContactos.Model;
using Microsoft.EntityFrameworkCore;

namespace AgendaContactos.Entity;

/// <summary> Puente que conecta C# con SQLite para efcore </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<ContactoEntity> Contactos => Set<ContactoEntity>();
}
