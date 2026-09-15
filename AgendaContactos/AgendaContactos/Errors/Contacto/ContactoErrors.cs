using AgendaContactos.Errors.Common;

namespace AgendaContactos.Errors.Contacto;

/// <summary>
/// Factory para crear errores de Contactos
/// </summary>
public static class ContactoErrors {
    
    /// <inheritdoc cref="ContactoError.NotFoundById"/>
    public static DomainError NotFoundById(int id) =>
        new ContactoError.NotFoundById(id);
    
    /// <inheritdoc cref="ContactoError.NotFoundByAlias"/>
    public static DomainError NotFoundByAlias(string alias) =>
        new ContactoError.NotFoundByAlias(alias);
    
    public static DomainError NotFoundByTelefono(string telefono) =>
        new ContactoError.NotFoundByTelefono(telefono);

    /// <inheritdoc cref="ContactoError.Validation"/>
    public static DomainError Validation(IEnumerable<string> errors) =>
        new ContactoError.Validation(errors);
    
    /// <inheritdoc cref="ContactoError.TelefonosRepetidos"/>
    public static DomainError TelefonosRepetidos(string telefono) =>
        new ContactoError.TelefonosRepetidos(telefono);
    
    /// <inheritdoc cref="ContactoError.AliasRepetidos"/>
    public static DomainError AliasRepetidos(string alias) =>
        new ContactoError.AliasRepetidos(alias);
}