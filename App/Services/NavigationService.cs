using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Terminal.Gui.App;
using Terminal.Gui.Drivers;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.UI;

namespace TOFF.Services
{
    internal class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Runnable _top;
        private IApplication _application;

        private List<(Type pageType, bool isBackable)> _windowStack = new List<(Type pageType, bool isBackable)>();

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            //_top.IsRunningChanging += (_, obj) =>
            //{
            //    if (windowStack.Count > 1 && obj.NewValue == false)
            //    {
            //        obj.Cancel = true;
            //        NavigateBack();
            //    }
            //};
        }

        public void Init<T>() where T : View
        { 
            using IApplication app = Application.Create().Init(DriverRegistry.Names.DOTNET);
            _application = app;
            _top = new Runnable();
            app.Mouse.IsMouseDisabled = true;

            NavigateTo<T>();

            app.Run(_top);
            app.Dispose();
        }

        /// <summary>
        /// Runs a dialog and returns the index of the button pressed, or -1 if it exits without a result.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public int RunDialog(Dialog view)
        {

            //we have instances where this is called from an external thread so use invoke instead
            _application.Invoke(() =>
            {
                _application.Run(view);
            });

            return view.Result ?? -1;
        }

        public void NavigateTo<T>(bool backable = true) where T : View
        {
            NavigateTo(typeof(T), backable);
        }

        public void NavigateTo(Type nav, bool backable = true)
        {
            _application.Invoke(() =>
            {
                if (typeof(IPopup).IsAssignableFrom(nav))
                {
                    var popup = (IPopup)_serviceProvider.GetRequiredService(nav);

                    RunDialog(popup.PopupWindow);
                }
                else
                {
                    _windowStack.Add((nav, backable));
                    _top.RemoveAll();
                    var page = (View)_serviceProvider.GetRequiredService(nav);
                    _top.Add(page);
                }
            });
        }

        public void NavigateBack()
        {
            _application.Invoke(() =>
            {
                Debug.WriteLine(_windowStack.Count());
                if (_windowStack.Count() <= 1)
                {
                    _top.RequestStop();
                    return;
                }
                _windowStack.RemoveAt(_windowStack.Count - 1);

                while (_windowStack.Count > 0 && !_windowStack[^1].isBackable)
                {
                    _windowStack.RemoveAt(_windowStack.Count - 1);
                }

                _top.RemoveAll();
                var page = (View)_serviceProvider.GetRequiredService(_windowStack[^1].pageType);
                _top.Add(page);
            });
        }
    }
}
