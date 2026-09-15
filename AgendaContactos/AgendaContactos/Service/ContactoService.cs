using AgendaContactos.Errors.Common;
using AgendaContactos.Errors.Contacto;
using AgendaContactos.Model;
using AgendaContactos.Repository.Common;
using AgendaContactos.Validator.Common;
using AgendaContactos.Cache.Common;
using CSharpFunctionalExtensions;
using Serilog;

namespace AgendaContactos.Service;

public class ContactoService(IContactoRepository repository, IValidator<Contacto> validator, ICache<int, Contacto> cache) : IContactoService {
    
    /// <inheritdoc cref="IContactoService.BuscarContactoPorId" />
    public Result<Contacto, DomainError> BuscarContactoPorId(int id) {
        Log.Debug($"Buscando Contacto por ID {id}");

        var isInCache = cache.Get(id);
        if (isInCache is not null)
            return Result.Success<Contacto, DomainError>(isInCache);

        var isInRepo = repository.GetById(id);
        if (isInRepo is null)
            return Result.Failure<Contacto, DomainError>(ContactoErrors.NotFoundById(id));

        cache.Add(id, isInRepo);
        return Result.Success<Contacto, DomainError>(isInRepo);
    }

    /// <inheritdoc cref="IContactoService.BuscarContactoPorAlias" />
    public Result<Contacto, DomainError> BuscarContactoPorAlias(string alias) {
        Log.Debug($"Buscando Contacto por Alias {alias}");

        var contacto = repository.GetByAlias(alias);
        return contacto != null
            ? Result.Success<Contacto, DomainError>(contacto)
            : Result.Failure<Contacto, DomainError>(ContactoErrors.NotFoundByAlias(alias));
    }

    /// <inheritdoc cref="IContactoService.BuscarContactoPorTelefono" />
    public Result<Contacto, DomainError> BuscarContactoPorTelefono(string telefono) {
        Log.Debug($"Buscando Contacto por Teléfono {telefono}");

        var contacto = repository.GetByTelefono(telefono);
        return contacto != null
            ? Result.Success<Contacto, DomainError>(contacto)
            : Result.Failure<Contacto, DomainError>(ContactoErrors.NotFoundByTelefono(telefono));
    }

    /// <inheritdoc cref="IContactoService.ObtenerContactosPaginados" />
    public IEnumerable<Contacto> ObtenerContactosPaginados(int pagina = 1, int tamPagina = 5) =>
        repository.GetAll(pagina, tamPagina);

    /// <inheritdoc cref="IContactoService.CrearContacto" />
    public Result<Contacto, DomainError> CrearContacto(Contacto contacto) {
        Log.Debug("Creación de Contacto...");

        return validator.Validar(contacto)
            .Ensure(c => repository.GetByTelefono(c.Telefono) is null,
                ContactoErrors.TelefonosRepetidos(contacto.Telefono))
            .Ensure(c => repository.GetByAlias(c.Alias) is null,
                ContactoErrors.AliasRepetidos(contacto.Alias))
            .Bind(c => {
                var creado = repository.Create(c);
                return creado
                    ? Result.Success<Contacto, DomainError>(c)
                    : Result.Failure<Contacto, DomainError>(
                        ContactoErrors.Validation(["No se pudo guardar el contacto."]));
            })
            .Tap(c => {
                var guardado = repository.GetByAlias(c.Alias);
                if (guardado != null) cache.Add(guardado.Id, guardado);
            });
    }

    /// <inheritdoc cref="IContactoService.ActualizarContacto" />
    public Result<Contacto, DomainError> ActualizarContacto(int id, Contacto contacto) {
        Log.Debug($"Procesando actualización de Contacto ID {id}");

        return validator.Validar(contacto)
            .Ensure(_ => repository.GetById(id) is not null,
                ContactoErrors.NotFoundById(id))
            .Ensure(c => {
                var existenteTelefono = repository.GetByTelefono(c.Telefono);
                return existenteTelefono == null || existenteTelefono.Id == id;
            }, ContactoErrors.TelefonosRepetidos(contacto.Telefono))
            .Ensure(c => {
                var existenteAlias = repository.GetByAlias(c.Alias);
                return existenteAlias == null || existenteAlias.Id == id;
            }, ContactoErrors.AliasRepetidos(contacto.Alias))
            .Bind(c => {
                var actualizado = repository.Update(id, c);
                return actualizado
                    ? Result.Success<Contacto, DomainError>(c)
                    : Result.Failure<Contacto, DomainError>(ContactoErrors.NotFoundById(id));
            })
            .Tap(c => cache.Add(id, c));
    }

    /// <inheritdoc cref="IContactoService.EliminarContacto" />
    public Result<Contacto, DomainError> EliminarContacto(int id) {
        Log.Warning($"Eliminando Contacto ID: {id}");

        var contacto = repository.GetById(id);
        if (contacto == null)
            return Result.Failure<Contacto, DomainError>(ContactoErrors.NotFoundById(id));

        var eliminado = repository.Delete(id);
        if (!eliminado)
            return Result.Failure<Contacto, DomainError>(ContactoErrors.NotFoundById(id));

        cache.Remove(id);
        return Result.Success<Contacto, DomainError>(contacto);
    }

    /// <inheritdoc cref="IContactoService.EliminarTodosLosContactos" />
    public bool EliminarTodosLosContactos() {
        Log.Warning("Eliminando de forma absoluta todos los contactos");
        var eliminado = repository.DeleteAll();
        if (eliminado) cache.Clear();
        return eliminado;
    }

    public int ContarAgenda() => repository.Count();
}