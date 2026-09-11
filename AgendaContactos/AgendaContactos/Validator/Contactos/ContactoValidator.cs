using AgendaContactos.Errors.Common;
using AgendaContactos.Errors.Contacto;
using AgendaContactos.Model;
using AgendaContactos.Validator.Common;
using CSharpFunctionalExtensions;
using Serilog;

namespace AgendaContactos.Validator.Contactos;

/// <summary>
/// Validador para campos de Contacto. Se usa el Result de la ROP
/// </summary>
public class ContactoValidator: IValidator<Contacto> {
    
    /// <inheritdoc cref="IValidator{T}.Validar" />
    public Result<Contacto, DomainError> Validar(Contacto contacto) {
        Log.Debug($"🔵 Validando el Contacto de ID: {contacto.Id}");
        
        // listado de mensajes de error
        var errores = new List<string>();
        
        // validacion campo por campo con funciones de extensión
        
        
        
        
        
        
        // si errores contiene algo se devuelve failure, si está vacío se devuelve success
        return errores.Any() ? 
            Result.Failure<Contacto, DomainError>(ContactoErrors.Validation(errores)) : 
            Result.Success<Contacto, DomainError>(contacto);
    }
}
