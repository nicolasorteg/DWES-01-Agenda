using AgendaContactos.Cache;
using AgendaContactos.Cache.Common;
using AgendaContactos.Config;
using AgendaContactos.Entity;
using AgendaContactos.Model;
using AgendaContactos.Repository.Common;
using AgendaContactos.Repository.EfCore;
using AgendaContactos.Service;
using AgendaContactos.Validator.Common;
using AgendaContactos.Validator.Contactos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AgendaContactos.Infrastructure;

/// <summary>
/// Config. de ID manual
/// </summary>
public static class DependenciesProvider {

    public static IServiceProvider BuildServiceProvider() {
        var services = new ServiceCollection();

        // dbcontext
        Directory.CreateDirectory(AppConfig.DataFolder);
        var dbPath = Path.Combine(AppConfig.DataFolder, "agenda.db");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // cache
        services.AddSingleton<ICache<int, Contacto>>(_ =>
            new LruCache<int, Contacto>(AppConfig.CacheSize));

        // repository
        services.AddScoped<IContactoRepository, ContactoEfCoreRepository>();

        // service
        services.AddScoped<IContactoService, ContactoService>();

        // validator
        services.AddTransient<IValidator<Contacto>, ContactoValidator>();
        
        return services.BuildServiceProvider();
    }
}