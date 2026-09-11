namespace AgendaContactos.Model;

/// <summary>
/// Modelo de Contacto inmutable
/// </summary>
public record Contacto {
    
    public int Id { get; init; }
    
    public string Nombre { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty; // mejor que int para internacionalizar
    public string Alias { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}