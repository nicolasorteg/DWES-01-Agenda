using AgendaContactos.Config;
using AgendaContactos.Entity;
using AgendaContactos.Factories;
using AgendaContactos.Model;
using AgendaContactos.Repository.Common;
using Serilog;

namespace AgendaContactos.Repository.EfCore;

public class ContactoEfCoreRepository : IContactoRepository {
    
    private readonly AppDbContext _context;
    private readonly ILogger _logger = Log.ForContext<ContactoEfCoreRepository>();

    // constructor
    public ContactoEfCoreRepository(AppDbContext context) {
        _context = context;

        if (AppConfig.DropData) {
            _logger.Warning("DropData activo. Eliminando base de datos...");
            _context.Database.EnsureDeleted();
        }
        _context.Database.EnsureCreated();

        // comprueba si es necesario el sembrado inicial
        if (!AppConfig.SeedData || _context.Contactos.Any()) return;
        _logger.Debug("Sembrando datos iniciales de contactos en EFCore...");
        foreach (var contacto in ContactoFactory.Seed()) Create(contacto);
    }
    
    public Contacto? GetById(int id) {
        throw new NotImplementedException();
    }

    public IEnumerable<Contacto> GetAll(int pagina = 1, int tamPagina = 10) {
        throw new NotImplementedException();
    }

    public bool Create(Contacto entity) {
        throw new NotImplementedException();
    }

    public bool Update(int id, Contacto entity) {
        throw new NotImplementedException();
    }

    public bool Delete(int id) {
        throw new NotImplementedException();
    }

    public bool DeleteAll() {
        throw new NotImplementedException();
    }

    public int Count() {
        throw new NotImplementedException();
    }

    public Contacto? GetByAlias(string alias) {
        throw new NotImplementedException();
    }

    public Contacto? GetByTelefono(string telefono) {
        throw new NotImplementedException();
    }
}