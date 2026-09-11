using System.Text;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("Logs/log.txt").CreateLogger();
Console.Title = "Agenda de Contactos";
Console.OutputEncoding = Encoding.UTF8;
