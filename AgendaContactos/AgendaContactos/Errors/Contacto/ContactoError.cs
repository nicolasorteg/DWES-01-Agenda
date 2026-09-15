using AgendaContactos.Errors.Common;

namespace AgendaContactos.Errors.Contacto;

/// <summary>
/// Contenedor para los errores específicos para Contactos
/// </summary>
/// <param name="Message">Mensaje de error</param>
public abstract record ContactoError(string Message) : DomainError(Message) {
    
    /// <summary>
    /// Error de Contacto no encontrado según ID (404)
    /// </summary>
    /// <param name="Id">Identificador único</param>
    public sealed record NotFoundById(int Id) 
        : ContactoError($"No se ha encontrado ningún Contacto con el identificador: {Id}");
    
    /// <summary>
    /// Error de Contacto no encontrado según Alidas (404)
    /// </summary>
    /// <param name="Alias">Alias del Contacto</param>
    public sealed record NotFoundByAlias(string Alias) 
        : ContactoError($"No se ha encontrado ningún Contacto con el alias: {Alias}");
    
    public sealed record NotFoundByTelefono(string Telefono) 
        : ContactoError($"No se ha encontrado ningún Contacto con el teléfono: {Telefono}");
    
    /// <summary>
    /// Error de validación en el Contacto (400)
    /// </summary>
    /// <param name="Errores">Listado de mensajes de errores de validación</param>
    public sealed record Validation(IEnumerable<string> Errores) 
        : ContactoError($"Se han detectado errores de validación en la entidad:{Environment.NewLine} {string.Join($"{Environment.NewLine}• ", Errores)}");
    
    /// <summary>
    /// Error de Telefono repetido (409)
    /// </summary>
    /// <param name="Telefono">Teléfono del contacto</param>
    public sealed record TelefonosRepetidos(string Telefono) 
        : ContactoError($"El Contacto con número {Telefono} ya existe en la agenda.");
    
    /// <summary>
    /// Error de Alias repetido (409)
    /// </summary>
    /// <param name="Alias">Alias del contacto</param>
    public sealed record AliasRepetidos(string Alias) 
        : ContactoError($"El Contacto con alias '{Alias}' ya existe en la agenda.");
}