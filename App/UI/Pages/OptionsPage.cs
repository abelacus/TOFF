using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Models;
using TOFF.Services;
using TOFF.UI.Pages.Options;

namespace TOFF.UI.Pages
{
    internal class OptionsPage : Window, IPage
    {
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;

        public OptionsPage(NavigationService navigationService, AppStateService appState)
        {
            _navigationService = navigationService;
            _appState = appState;

            Title = "Options";

            var options = new[]
            {
                new OptionsMenuItem
                {
                    Text = "Torrent Client",
                    Tag = typeof(ClientSelection),
                    currentValues = [_appState.clientSelection]
                },
                new OptionsMenuItem
                {
                    Text = "Client Configuration",
                    Tag = typeof(ClientConfiguration),
                    currentValues = _appState.torrentClientConfig.ToDetailsArray() ?? ["Not Configured"]
                },
                new OptionsMenuItem {
                    Text = "Torrent Directory"
                },
                new OptionsMenuItem {
                    Text = "Directories to ignore"
                },
                new OptionsMenuItem {
                    Text = "Path Translations"
                },
            };

            var optionsTree = new TreeView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            optionsTree.Style.ExpandableSymbol = null;


            optionsTree.AddObjects(options);
            optionsTree.ExpandAll();

            optionsTree.SelectionChanged += (_, e) =>
            {
                //if it isn't found, that means it's a child node
                if(!e.Tree.Objects.Contains(e.NewValue))
                {
                    var parent = e.Tree.GetParent(e.NewValue);

                    //check whether previous selection was a root item, and whether it's the root of the selected item
                    if (e.Tree.Objects.Contains(e.OldValue) && e.OldValue == parent)
                    {
                        //select next entry
                        var index = options.IndexOf(e => e.Text == parent.Text);
                        e.Tree.GoTo(e.Tree.Objects.First(e => e.Text == options[index + 1].Text));
                    }
                    else
                    {
                        //select parent
                        e.Tree.GoTo(parent);
                    }
                }
            };


            //initialise selected page
            optionsTree.ObjectActivated += (_, entry) =>
            {
                _navigationService.NavigateTo((Type)entry.ActivatedObject.Tag);
            };

            Add(optionsTree);



        }
    }
}
