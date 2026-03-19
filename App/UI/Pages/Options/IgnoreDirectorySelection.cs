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
    internal class IgnoreDirectorySelection : Window, IPage
    {
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;

        private ListView ignoreList;
        private ObservableCollection<string> ignoreListSource;


        public IgnoreDirectorySelection(AppStateService appState, NavigationService navigationService)
        {
            _appState = appState;
            _navigationService = navigationService;
            Title = "Directories To Ignore";

            ignoreListSource = new ObservableCollection<string>(_appState.preferences.IgnoreDirectories);

            ignoreList = new ListView()
            {
                X = 0,
                Y = 0,
                ShowMarks = false,
                MarkMultiple = false,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
            };

            ignoreList.SetSource(ignoreListSource);

            ignoreList.Accepting += (_, e) =>
            {
                if(ignoreList.SelectedItem != null && ignoreList.Source.Count > 0)
                {
                    EditPopup((int)ignoreList.SelectedItem);
                }
                e.Handled = true;
            };

            Add(ignoreList);

            //shortcut bar
            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(ignoreList),
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

            if(ignoreListSource.Count > 0)
            { 
                ignoreList.SetFocus();
                ignoreList.SelectedItem = 0;
            }

        }

        private void SaveAndBack()
        {
            _appState.preferences.IgnoreDirectories = ignoreListSource.ToArray();

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
                    ignoreListSource.Add(textField.Text.ToString());
                }
            }


            if (addNewPopup.Result == 1)
            {
                ignoreListSource.Add(textField.Text.ToString());
            }

            ignoreList.SetFocus();
        }

        private void EditPopup(int index)
        {
            if(ignoreListSource.Count == 0)
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
                Text = ignoreListSource.ElementAt(index),
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
                    ignoreListSource[index] = textField.Text.ToString();
                }
            }
            if (addNewPopup.Result == 1)
            {
                ignoreListSource.RemoveAt(index);
            }

            if (addNewPopup.Result == 2)
            {
                ignoreListSource[index] = textField.Text.ToString();
            }
            ignoreList.SetFocus();
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
