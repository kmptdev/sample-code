using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace challenge1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var loggerFactory = LoggerFactory.Create(builder=>
                    {
                        builder.AddConsole();
                    });

                
                    services.AddPalindrome(options=>
                    {

                    }, loggerFactory);

                    services.AddHostedService<PallyHostedService>();
                })
                .RunConsoleAsync();

        }
    }
}
