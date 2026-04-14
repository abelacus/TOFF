using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Models;
using TOFF.Services;

namespace TOFF.UI.Pages
{
    internal class ResultsTablePage : Window, IPage
    {
        private readonly NavigationService _navigationService;
        private readonly AppStateService _appState;

        private TableView table;
        public ResultsTablePage(NavigationService navigationService, AppStateService appState)
        {
            _navigationService = navigationService;
            _appState = appState;

            table = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                MultiSelect = true,
                
                FullRowSelect = true,
            };
            table.KeyBindings.ReplaceCommands(Key.Space, Command.Toggle);
            table.Style.AlwaysShowHeaders = true;

            //TODO: revisit when/if the Terminal.Gui TableView gains the ability to visually discern between toggled entries and the one that's been selected with the arrow keys
            //Scheme scheme = new Scheme(SchemeManager.GetScheme("Base"))
            //{
            //    Focus = new Terminal.Gui.Drawing.Attribute(Color.Red, Color.Magenta),
            //    Active = new Terminal.Gui.Drawing.Attribute(Color.Green, Color.BrightYellow),
            //    Disabled = new Terminal.Gui.Drawing.Attribute(Color.Blue, Color.White),
            //};

            Scheme selectedStyle = new Scheme(new Terminal.Gui.Drawing.Attribute(new Color(34, 90, 109), StandardColor.AmberPhosphor));
            Scheme selectedCursor = new Scheme(new Terminal.Gui.Drawing.Attribute(StandardColor.CornflowerBlue, StandardColor.HoneyDew));

            table.Style.ColumnStyles[0] = new ColumnStyle
            {
                RepresentationGetter = (value) => value.ToString().Remove(0, _appState.Preferences.TorrentDirectory!.Length),

                Alignment = Alignment.Fill,
            };

            table.Style.RowColorGetter = (args) => // ugly af but good enough i guess
            {
                if (table.IsSelected(0, args.RowIndex) && table.SelectedRow != args.RowIndex)
                {
                    return selectedStyle;
                }
                if(table.MultiSelectedRegions.Any(r => r.Rectangle.Location.Y == args.RowIndex) && table.SelectedRow == args.RowIndex) //don't use table.IsSelected since that returns true for the cursors position
                {
                    return selectedCursor;
                }
                return null;
            };
            

            table.Table = new EnumerableTableSource<FileInformation>(_appState.FilesMissingFromClient,
                    new Dictionary<string, Func<FileInformation, object>>()
                    {
                        { "File Path", (p) => p.SavePath },
                        { "Links", (p) => p.Links },
                        { "Creation Date", (p) => p.CreationDate.ToShortDateString() },
                        { "Last Modified Date", (p) => p.LastModifiedDate.ToShortDateString() },
                    }
                );


            table.KeyDown += (_, e) =>
            {
                //remove unwanted keybinds
                if (e.KeyCode == Key.CursorLeft || e.KeyCode == Key.CursorRight || e.KeyCode == Key.CursorLeft.WithShift || e.KeyCode == Key.CursorRight.WithShift)
                {
                    e.Handled = true;
                }
                
                if (e.KeyCode == Key.CursorUp.WithShift )
                {
                    table.SelectedRow -= 1;
                    table.SetNeedsDraw();
                    e.Handled = true;
                }
                if (e.KeyCode == Key.CursorDown.WithShift )
                {
                    table.SelectedRow += 1;
                    table.SetNeedsDraw();
                    e.Handled = true;
                }
                //because the default select all works weirdly, add each item individually instead.
                if (e.KeyCode == Key.A.WithCtrl)
                {
                    if(table.MultiSelectedRegions.Sum(e => e.Rectangle.Height) == table.Table.Rows)
                    {
                        table.MultiSelectedRegions.Clear();
                        table.SetNeedsDraw();
                        e.Handled = true;
                        return;
                    }

                    //add all not-selected entries to the list. might have performance issues on large numbers of files.
                    var selected = table.MultiSelectedRegions.Select(e => e.Origin.Y);
                    
                    for (int i = 0; i < table.Table.Rows; i++)
                    {
                        if(!selected.Any(e => e == i))
                        {
                            //TODO: figure out a way to speed this up with large numbers of items. 
                            table.MultiSelectedRegions.Push(new TableSelection(new System.Drawing.Point(0, i), new System.Drawing.Rectangle(0, i, 1, 1)) { IsToggled = true });
                        }
                    }
                    table.SetNeedsDraw();
                    e.Handled = true;
                }

                if(e.KeyCode == Key.Space)
                {
                    table.SetNeedsDraw(); //forces visual update on selection
                    return;
                }
            };

