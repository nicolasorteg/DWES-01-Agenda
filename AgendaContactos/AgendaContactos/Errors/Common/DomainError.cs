namespace AgendaContactos.Errors.Common;

/// <summary>
/// Contenedor para los errores del sistema
/// </summary>
/// <param name="Message">Mensaje de error</param>
public abstract record DomainError(string Message);