using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ProjetBase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /* Pour récupurer ficher de configuration */
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            /* Configurer Serilog */
            Log.Logger = new LoggerConfiguration()
                        .WriteTo.MySQL(config.GetConnectionString("SerilogConnection"))
                        .WriteTo.Seq("http://localhost:5341")
                        .CreateLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
                Log.Information("Server started");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
