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

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                foreach (var item in _appState.toBeDeleted)
                {
                    current.Text = "Deleting: " + item.savePath;
                    //Thread.Sleep(2000);
                    File.Delete(item.savePath);
                }

                //reset to nothing just in case
                _appState.toBeDeleted = Array.Empty<Models.FileInformation>();
                _appState.filesMissingFromClient = Array.Empty<Models.FileInformation>();

                _navigationService.NavigateBack();
            }).Start();


        }
    }
}
