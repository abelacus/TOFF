using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Models;
using TOFF.Services;
using TOFF.UI.Pages.Options;
using TOFF.UI.Views;

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
                    DataDataSource = () => [_appState.Preferences.ClientSelection],
                },
                new DataSourceTreeNode
                {
                    Text = "Client Configuration",
                    Tag = typeof(ClientConfiguration),
                    DataDataSource = () => _appState.Preferences.TorrentClientConfig.ToDetailsArray() ?? ["Not Configured"]
                },
                new DataSourceTreeNode {
                    Text = "Torrent Directory",
                    Tag = typeof(TorrentDirectorySelection),
                    DataDataSource = () => [_appState.Preferences.TorrentDirectory ?? "Not Set"]
                },
                new DataSourceTreeNode {
                    Text = "Directories to ignore",
                    Tag = typeof(IgnoreDirectorySelection),
                    DataDataSource = () => _appState.Preferences.IgnoreDirectories.Length > 0 ? [$"{_appState.Preferences.IgnoreDirectories.Length} ignored directories"] : ["No ignored directories"]
                },
                new DataSourceTreeNode {
                    Text = "Path Translations",
                    Tag = typeof(PathTranslationConfiguration),
                    DataDataSource = () => _appState.Preferences.PathTranslations.Count > 0 ? [$"{_appState.Preferences.PathTranslations.Count} translations"] : ["No path translations"]
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
                Width = Dim.Fill(),
                AlignmentModes = AlignmentModes.IgnoreFirstOrLast
            };

            optionsTree.Style.ExpandableSymbol = null;

            optionsTree.AddObjects(options);
            optionsTree.ExpandAll();

            //visually discern non-interactable items
            optionsTree.DrawLine += (_, e) =>
            {
                if(e.Model.Children.Count() == 0)
                {
                    for (int i = e.IndexOfModelText - 2; i < e.Cells.Count; i++)
                    {
                        var cell = e.Cells[i];
                        e.Cells[i] = new Cell
                        {
                            Grapheme = cell.Grapheme,
                            Attribute = new Terminal.Gui.Drawing.Attribute(SchemeManager.GetScheme(Schemes.Base).ReadOnly.Foreground, SchemeManager.GetScheme(Schemes.Base).Normal.Background, TextStyle.None),
                            IsDirty = cell.IsDirty
                        };
                    }
                }

            };

            //disable collapsing of tree elements
            optionsTree.KeyBindings.Remove(Key.CursorLeft);
            optionsTree.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Key.CursorLeft)
                {
                    e.Handled = true;
                }
            };

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
                        var index = options.IndexOf(c => c.Text == parent.Text);
                        if(index + 1 < options.Length)
                        {
                            e.Tree.GoTo(e.Tree.Objects.First(c => c.Text == options[index + 1].Text));
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
                Key = Key.N.WithAlt,
                Text = "Find Orphans",
            };

            Shortcut versionLabel = new Shortcut()
            {
                Action = DisplayVersionInfo,
                Text = "v0.0.1",
            };
            versionLabel.Padding.Thickness = new Thickness(0, 0, 2, 0);

            shortcutBar.Add(backShortcut, nextStep, versionLabel);

            Add(divider, shortcutBar);
        }

        private void Exit()
        {
            _appState.SavePreferences();
            _navigationService.NavigateBack();
        }

        private void DisplayVersionInfo()
        {
            Dialog infoDialog = new Dialog()
            {
                Title = "About",
                X = Pos.Center(),
                Y = Pos.Center(),
            };

            View infoCard = new View()
            {
                Width = Dim.Auto(),
                Height = Dim.Auto(),
            };

            Label name = new Label()
            {
                Text = "Torrent Orphan File Finder",
                X = 0,
                Y = 0
            };
            Label version = new Label()
            {
                Text = "Version: 0.0.1",
                X = 0,
                Y = Pos.Bottom(name) + 1
            };
            Label license = new Label()
            {
                Text = "License: GPLv3",
                X = 0,
                Y = Pos.Bottom(version)
            };
            Label githubLink = new Label()
            {
                Text = "Source: github.com/abelacus/TOFF",
                X = 0,
                Y = Pos.Bottom(license),
            };

            infoCard.Add(name, version, githubLink);
            
            QrCode qrCode = new QrCode("https://github.com/abelacus/TOFF")
            {
                X = Pos.Right(infoCard) + 2
            };
            Label qrCodeLabel = new Label()
            {
                Text = "GitHub",
                X = Pos.Left(qrCode),
                Y = Pos.Bottom(qrCode),
            };
            
            infoDialog.Add(infoCard, qrCode, qrCodeLabel);

            infoDialog.AddButton(new Button() { Title = "Close" });

            _navigationService.RunDialog(infoDialog);
        }
        
        private void SearchAndDisplayResults()
        {
            Dialog errorDialog = new Dialog()
            {
                Title = "Required parameters not set",
                X = Pos.Center(),
                Y = Pos.Center(),
            };

            errorDialog.SetScheme(SchemeManager.GetScheme(Schemes.Error));

            //validate current settings
            if (_appState.Preferences.TorrentClientConfig.ApiUrl == null || _appState.Preferences.TorrentClientConfig.ApiUrl.Length == 0)
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

            if(string.IsNullOrEmpty(_appState.Preferences.TorrentDirectory))
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


            _navigationService.NavigateTo(typeof(SearchPage), false);
        }

    }
}
