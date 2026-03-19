using System;
using System.Collections.Generic;
using System.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Services;

namespace TOFF.UI.Pages.Options
{
    internal class TorrentDirectorySelection : IPopup
    {
        private Dialog _popupWindow;
        public Dialog popupWindow => _popupWindow;

        public TorrentDirectorySelection(AppStateService appState, NavigationService navigation)
        {
            //TODO: figure out why the search/find entry doesn't work.
            var directorySelector = new OpenDialog()
            {
                Title = "Torrent Download Directory",
                OpenMode = OpenMode.Directory,
                X = Pos.Center(),
                Y = Pos.Center(),
                AllowsMultipleSelection = false,
                MustExist = true,
            };

            if(appState.preferences.torrentDirectory != null)
            {
                directorySelector.Path = appState.preferences.torrentDirectory;
            }

            directorySelector.FilesSelected += (_, ev) =>
            {
                appState.preferences.torrentDirectory = ev.Dialog.Path;
            };

            _popupWindow = directorySelector;

        }

    }
}
