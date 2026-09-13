using AgendaContactos.Model;

namespace AgendaContactos.Repository.Common;

public interface IContactoRepository : ICrudRepository<Contacto, int> {
    
    /// <summary>
    /// Obtiene una entidad por su Alias
    /// </summary>
    Contacto? GetByAlias(string alias);
}