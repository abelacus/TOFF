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

            ignoreListSource = new ObservableCollection<string>(_appState.IgnoreDirectories);

            ignoreList = new ListView()
            {
                X = 0,
                Y = 0,
                AllowsMarking = false,
                AllowsMultipleSelection = false,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1,
                BorderStyle = Terminal.Gui.Drawing.LineStyle.Single,
            };

            ignoreList.SetSource(ignoreListSource);

            ignoreList.OpenSelectedItem += (_, e) =>
            {
                EditPopup((int)e.Item);
            };

            Add(ignoreList);

            //shortcut bar
            Bar shortcutBar = new Bar()
            {
                X = 0,
                Y = Pos.Bottom(ignoreList),
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

            shortcutBar.Add(infoShortcut, addNewShortcut, backShortcut);

            Add(shortcutBar);

            if(ignoreListSource.Count > 0)
            { 
                ignoreList.SetFocus();
                ignoreList.SelectedItem = 0;
            }

        }

        private void SaveAndBack()
        {
            _appState.IgnoreDirectories = ignoreListSource.ToArray();

            _navigationService.NavigateBack();
        }

        private void AddNewPopup()
        {
            Dialog addNewPopup = new Dialog()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Title = "Add Directory To Ignore"
            };

            var label = new Label()
            {
                Title = "Ignore Path:",
                X = 1,
                Y = 0
            };

            var textField = new TextField()
            {
                X = Pos.Right(label) + 1,
                Y = 1,
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
            Dialog addNewPopup = new Dialog()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Title = "Add Directory To Ignore"
            };

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
                Y = 1,
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
                Title = "What is this?"
            };

            Label instructionLabel = new Label()
            {
                X = 0,
                Y = 0,
                Text = "Ignored directories should be formatted as they appear in the torrent client.\n" +
                        "Skips requesting additional details for torrents that have files in folders outside of of the system [green]Torrent Directory[/] ",

            };

            infoPopup.Add(instructionLabel);
            infoPopup.AddButton(new Button() { Text = "OK" });


            _navigationService.RunDialog(infoPopup);

        }
    }
}
