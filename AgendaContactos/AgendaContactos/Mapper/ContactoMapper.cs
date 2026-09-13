using AgendaContactos.Entity;
using AgendaContactos.Model;

namespace AgendaContactos.Mapper;

public static class ContactoMapper {

    /// <summary> Conversión Model -> Entity </summary>
    public static ContactoEntity ToEntity(this Contacto contacto) => new ContactoEntity() {
        Id = contacto.Id,
        Nombre = contacto.Nombre,
        Telefono = contacto.Telefono,
        Alias = contacto.Alias,
        Email = contacto.Email
    };
    
    /// <summary> Conversión Entity -> Model </summary>
    public static Contacto ToModel(this ContactoEntity entity) => new() {
        Id = entity.Id,
        Nombre = entity.Nombre,
        Telefono = entity.Telefono,
        Alias = entity.Alias,
        Email = entity.Email
    };
    
    /// <summary> Conversión Listado de Entity -> Listado de Model </summary>
    public static IEnumerable<Contacto> ToModel(this IEnumerable<ContactoEntity> entities) {
        return entities
            .Select(c => c.ToModel());
    }
}