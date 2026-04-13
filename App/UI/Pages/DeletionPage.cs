using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Services;

namespace TOFF.UI.Pages
{
    internal class DeletionPage : Window, IPage
    {
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;
        public DeletionPage(AppStateService appState, NavigationService navigationService)
        {
            _appState = appState;
            _navigationService = navigationService;

            //show loading spinner
            //once done show dialog with errors, or success message with # of deleted files.
            Label current = new Label()
            {
                Text = "",
                X = Pos.Center(),
                Y = Pos.Center()
            };
            
            SpinnerView spinner = new SpinnerView()
            {
                X = Pos.Left(current) - 2,
                Y = Pos.Center(),
                Style = new SpinnerStyle.Arc(),
                SpinDelay = 90,
            };

            this.Initialized += (_, e) =>
            {
                spinner.AutoSpin = true;
            };

            Add(spinner, current);

            Task.Run(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                foreach (var item in _appState.ToBeDeleted)
                {
                    App.Invoke(() =>
                    {
                        current.Text = "Deleting: " + item.SavePath;
                    });
                    //Thread.Sleep(2000);
                    File.Delete(item.SavePath);
                }

                //reset to nothing just in case
                _appState.ToBeDeleted = Array.Empty<Models.FileInformation>();
                _appState.FilesMissingFromClient = Array.Empty<Models.FileInformation>();

                App.Invoke(() =>
                {
                    spinner.Dispose(); //call dispose before navigating back; crashes otherwise. might want to find a way to handle this within NavigateBack.
                });

                _navigationService.NavigateBack();
            });


        }
    }
}
