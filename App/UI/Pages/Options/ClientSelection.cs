using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Services;

namespace TOFF.UI.Pages.Options
{
    internal class ClientSelection : Window, IPage
    {
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;
        private readonly TorrentClientService _torrentClientService;

        public ClientSelection(AppStateService appState, NavigationService navigationService, TorrentClientService torrentClientService)
        {
            _appState = appState;
            _navigationService = navigationService;
            _torrentClientService = torrentClientService;

            Title = "Select Torrent Client";

            string currentSelection = _appState.clientSelection;
            var clientsList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMarking = true,
                AllowsMultipleSelection = false,
            };

            clientsList.SetSource(new ObservableCollection<string>(_torrentClientService.GetTorrentClients()));

            //marks the current selection in the UI, then sets highlighted item to first entry
            clientsList.SelectedItem = clientsList.Source.ToList().IndexOf(currentSelection);
            clientsList.MarkUnmarkSelectedItem();
            clientsList.SelectedItem = 0;

            clientsList.OpenSelectedItem += (_, val) =>
            {
                _appState.clientSelection = val.Value.ToString();
                navigationService.NavigateBack();

            };


            Add(clientsList);



        }

    }
}
