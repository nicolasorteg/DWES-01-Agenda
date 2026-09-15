using AgendaContactos.Cache;
using AgendaContactos.Config;
using AgendaContactos.Entity;
using AgendaContactos.Model;
using AgendaContactos.Repository.EfCore;
using AgendaContactos.Service;
using AgendaContactos.Validator.Contactos;
using Microsoft.EntityFrameworkCore;
using Serilog;

// config serilog con los datos del json
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(Enum.Parse<Serilog.Events.LogEventLevel>(AppConfig.LogMinimumLevel))
    .WriteTo.File(
        path: AppConfig.LogFilePath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: AppConfig.LogRetainedFiles,
        outputTemplate: AppConfig.LogOutputTemplate)
    .CreateLogger();

// inicia la bd
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(AppConfig.ConnectionString)
    .Options;
var context = new AppDbContext(options);

var repository = new ContactoEfCoreRepository(context);
var validator = new ContactoValidator();
var cache = new LruCache<int, Contacto>(AppConfig.CacheSize); // capacidad 5
var service = new ContactoService(repository, validator, cache);


Console.WriteLine($"{AppConfig.AppName} - Simulación de peticiones\n");
Console.WriteLine($"Nº de Contactos -> {service.ContarAgenda()}");

Console.WriteLine("- Página 1 -");
foreach (var c in service.ObtenerContactosPaginados()) 
    Console.WriteLine(c);

Console.WriteLine("\n- Página 2 -");
foreach (var c in service.ObtenerContactosPaginados(2)) 
    Console.WriteLine(c);

Console.WriteLine("\n- Página 3 -");
foreach (var c in service.ObtenerContactosPaginados(3)) 
    Console.WriteLine(c);

Console.WriteLine("\n- Obteniendo por ID = 7 -");
Console.WriteLine(service.BuscarContactoPorId(7).Value);

Console.WriteLine("\n- Obteniendo por ID = 999 -");
Console.WriteLine(service.BuscarContactoPorId(999).Error.Message);

Console.WriteLine("\n- Obteniendo por Alias = Nick -");
Console.WriteLine(service.BuscarContactoPorAlias("Nick").Value);

Console.WriteLine("\n- Obteniendo por Alias = EjemploInvalido -");
Console.WriteLine(service.BuscarContactoPorAlias("EjemploInvalido").Error.Message);

Console.WriteLine("\n- Obteniendo por Telefono = 612345678 -");
Console.WriteLine(service.BuscarContactoPorTelefono("612345678").Value);

Console.WriteLine("\n- Obteniendo por Telefono = EjemploInvalido -");
Console.WriteLine(service.BuscarContactoPorTelefono("EjemploInvalido").Error.Message);

Console.WriteLine("\n- Creando contacto nuevo -");
service.CrearContacto(new Contacto()
    { Nombre = "ContactoCreado", Telefono = "666777888", Alias = "Contacti", Email = "contacti@gmail.com" });
Console.WriteLine(service.BuscarContactoPorAlias("Contacti").Value);
Console.WriteLine($"Nº de Contactos -> {service.ContarAgenda()}");


Console.WriteLine("\n- Creando contacto con teléfono existente -");
Console.WriteLine(service.CrearContacto(new Contacto()
    { Nombre = "ContactoRepe", Telefono = "666777888", Alias = "Contacti2", Email = "contacti@gmail.com" }).Error.Message);

Console.WriteLine("\n- Creando contacto con alias existente -");
Console.WriteLine(service.CrearContacto(new Contacto()
    { Nombre = "ContactoRepe", Telefono = "6971022222", Alias = "Contacti", Email = "contacti@gmail.com" }).Error.Message);

Console.WriteLine("\n- Creando contacto con errores de validacion -");
Console.WriteLine(service.CrearContacto(new Contacto()
    { Nombre = "NombreDemasiadoLargo", Telefono = "NoTlf", Alias = "", Email = "FormatoERRONEO" }).Error.Message);

Console.WriteLine("\n - Actualizando primer Contacto -");
service.ActualizarContacto(1, new Contacto() { Nombre = "Actualizado", Alias = "Nuevo", Telefono = "736182736", Email = "nose@nose.com"});
foreach (var c in service.ObtenerContactosPaginados(1, 3)) 
    Console.WriteLine(c);

Console.WriteLine("\n - Actualizando Contacto inexistente -");
Console.WriteLine(service.ActualizarContacto(999, new Contacto() { Nombre = "Invalido", Alias = "Invalido", Telefono = "67436242374", Email = "invalid@invalid.com"}).Error.Message);

Console.WriteLine("\n - Eliminando primer Contacto -");
service.EliminarContacto(1);
foreach (var c in service.ObtenerContactosPaginados(1, 3)) 
    Console.WriteLine(c);

Console.WriteLine("\n - Eliminando Contacto inexistente -");
Console.WriteLine(service.EliminarContacto(999).Error.Message);

Console.WriteLine("\n - Borrando todos los Contactos -");
if (service.EliminarTodosLosContactos()) 
    Console.WriteLine($"Nº de Contactos -> {service.ContarAgenda()}");