            // show popup with all info when pressing 'enter' on selected file. should show non-truncated file name and directory.
            // include 'select' button to mark file for deletion alongside 'ok' to close popup.
            table.CellActivated += (_, e) =>
            {
                FileInformation fileInfo = _appState.FilesMissingFromClient[e.Row];

                Scheme textViewScheme = new Scheme()
                {
                    ReadOnly = new Terminal.Gui.Drawing.Attribute(StandardColor.LightBlue, StandardColor.RaisinBlack)
                };

                Dialog infoPopup = new Dialog()
                {
                    Title = "File Info",
                    Width = Dim.Percent(65),
                    X = Pos.Center(),
                    Y = Pos.Center(),
                };

                infoPopup.Padding.Thickness = new Thickness(2, 1, 2, 1);

                Label nameLabel = new Label()
                {
                    Title = "File Name     :",
                    X = 0,
                    Y = 0,
                    Height = 1,
                };

                TextView nameValue = new TextView()
                {
                    Text = fileInfo.SavePath,
                    X = Pos.Right(nameLabel) + 1,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Func((v) => { //This should probably be improved. Dim.Auto doesn't work as expected, leaves excess lines or uses a height of 0
                        return (int)Math.Ceiling((decimal)fileInfo.SavePath.Length / (decimal)(this.Frame.Width * 0.65 - 19));
                    }),
                    CanFocus = false,
                    WordWrap = true,
                    ReadOnly = true,
                };
                nameValue.SetScheme(textViewScheme);


                Label linksLabel = new Label()
                {
                    Title = "Hard Links    :",
                    X = 0,
                    Y = Pos.Bottom(nameValue),
                };

                TextView linksValue = new TextView()
                {
                    Text = fileInfo.Links.ToString(),
                    X = Pos.Right(linksLabel) + 1,
                    Y = linksLabel.Y,
                    Width = fileInfo.Links.ToString().Length,
                    Height = 1,
                    CanFocus = false,
                    WordWrap = true,
                    ReadOnly = true,
                };
                linksValue.SetScheme(textViewScheme);

                Label creationDateLabel = new Label()
                {
                    Title = "Creation Date :",
                    X = 0,
                    Y = Pos.Bottom(linksLabel),
                };

                TextView creationDateValue = new TextView()
                {
                    Text = fileInfo.CreationDate.ToString("yyyy-MM-dd"),
                    X = Pos.Right(creationDateLabel) + 1,
                    Y = creationDateLabel.Y,
                    Width = fileInfo.CreationDate.ToString("yyyy-MM-dd").Length,
                    Height = 1,
                    CanFocus = false,
                    WordWrap = true,
                    ReadOnly = true,
                };
                creationDateValue.SetScheme(textViewScheme);

                Label modifiedDateLabel = new Label()
                {
                    Title = "Modified Date :",
                    X = 0,
                    Y = Pos.Bottom(creationDateLabel),
                };

                TextView modifiedDateValue = new TextView()
                {
                    Text = fileInfo.LastModifiedDate.ToString("yyyy-MM-dd"),
                    X = Pos.Right(modifiedDateLabel) + 1,
                    Y = modifiedDateLabel.Y,
                    Width = fileInfo.LastModifiedDate.ToString("yyyy-MM-dd").Length,
                    Height = 1,
                    CanFocus = false,
                    WordWrap = true,
                    ReadOnly = true,
                };
                modifiedDateValue.SetScheme(textViewScheme);

                infoPopup.Add(nameLabel, nameValue, linksLabel, linksValue, creationDateLabel, creationDateValue, modifiedDateLabel, modifiedDateValue);

                Button selectButton = new Button()
                {
                    Title = table.MultiSelectedRegions.Any(s => s.Rectangle.Location.Y == e.Row) ? "De-Select" : "Select",
                };

                infoPopup.AddButton(selectButton);

                infoPopup.AddButton(new() { Title = "Ok" });

                _navigationService.RunDialog(infoPopup);

                if(infoPopup.Result == 0)
                {
                    table.NewKeyDownEvent(Key.Space);
                }
            };

            Add(table);

            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(table),
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

            Shortcut deleteSelectedShortcut = new Shortcut()
            {
                Action = StartRemoveJob,
                Key = Key.D.WithCtrl.WithAlt,
                Text = "Delete Selected",
            };

            shortcutBar.Add(backShortcut, deleteSelectedShortcut);

            Add(divider, shortcutBar);
        }

        private void StartRemoveJob()
        {
            if(table.MultiSelectedRegions.Count() == 0)
            {
                return;
            }

            Dialog errorDialog = new Dialog()
            {
                Title = "Are you sure?",
                X = Pos.Center(),
                Y = Pos.Center(),
            };

            errorDialog.SetScheme(SchemeManager.GetScheme("Error"));

            Label errorLabel = new Label()
            {
                Text = $"You are about to delete {table.MultiSelectedRegions.Count()} files",
                X = Pos.Center() + 1,
                Y = 1,
                Height = 1,
                CanFocus = false,
            };

            errorDialog.Add(errorLabel);
            errorDialog.AddButton(new() { Title = "Cancel" });
            errorDialog.AddButton(new() { Title = "Continue" });

            _navigationService.RunDialog(errorDialog);

            if(errorDialog.Result != 1)
            {
                return;
            }

            int[] indexes = table.MultiSelectedRegions.Select(e => e.Origin.Y).ToArray();

            _appState.ToBeDeleted = indexes.Select(i => _appState.FilesMissingFromClient[i]).ToArray();

            _navigationService.NavigateTo<DeletionPage>(false);
        }
    }
}
