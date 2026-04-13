using System.Collections.ObjectModel;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Services;

namespace TOFF.UI.Pages.Options
{
    internal class IgnoreDirectorySelection : Window, IPage
    {
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;

        private readonly ListView _ignoreList;
        private readonly ObservableCollection<string> _ignoreListSource;


        public IgnoreDirectorySelection(AppStateService appState, NavigationService navigationService)
        {
            _appState = appState;
            _navigationService = navigationService;
            Title = "Directories To Ignore";

            _ignoreListSource = new ObservableCollection<string>(_appState.Preferences.IgnoreDirectories);

            _ignoreList = new ListView()
            {
                X = 0,
                Y = 0,
                ShowMarks = false,
                MarkMultiple = false,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
            };

            _ignoreList.SetSource(_ignoreListSource);

            _ignoreList.Accepting += (_, e) =>
            {
                if(_ignoreList.SelectedItem != null && _ignoreList.Source != null && _ignoreList.Source.Count > 0)
                {
                    EditPopup((int)_ignoreList.SelectedItem);
                }
                e.Handled = true;
            };

            Add(_ignoreList);

            //shortcut bar
            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(_ignoreList),
                Length = Dim.Fill(),
                Orientation = Orientation.Horizontal,
            };

            Bar shortcutBar = new Bar()
            {
                X = 0,
                Y = Pos.Bottom(divider),
                AlignmentModes = AlignmentModes.StartToEnd,
            };

            Shortcut infoShortcut = new Shortcut()
            {
                Action = InformationPopup,
                Key = Key.I.WithAlt,
                Text = "Info",
            };

            Shortcut addNewShortcut = new Shortcut()
            {
                Action = AddNewPopup,
                Key = Key.A.WithAlt,
                Text = "Add New",
            };

            Shortcut backShortcut = new Shortcut()
            {
                Action = SaveAndBack,
                Key = Key.Esc,
                Text = "Back",
            };

            shortcutBar.Add(backShortcut, addNewShortcut, infoShortcut);

            Add(divider, shortcutBar);

            if(_ignoreListSource.Count > 0)
            { 
                _ignoreList.SetFocus();
                _ignoreList.SelectedItem = 0;
            }

        }

        private void SaveAndBack()
        {
            _appState.Preferences.IgnoreDirectories = _ignoreListSource.ToArray();

            _navigationService.NavigateBack();
        }

        private void AddNewPopup()
        {
            Dialog addNewPopup = new Dialog()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Title = "Add Directory To Ignore",
                Width = Dim.Percent(40),
            };

            addNewPopup.Padding.Thickness = new Terminal.Gui.Drawing.Thickness(2, 1, 2, 1);

            var label = new Label()
            {
                Title = "Ignore Path:",
                X = 1,
                Y = 0
            };

            var textField = new TextField()
            {
                X = Pos.Right(label) + 1,
                Y = 0,
                Width = Dim.Fill() - 1
            };

            //required for save on enter
            textField.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Key.Enter)
                {
                    addNewPopup.Result = 1;
                }
            };

            addNewPopup.Add(label, textField);

            addNewPopup.AddButton(new() { Title = "Cancel", IsDefault = false });
            addNewPopup.AddButton(new() { Title = "OK", IsDefault = true });


            _navigationService.RunDialog(addNewPopup);

            if (addNewPopup.Result == null)
            {
                if (addNewPopup.Canceled == false)
                {
                    _ignoreListSource.Add(textField.Text);
                }
            }


            if (addNewPopup.Result == 1)
            {
                _ignoreListSource.Add(textField.Text);
            }

            _ignoreList.SetFocus();
        }

        private void EditPopup(int index)
        {
            if(_ignoreListSource.Count == 0)
            {
                return;
            }

            Dialog addNewPopup = new Dialog()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Title = "Update Directory To Ignore",
                Width = Dim.Percent(40),
            };

            addNewPopup.Padding.Thickness = new Terminal.Gui.Drawing.Thickness(2, 1, 2, 1);

            var label = new Label()
            {
                Title = "Ignore Path:",
                X = 1,
                Y = 0
            };

            var textField = new TextField()
            {
                Text = _ignoreListSource.ElementAt(index),
                X = Pos.Right(label) + 1,
                Y = 0,
                Width = Dim.Fill() - 1
            };

            //required for save on enter
            textField.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Key.Enter)
                {
                    addNewPopup.Result = 2;
                }
            };

            addNewPopup.Add(label, textField);

            addNewPopup.AddButton(new() { Title = "Cancel", IsDefault = false });
            addNewPopup.AddButton(new() { Title = "Delete", IsDefault = false });
            addNewPopup.AddButton(new() { Title = "Update", IsDefault = true });


            _navigationService.RunDialog(addNewPopup);

            if (addNewPopup.Result == null)
            {
                if (addNewPopup.Canceled == false)
                {
                    _ignoreListSource[index] = textField.Text;
                }
            }
            if (addNewPopup.Result == 1)
            {
                _ignoreListSource.RemoveAt(index);
            }

            if (addNewPopup.Result == 2)
            {
                _ignoreListSource[index] = textField.Text;
            }
            _ignoreList.SetFocus();
        }

        private void InformationPopup()
        {
            Dialog infoPopup = new Dialog()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Title = "What is this?",
            };

            infoPopup.Padding.Thickness = new Terminal.Gui.Drawing.Thickness(2, 1, 2, 2);


            Label instructionLabel = new Label()
            {
                X = 0,
                Y = 0,
                Text = "Skips requesting additional details for torrents that have files in folders\n" +
                       "outside of of the currently set Torrent Directory\n\n" +
                       "Ignored directories should be formatted as they appear in the torrent client\n" +
                       "before any path translation rules are applied\n\n" +
                       "Can likely be ignored unless you have a large number of torrents being saved\n" +
                       "to multiple download directories",

            };

            infoPopup.Add(instructionLabel);
            infoPopup.AddButton(new Button() { Text = "OK" });


            _navigationService.RunDialog(infoPopup);

        }
    }
}
