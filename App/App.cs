using Microsoft.Extensions.DependencyInjection;
using TOFF.Services;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.UI.Pages;
using Terminal.Gui.Configuration;

namespace TOFF
{
    internal class App
    {
        private readonly IServiceProvider _services;
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;

        public App(IServiceProvider services, AppStateService appState, NavigationService navigationService)
        {
            _services = services;
            _appState = appState;
            _navigationService = navigationService;
        }

        public void Run()
        {
            ConfigurationManager.Enable(ConfigLocations.All);

            _navigationService.Init<OptionsPage>();

        }
    }
}
