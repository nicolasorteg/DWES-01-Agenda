using System.Text.RegularExpressions;

namespace AgendaContactos.Validator.Contactos;

/// <summary>
/// Contenedor de funciones de extension para validar los datos de Contactos
/// </summary>
public static class ContactoValidatorExtensions {

    /// <summary>
    /// Validador del nombre del Contacto
    /// </summary>
    /// <param name="nombre">Nombre del Contacto</param>
    extension(string nombre) {
        public bool IsNombreValid() =>
            !string.IsNullOrWhiteSpace(nombre) &&
            nombre.Trim().Length < 15 &&
            nombre.Trim().Length > 1;
    }
    
    /// <summary>
    /// Validador del alias del Contacto
    /// </summary>
    /// <param name="alias">Alias del Contacto</param>
    extension(string alias) {
        public bool IsAliasValid() =>
            !string.IsNullOrWhiteSpace(alias) &&
            alias.Trim().Length < 20 &&
            alias.Trim().Length > 0;
    }
    
    /// <summary>
    /// Validador del teléfono del Contacto
    /// </summary>
    /// <param name="telefono">Telefono del Contacto</param>
    extension(string telefono) {
        public bool IsTelefonoValid() {
            const string TelefonoRegex = @"^\+?\d{9,15}$";
            return Regex.IsMatch(telefono.Trim(), TelefonoRegex);
        }
    }
    
    /// <summary>
    /// Validador del email del Contacto
    /// </summary>
    /// <param name="email">Email del Contacto</param>
    extension(string email) {
        public bool IsEmailValid() {
            const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email.Trim(), EmailRegex);
        }
    }
}