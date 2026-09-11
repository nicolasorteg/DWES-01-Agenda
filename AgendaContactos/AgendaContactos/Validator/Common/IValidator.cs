using AgendaContactos.Errors.Common;
using CSharpFunctionalExtensions;

namespace AgendaContactos.Validator.Common;

/// <summary>
/// Contrato para la validación de entidades
/// </summary>
/// <typeparam name="T">Entidad a validar</typeparam>
public interface IValidator<T> {
    
    /// <summary>
    /// Valida una entidad
    /// </summary>
    /// <param name="entity">Entidad a validar</param>
    /// <returns>La entidad si se valida correctamente o error de dominio si no</returns>
    Result<T, DomainError> Validar(T entity);
}