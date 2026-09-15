namespace AgendaContactos.Repository.Common;

/// <summary>
/// Contrato genérico para operaciones CRUD
/// </summary>
/// <typeparam name="TEntity">Tipo del modelo de dominio</typeparam>
/// <typeparam name="TKey">Tipo del ID</typeparam>
public interface ICrudRepository<TEntity, in TKey> where TEntity : class {
    
    /// <summary>
    /// Obtiene una entidad por su ID
    /// </summary>
    TEntity? GetById(TKey id);

    /// <summary>
    /// Obtiene todos los contactos de forma paginada
    /// </summary>
    IEnumerable<TEntity> GetAll(int pagina = 1, int tamPagina = 10);

    /// <summary>
    /// Crea una nueva entidad en el sistema
    /// </summary>
    bool Create(TEntity entity);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    bool Update(TKey id, TEntity entity);

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    bool Delete(TKey id);

    /// <summary>
    /// Elimina todos los registros del sistema
    /// </summary>
    bool DeleteAll();
    
    /// <summary>
    /// Cuenta el total de entidades
    /// </summary>
    int Count();
}