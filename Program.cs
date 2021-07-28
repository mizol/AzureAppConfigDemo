using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace AzureAppConfigDemo
{
    public class Program
    {
        public static void Main(string[] args) => 
            CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(config => {
                        var settings = config.Build();
                        // .NET CLI
                        // dotnet user-secrets set ConnectionStrings:AppConfig "<your_connection_string>"
                        // ConnectionString "Endpoint=https://app-config-provider-demo.azconfig.io;Id=..."
                        // Stored locally in app secret.json and not versioned
                        var connection = settings.GetConnectionString("AppConfig");
                        //var connection = settings["ConnectionStrings:AppConfig"];

                        config.AddAzureAppConfiguration(options => {
                            options.Connect(connection);

                            options.ConfigureRefresh(refresh =>
                            {
                                refresh.Register("TestApp:Settings:Sentinel", refreshAll: true);
                                refresh.SetCacheExpiration(new TimeSpan(0, 5, 0));
                            });
                        });


                    });

                    webBuilder.UseStartup<Startup>();
                });

        // Initial version
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
