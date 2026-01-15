using TOFF.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Terminal.Gui.App;
using TOFF.UI;

namespace TOFF
{
    internal class Program
    {
        static void Main(string[] args)
        {


            var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
            {
                services.AddSingleton<TorrentClientService>();
                services.AddSingleton<AppStateService>();
                services.AddSingleton<NavigationService>();
                services.AddSingleton<App>();

                //identifies all pages and adds them dynamically
                var pageTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IPage).IsAssignableFrom(t) && !t.IsInterface);
                foreach (var page in pageTypes)
                {
                    services.AddTransient(page);
                }

            })
            .Build();

            host.Services.GetRequiredService<App>().Run();


        }
    }
}
