using AgendaContactos.Errors.Common;
using AgendaContactos.Model;
using CSharpFunctionalExtensions;

namespace AgendaContactos.Service;

public interface IContactoService {
    
    /// <summary> Busca por ID un contacto </summary>
    Result<Contacto, DomainError> BuscarContactoPorId(int id);
    
    /// <summary> Busca por Alias un contacto </summary>
    Result<Contacto, DomainError> BuscarContactoPorAlias(string alias);
    
    /// <summary> Busca por Teléfono un contacto </summary>
    Result<Contacto, DomainError> BuscarContactoPorTelefono(string telefono);
    
    /// <summary> Obtiene los contactos de forma paginada </summary>
    IEnumerable<Contacto> ObtenerContactosPaginados(int pagina = 1, int tamPagina = 5);
    
    /// <summary> Guarda un contacto pasado </summary>
    Result<Contacto, DomainError> CrearContacto(Contacto contacto);
    
    /// <summary> Actualiza un contacto pasado </summary>
    Result<Contacto, DomainError> ActualizarContacto(int id, Contacto contacto);
    
    /// <summary> Elimina un contacto pasado </summary>
    Result<Contacto, DomainError> EliminarContacto(int id);
    
    /// <summary> Elimina todos los contactos </summary>
    bool EliminarTodosLosContactos();
}