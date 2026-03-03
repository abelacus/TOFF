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

            string currentSelection = _appState.clientSelection;

            var clientSelection = new OptionSelector()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
            };

            clientSelection.Labels = _torrentClientService.GetTorrentClients();

            clientSelection.Value = clientSelection.Labels.IndexOf(currentSelection);

            clientSelection.ValueChanged += (_, e) =>
            {
                _appState.clientSelection = clientSelection.Labels[e.NewValue ?? 0];
            };

            Add(clientSelection);

            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(clientSelection),
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
