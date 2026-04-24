using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Services;

namespace TOFF.UI.Pages.Options
{
    internal class TorrentDirectorySelection : IPopup
    {
        public Dialog PopupWindow { get; }

        public TorrentDirectorySelection(AppStateService appState)
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

            if(appState.Preferences.TorrentDirectory != null)
            {
                directorySelector.Path = appState.Preferences.TorrentDirectory;
            }

            directorySelector.FilesSelected += (_, ev) =>
            {
                appState.Preferences.TorrentDirectory = ev.Dialog.Path;
            };

            PopupWindow = directorySelector;

        }

    }
}
