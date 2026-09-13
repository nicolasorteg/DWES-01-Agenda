using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaContactos.Entity;

/// <summary>
/// Representación de Contacto para la BD
/// </summary>
[Table("Contactos")]
public class ContactoEntity {
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // autoincrement
    public int Id { get; set; }

    [Required, MaxLength(14)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Telefono { get; set; } = string.Empty;

    [Required, MaxLength(19)]
    public string Alias { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;
}