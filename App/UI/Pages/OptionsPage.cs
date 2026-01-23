using Terminal.Gui.Drawing;
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
                new DataSourceTreeNode
                {
                    Text = "Torrent Client",
                    Tag = typeof(ClientSelection),
                    DataDataSource = () => [_appState.clientSelection]
                },
                new DataSourceTreeNode
                {
                    Text = "Client Configuration",
                    Tag = typeof(ClientConfiguration),
                    DataDataSource = () => _appState.torrentClientConfig.ToDetailsArray() ?? ["Not Configured"]
                },
                new DataSourceTreeNode {
                    Text = "Torrent Directory",
                    Tag = typeof(TorrentDirectorySelection),
                    DataDataSource = () => [_appState.torrentDirectory ?? "Not Set"]
                },
                new DataSourceTreeNode {
                    Text = "Directories to ignore",
                    Tag = typeof(IgnoreDirectorySelection),
                    DataDataSource = () => _appState.IgnoreDirectories.Length > 0 ? [$"{_appState.IgnoreDirectories.Length} ignored directories"] : ["No ignored directories"]
                },
                new DataSourceTreeNode {
                    Text = "Path Translations",
                    Tag = typeof(PathTranslationConfiguration),
                    DataDataSource = () => _appState.PathTranslations.Count > 0 ? [$"{_appState.PathTranslations.Count} translations"] : ["No path translations"]
                },
            };


            var optionsTree = new TreeView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
            };

            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(optionsTree),
                Length = Dim.Fill(),
                Orientation = Orientation.Horizontal,
            };

            Bar shortcutBar = new Bar()
            {
                X = 0,
                Y = Pos.Bottom(divider),
                AlignmentModes = AlignmentModes.StartToEnd,
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
                        if(index + 1 < options.Length)
                        {
                            e.Tree.GoTo(e.Tree.Objects.First(e => e.Text == options[index + 1].Text));
                        }
                        else
                        {
                            e.Tree.GoTo(parent);
                            shortcutBar.SetFocus();
                        }
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
                optionsTree.RebuildTree();
            };

            Add(optionsTree);

            Shortcut backShortcut = new Shortcut()
            {
                Action = Exit,
                Key = Key.Esc,
                Text = "Quit",
            };

            Shortcut nextStep = new Shortcut()
            {
                Action = SearchAndDisplayResults,
                Key = Key.N.WithCtrl.WithAlt,
                Text = "Find Orphans",
            };

            shortcutBar.Add(backShortcut, nextStep);

            Add(divider, shortcutBar);
        }

        private void Exit()
        {
            _navigationService.NavigateBack();
        }
        
        private void SearchAndDisplayResults()
        {


            Dialog errorDialog = new Dialog()
            {
                Title = "Required parameters not set",
                X = Pos.Center(),
                Y = Pos.Center(),
            };

            errorDialog.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(Color.BrightRed, Color.Black)));

            //validate current settings
            if (_appState.torrentClientConfig.ApiURL == null || _appState.torrentClientConfig.ApiURL.Length == 0)
            {
                Label errorLabel = new Label()
                {
                    Text = "API url must be set",
                    X = Pos.Center() + 1,
                    Y = 1,
                    Height = 1,
                    CanFocus = false,
                };

                errorDialog.Add(errorLabel);
                errorDialog.AddButton(new () { Title = "Ok" });
                _navigationService.RunDialog(errorDialog);

                return;
                //show popup
            }

            if(_appState.torrentDirectory == null || _appState.torrentDirectory.Length == 0)
            {
                Label errorLabel = new Label()
                {
                    Title = "Local directory to scan and compare for files must be set",
                    X = Pos.Center(),
                    Y = Pos.Center(),
                    Height = 1,
                    CanFocus = false,
                };

                errorDialog.Add(errorLabel);
                errorDialog.AddButton(new Button() { Title = "Ok" });
                _navigationService.RunDialog(errorDialog);

                return;
            }


            _navigationService.NavigateTo(typeof(SearchPage));
        }

    }
}
