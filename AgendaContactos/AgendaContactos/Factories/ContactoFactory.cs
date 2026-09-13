using AgendaContactos.Model;

namespace AgendaContactos.Factories;

public class ContactoFactory {
    
    /// <summary>
    /// Semilla de datos iniciales
    /// </summary>
    /// <returns>Contactos Iniciales</returns>
    public static IEnumerable<Contacto> Seed() =>
        new List<Contacto> {
            new() { Id = 1, Nombre = "Nicolas", Telefono = "677888999", Alias = "Nick", Email = "Nicolas@gmail.com" },
            new() { Id = 2, Nombre = "Aitor", Telefono = "654789102", Alias = "Aitoraros", Email = "Aitor@gmail.com" },
            new() { Id = 3, Nombre = "Jose", Telefono = "612345678", Alias = "Joselu", Email = "Jose@gmail.com" },
            new() { Id = 4, Nombre = "Pepe", Telefono = "675849321", Alias = "Calvo", Email = "Pepe@gmail.com" },
            new() { Id = 5, Nombre = "Marcos", Telefono = "654389102", Alias = "Marquitos", Email = "Marcos@gmail.com" },
            new() { Id = 6, Nombre = "Antoine", Telefono = "689888754", Alias = "Antu", Email = "Antoine@gmail.com" },
            new() { Id = 7, Nombre = "Lucia", Telefono = "666662135", Alias = "Lu", Email = "Lucia@gmail.com" },
            new() { Id = 8, Nombre = "Laura", Telefono = "684930009", Alias = "Lauraa", Email = "Laura@gmail.com" },
            new() { Id = 9, Nombre = "Natalia", Telefono = "677444788", Alias = "Debi", Email = "Natalia@gmail.com" },
            new() { Id = 10, Nombre = "Diego", Telefono = "684012345", Alias = "Diegox", Email = "Diego@gmail.com" },
            new() { Id = 11, Nombre = "Jesus", Telefono = "675444908", Alias = "Yisus", Email = "Jesus@gmail.com" },
            new() { Id = 12, Nombre = "Rafa", Telefono = "689078921", Alias = "Farra", Email = "Rafa@gmail.com" },
            new() { Id = 13, Nombre = "Carlos", Telefono = "674543987", Alias = "Carlitos", Email = "Carlos@gmail.com" },
            new() { Id = 14, Nombre = "Pablo", Telefono = "677778856", Alias = "Pablito", Email = "Pablo@gmail.com" },
            new() { Id = 15, Nombre = "Kevin", Telefono = "671253467", Alias = "Kev", Email = "Kevin@gmail.com" },
        };
}