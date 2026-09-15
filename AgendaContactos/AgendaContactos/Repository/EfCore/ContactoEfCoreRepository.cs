using AgendaContactos.Config;
using AgendaContactos.Entity;
using AgendaContactos.Factories;
using AgendaContactos.Mapper;
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
    
    /// <inheritdoc cref="IContactoRepository.GetById" />
    public Contacto? GetById(int id) {
        try {
            _logger.Debug($"Obteniendo Contacto por ID: {id}");
            return _context.Contactos.FirstOrDefault(c => c.Id == id)?.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, $"Error al obtener Contacto por ID {id}.");
            return null;
        }
    }

    /// <inheritdoc cref="IContactoRepository.GetAll" />
    public IEnumerable<Contacto> GetAll(int pagina = 1, int tamPagina = 10) {
        try {
            _logger.Debug($"Obteniendo todos de forma paginada...");
            
            // ordenacion y paginacion
            var entidades = _context.Contactos
                .OrderBy(c => c.Id) // menor a mayor ID
                .Skip((pagina - 1) * tamPagina) // se salta los de las páginas anteriores
                .Take(tamPagina)
                .ToList();
            
            return entidades.Select(e => e.ToModel());
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error obteniendo todos los Contactos.");
            return [];
        }
    }

    /// <inheritdoc cref="IContactoRepository.Create" />
    public bool Create(Contacto entity) {
        try {
            _logger.Debug($"Insertando nuevo Contacto...");

            var dbEntity = entity.ToEntity();
            _context.Contactos.Add(dbEntity);
            _context.SaveChanges(); // genera ID autoincremental
            
            return true;
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al crear Contacto en EF Core.");
            return false;
        }
    }

    /// <inheritdoc cref="IContactoRepository.Update" />
    public bool Update(int id, Contacto entity) {
        try {
            _logger.Debug($"Actualizando Contacto por EFCore con ID: {id}");

            var dbEntity = _context.Contactos.FirstOrDefault(c => c.Id == id);
            if (dbEntity == null) return false;
            
            dbEntity.Nombre = entity.Nombre;
            dbEntity.Telefono = entity.Telefono;
            dbEntity.Alias = entity.Alias;
            dbEntity.Email = entity.Email;
            _context.SaveChanges();
            
            return true;
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al actualizar el Contacto.");
            return false;
        }
    }

    /// <inheritdoc cref="IContactoRepository.Delete" />
    public bool Delete(int id) {
        try {
            _logger.Debug($"Eliminando Contacto por EFCore con ID: {id}");

            var dbEntity = _context.Contactos.FirstOrDefault(c => c.Id == id);
            if (dbEntity == null) return false;

            _context.Contactos.Remove(dbEntity);
            _context.SaveChanges();
            
            return true;
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al eliminar el Contacto.");
            return false;
        }
    }

    /// <inheritdoc cref="IContactoRepository.DeleteAll" />
    public bool DeleteAll() {
        try {
            _context.Contactos.RemoveRange(_context.Contactos);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al vaciar la tabla Contactos.");
            return false;
        }
    }

    /// <inheritdoc cref="IContactoRepository.Count" />
    public int Count() {
        try {
            _logger.Debug("Contando nº de Contactos...");
            return _context.Contactos.Count();
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al contar Contactos.");
            return 0;
        }
    }

    /// <inheritdoc cref="IContactoRepository.GetByAlias" />
    public Contacto? GetByAlias(string alias) {
        try {
            _logger.Debug($"Obteniendo Contacto por Alias: {alias}");
            return _context.Contactos.FirstOrDefault(c => c.Alias == alias)?.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, $"Error al obtener Contacto por Alias {alias}.");
            return null;
        }
    }

    /// <inheritdoc cref="IContactoRepository.GetByTelefono" />
    public Contacto? GetByTelefono(string telefono) {
        try {
            _logger.Debug($"Obteniendo Contacto por Teléfono: {telefono}");
            return _context.Contactos.FirstOrDefault(c => c.Telefono == telefono)?.ToModel();
        }
        catch (Exception ex) {
            _logger.Error(ex, $"Error al obtener Contacto por Teléfono {telefono}.");
            return null;
        }
    }
}