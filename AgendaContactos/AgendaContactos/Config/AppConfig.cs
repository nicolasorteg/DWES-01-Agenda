using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace AgendaContactos.Config;

/// <summary>
/// Clase de configuración que lee appsettings.json
/// </summary>
public static class AppConfig {
    
    private static IConfigurationRoot Configuration { get; }
    
    // encendido
    static AppConfig() {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true) // archivo no opcional y cambios reactivos
            .Build();
    }
    
    public static CultureInfo Locale => CultureInfo.GetCultureInfo("es-ES");
    public static string AppName => Configuration.GetValue("AppName", "Agenda Contactos");
    public static string Version => Configuration.GetValue("Version", "1.0.0");
    
    // config repositorio y datos
    public static string DataFolder => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, // portable
        Configuration.GetValue<string>("Repository:Directory") ?? "data");
    
    public static string ConnectionString => 
        Configuration.GetValue<string>("Repository:ConnectionString") ?? "Data Source=data/agenda.db";
    
    public static bool DropData => Configuration.GetValue("Repository:DropData", false);
    public static bool SeedData => Configuration.GetValue("Repository:SeedData", true);
    
    // caché
    public static int CacheSize => Configuration.GetValue("Cache:Size", 10);
    
    // logs
    public static string LogMinimumLevel => 
        Configuration.GetValue<string>("Serilog:MinimumLevel") ?? "Debug";

    public static string LogFilePath => 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
            Configuration.GetValue<string>("Serilog:WriteTo:1:Args:path") ?? "log/log-.txt");
    
    public static int LogRetainedFiles => 
        Configuration.GetValue<int>("Serilog:WriteTo:1:Args:retainedFileCountLimit", 5);

    public static string LogOutputTemplate => 
        Configuration.GetValue<string>("Serilog:WriteTo:1:Args:outputTemplate") ?? 
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
}