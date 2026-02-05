using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.UI;

namespace TOFF.Services
{
    internal class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Runnable _top;
        private IApplication application;

        private List<(Type pageType, bool isBackable)> windowStack = new List<(Type pageType, bool isBackable)>();

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
            Application.IsMouseDisabled = true;

            using IApplication app = Application.Create().Init();
            application = app;
                        _top = new Runnable();

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
            application.Run(view);
            return view.Result ?? -1;
        }

        public void NavigateTo<T>(bool Backable = true) where T : View
        {
            NavigateTo(typeof(T), Backable);
        }

        public void NavigateTo(Type nav, bool Backable = true)
        {
            if (typeof(IPopup).IsAssignableFrom(nav))
            {
                var popup = (IPopup)_serviceProvider.GetRequiredService(nav);
                
                RunDialog(popup.popupWindow);
            }
            else
            {
                windowStack.Add((nav, Backable));
                _top.RemoveAll();
                var page = (View)_serviceProvider.GetRequiredService(nav);
                _top.Add(page);
            }
        }

        public void NavigateBack()
        {
            Debug.WriteLine(windowStack.Count());
            if(windowStack.Count() <= 1)
            {
                _top.RequestStop();
                return;
            }
            windowStack.RemoveAt(windowStack.Count - 1);

            while (windowStack.Count > 0 && !windowStack[^1].isBackable)
            {
                windowStack.RemoveAt(windowStack.Count - 1);
            }

            _top.RemoveAll();
            var page = (View)_serviceProvider.GetRequiredService(windowStack[^1].pageType);
            _top.Add(page);
        }
    }
}
