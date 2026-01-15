using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TOFF.Services
{
    internal class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Runnable _top;
        private IApplication application;

        private List<Type> windowStack = new List<Type>();

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _top = new Runnable();

            //_top.IsRunningChanging += (_, obj) =>
            //{
            //    if (windowStack.Count > 1 && obj.NewValue == false)
            //    {
            //        obj.Cancel = true;
            //        NavigateBack();
            //    }
            //};
        }

        public void Init()
        { 
            Application.IsMouseDisabled = true;

            using IApplication app = Application.Create().Init();
            application = app;
            app.Run(_top);
            app.Dispose();
        }

        public void RunDialog(Dialog view)
        {
            application.Run(view);
        }

        public void NavigateTo<T>() where T : View
        {
            windowStack.Add(typeof(T));

            _top.RemoveAll();
            var page = (T)_serviceProvider.GetRequiredService(typeof(T));
            _top.Add(page);
        }

        public void NavigateTo(Type nav)
        {
            windowStack.Add(nav);

            _top.RemoveAll();
            var page = (View)_serviceProvider.GetRequiredService(nav);
            _top.Add(page);
        }

        public void NavigateBack()
        {
            Debug.WriteLine(windowStack.Count());
            if(windowStack.Count() <= 1)
            {
                return;
            }
            windowStack.RemoveAt(windowStack.Count - 1);

            _top.RemoveAll();
            var page = (View)_serviceProvider.GetRequiredService(windowStack[^1]);
            _top.Add(page);
        }
    }
}
