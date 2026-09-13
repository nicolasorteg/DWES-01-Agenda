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
        if (!contacto.Nombre.IsNombreValid()) 
            errores.Add("El Nombre es obligatorio y debe tener entre 2-14 caracteres.");
        
        if (!contacto.Alias.IsAliasValid())
            errores.Add("El Alias es obligatorio y debe tener entre 1-19 caracteres.");
        
        if (!contacto.Telefono.IsTelefonoValid())
            errores.Add("El Teléfono es obligatorio y solo permite prefijo y números sin - ni caracteres especiales.");
        
        if(!contacto.Email.IsEmailValid())
            errores.Add("El Email es obligatorio y tiene que seguir el formato 'xxx@xxx.xxx'");
        
        // si errores contiene algo se devuelve failure, si está vacío se devuelve success
        return errores.Any() ? 
            Result.Failure<Contacto, DomainError>(ContactoErrors.Validation(errores)) : 
            Result.Success<Contacto, DomainError>(contacto);
    }
}