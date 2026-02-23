using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Terminal.Gui.Input;
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

            //TODO: replace with OptionSelector
            string currentSelection = _appState.clientSelection;
            var clientsList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                ShowMarks = true,
                MarkMultiple = false,
            };

            clientsList.SetSource(new ObservableCollection<string>(_torrentClientService.GetTorrentClients()));

            //marks the current selection in the UI, then sets highlighted item to first entry
            clientsList.SelectedItem = clientsList.Source.ToList().IndexOf(currentSelection);
            clientsList.MarkUnmarkSelectedItem();
            clientsList.SelectedItem = 0;

            clientsList.Accepting += (_, e) =>
            {
                _appState.clientSelection = clientsList.Source.ToList()[(int)clientsList.SelectedItem].ToString();
                navigationService.NavigateBack();

            };

            Add(clientsList);

            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(clientsList),
                Length = Dim.Fill(),
                Orientation = Orientation.Horizontal,
            };

            Bar shortcutBar = new Bar()
            {
                X = 0,
                Y = Pos.Bottom(divider),
                AlignmentModes = AlignmentModes.StartToEnd,
            };

            Shortcut backShortcut = new Shortcut()
            {
                Action = _navigationService.NavigateBack,
                Key = Key.Esc,
                Text = "Back",
            };

            shortcutBar.Add(backShortcut);

            Add(divider, shortcutBar);



        }

    }
}
