using Serilog;

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
            nombre.Trim().Length > 2;
    }
}
