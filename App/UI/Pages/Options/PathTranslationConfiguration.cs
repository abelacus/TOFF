using System.Collections.ObjectModel;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Models;
using TOFF.Services;

namespace TOFF.UI.Pages.Options
{
    internal class PathTranslationConfiguration : Window, IPage
    {
        private readonly NavigationService _navigationService;
        private readonly AppStateService _appState;

        private readonly Dictionary<string, string> _translations;
        private readonly ObservableCollection<KeyValueListItem> _translationListSource;

        public PathTranslationConfiguration(AppStateService appState, NavigationService navigationService)
        {
            _appState = appState;
            _navigationService = navigationService;

            Title = "Path Translations";

            _translations = _appState.Preferences.PathTranslations;

            ListView translationList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                ShowMarks = false,
            };

            _translationListSource = new ObservableCollection<KeyValueListItem>(_translations.Select(e => new KeyValueListItem(e)));

            translationList.SetSource(_translationListSource);

            translationList.Accepting += (_, e) =>
            {
                if(translationList.SelectedItem != null && _translationListSource.Count > 0)
                {
                    UpdateTranslation((int)translationList.SelectedItem);
                }
                e.Handled = true;
            };

            Add(translationList);

            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(translationList),
                Length = Dim.Fill(),
                Orientation = Orientation.Horizontal,
            };

            Bar shortcutBar = new Bar()
            {
                X = 0,
                Y = Pos.Bottom(divider),
                AlignmentModes = AlignmentModes.StartToEnd,
            };

            Shortcut addNewShortcut = new Shortcut()
            {
                Action = AddNewTranslation,
                Key = Key.A,
                Text = "Add Translation",
            };

            Shortcut backShortcut = new Shortcut()
            {
                Action = SaveAndBack,
                Key = Key.Esc,
                Text = "Save and Back",
            };

            Shortcut discardShortcut = new Shortcut()
            {
                Action = DiscardAndBack,
                Key = Key.Esc.WithShift,
                Text = "Discard Changes",
            };

            Shortcut aboutShortcut = new Shortcut()
            {
                Action = ShowAboutPopup,
                Key = Key.I.WithAlt,
                Text = "Info"
            };

            shortcutBar.Add(backShortcut, discardShortcut, addNewShortcut, aboutShortcut);
            translationList.SetFocus();

            Add(divider, shortcutBar);

        }

        private void AddNewTranslation()
        {
            Dialog translationWizard = new Dialog()
            {
                Title = "Add Path Translation",
                Width = Dim.Percent(50),
            };

            translationWizard.Padding.Thickness = new Thickness(2, 1, 2, 1);

            View inputView = new View()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(60),
                Height = Dim.Fill(),
                CanFocus = true,
            };
            inputView.Margin.Thickness = new Thickness(0, 0, 4, 0);

            Label description = new Label()
            {
                X = Pos.Right(inputView),
                Y = 0,
                Width = Dim.Percent(40),
                Height = 9,
                Text = "The path you want to translate as it appears in the torrent client. In a Docker Compose file, this would be the part after the 'destination' of a volume mount",
            };

            Label clientPathLabel = new Label()
            {
                X = 0,
                Y = 0,
                Title = "From Path:",
            };

            TextField clientPath = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(clientPathLabel),
                Width = Dim.Fill(),
            };

            Label localPathLabel = new Label()
            {
                X = 0,
                Y = Pos.Bottom(clientPath),
                Title = "To Path:"
            };

            TextField localPath = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(localPathLabel),
                Width = Dim.Fill(),
            };

            Button directorySelectButton = new Button()
            {
                X = Pos.Right(localPathLabel) + 1,
                Y = Pos.Bottom(clientPath),
                Title = "Select Folder"
            };

            directorySelectButton.Activating += (_, e) =>
            {
                string? selectedDirectory = ShowFolderSelectPopup(localPath.Text.Length > 0 ? localPath.Text : null);
                if(selectedDirectory != null)
                {
                    localPath.Text = selectedDirectory;
                }
                e.Handled = true;
            };

            //add dialog buttons
            Button cancelButton = new Button()
            {
                Title = "Cancel",
                IsDefault = false,
            };

            Button submitButton = new Button()
            {
                Title = "Submit",
                IsDefault = true,
            };

            clientPath.KeyDown += (_, e) =>
            {
                if(e.KeyCode == Key.Enter)
                {
                    localPath.SetFocus();
                    e.Handled = true;
                }
            };

            localPath.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Key.Enter)
                {
                    submitButton.SetFocus();
                    e.Handled = true;
                }
            };

            inputView.FocusedChanged += (_, e) =>
            {
                if (e.NewFocused == clientPath)
                {
                    description.Text = "The path you want to translate as it appears in the torrent client. In a Docker Compose file, this would be the 'destination' of a volume mount";
                }
                if (e.NewFocused == localPath || e.NewFocused == directorySelectButton)
                {
                    description.Text = "The destination path that the path should be translated to. In a Docker Compose file, this would be the part 'source' of a volume mount";
                }
            };

            inputView.Add(clientPathLabel, clientPath, localPathLabel, directorySelectButton, localPath);

            translationWizard.Add(inputView, description);
            translationWizard.AddButton(cancelButton);
            translationWizard.AddButton(submitButton);

            submitButton.Activating += (_, e) =>
            {
                if (clientPath.Text.Length == 0)
                {
                    Dialog errorDialog = new Dialog()
                    {
                        Title = "Empty Client Path",
                        X = Pos.Center(),
                        Y = Pos.Center(),
                    };

                    errorDialog.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(StandardColor.BrightRed, StandardColor.RaisinBlack)));

                    var errorLabel = new Label()
                    {
                        Title = "From Path cannot be empty",
                        Height = 1,
                        X = 1,
                        Y = 1,
                    };

                    errorDialog.Add(errorLabel);
                    errorDialog.AddButton(new() { Title = "Ok" });
                    errorLabel.VerticalScrollBar.Visible = false;
                    _navigationService.RunDialog(errorDialog);

                    e.Handled = true;
                    return;
                }

                if (_translations.ContainsKey(clientPath.Text))
                { 
                    Dialog errorDialog = new Dialog()
                    {
                        Title = "Key already exists",
                        X = Pos.Center(),
                        Y = Pos.Center(),
                    };

                    errorDialog.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(StandardColor.BrightRed, StandardColor.RaisinBlack)));

                    var errorLabel = new Label()
                    {
                        Title = "Replace existing entry?",
                        Height = 1,
                        X = 1,
                        Y = 1,
                    };

                    errorDialog.Add(errorLabel);
                    errorDialog.AddButton(new() { Title = "No" });
                    errorDialog.AddButton(new() { Title = "Yes" });
                    errorLabel.VerticalScrollBar.Visible = false;
                    _navigationService.RunDialog(errorDialog);

                    if(errorDialog.Canceled || errorDialog.Result == 0)
                    {
                        e.Handled = true;
                    }

                    if(errorDialog.Result == 1)
                    {
                        e.Handled = false;
                    }
                }
            };

            _navigationService.RunDialog(translationWizard);
            
            if(translationWizard.Result == 1)
            {
                _translations[clientPath.Text] = localPath.Text;
                UpdateListView();
            }

        }

        private void UpdateListView()
        {
            foreach(var entry in _translations)
            {
                if(_translationListSource.Any(e => e.Key == entry.Key))
                {
                    _translationListSource[_translationListSource.IndexOf(_translationListSource.First(e => e.Key == entry.Key))].Value = entry.Value;
                }
                else
                {
                    _translationListSource.Add(new KeyValueListItem(entry));
                }
            }

            //remove entries no longer present
            foreach(var entry in _translationListSource.ToList())
            {
                if (!_translations.ContainsKey(entry.Key))
                {
                    _translationListSource.Remove(entry);
                }
            }
        }


        private void SaveAndBack()
        {
            _appState.Preferences.PathTranslations = _translations;

            _navigationService.NavigateBack();
        }

        private void DiscardAndBack()
        {
            _navigationService.NavigateBack();
        }

        private void UpdateTranslation(int i)
        {
            Dialog updateWizard = new Dialog()
            {
                Title = "Edit Path Translation",
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            updateWizard.Padding.Thickness = new Thickness(2, 1, 2, 1);

            View inputView = new View()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(60),
                Height = Dim.Fill(),
                CanFocus = true,
            };
            inputView.Margin.Thickness = new Thickness(0, 0, 4, 0);


            Label description = new Label()
            {
                X = Pos.AnchorEnd() + 1,
                Y = 0,
                Width = Dim.Percent(40),
                Height = 9,
                Text = "The path you want to translate as it appears in the torrent client. In a Docker Compose file, this would be the part after the 'destination' of a volume mount",
            };

            Label clientPathLabel = new Label()
            {
                X = 0,
                Y = 0,
                Title = "From Path:",
            };

            TextField clientPath = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(clientPathLabel),
                Width = Dim.Fill(),
                Text = _translationListSource[i].Key,
            };

            Label localPathLabel = new Label()
            {
                X = 0,
                Y = Pos.Bottom(clientPath),
                Title = "To Path:"
            };

            TextField localPath = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(localPathLabel),
                Width = Dim.Fill(),
                Text = _translationListSource[i].Value,
            };

            Button directorySelectButton = new Button()
            {
                X = Pos.Right(localPathLabel) + 1,
                Y = Pos.Bottom(clientPath),
                Title = "Select Folder"
            };

            directorySelectButton.Activating += (_, e) =>
            {
                string? selectedDirectory = ShowFolderSelectPopup(localPath.Text.Length > 0 ? localPath.Text : null);
                if (selectedDirectory != null)
                {
                    localPath.Text = selectedDirectory;
                }
                e.Handled = true;
            };

            //add dialog buttons
            Button cancelButton = new Button()
            {
                Title = "Cancel",
                IsDefault = false,
            };

            Button deleteButton = new Button()
            {
                Title = "Delete",
                IsDefault = false,
            };

            Button submitButton = new Button()
            {
                Title = "Update",
                IsDefault = true,
            };

            clientPath.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Key.Enter)
                {
                    localPath.SetFocus();
                    e.Handled = true;
                }
            };

            localPath.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Key.Enter)
                {
                    submitButton.SetFocus();
                    e.Handled = true;
                }
            };

            inputView.FocusedChanged += (_, e) =>
            {
                if (e.NewFocused == clientPath)
                {
                    description.Text = "The path you want to translate as it appears in the torrent client. In a Docker Compose file, this would be the 'destination' of a volume mount";
                }
                if (e.NewFocused == localPath || e.NewFocused == directorySelectButton)
                {
                    description.Text = "The destination path that the path should be translated to. In a Docker Compose file, this would be the part 'source' of a volume mount";
                }
            };

            inputView.Add(clientPathLabel, clientPath, localPathLabel, directorySelectButton, localPath);

            updateWizard.Add(inputView, description);
            updateWizard.AddButton(cancelButton);
            updateWizard.AddButton(deleteButton);
            updateWizard.AddButton(submitButton);

            submitButton.Activating += (_, e) =>
            {
                if (clientPath.Text.Length == 0)
                {
                    Dialog errorDialog = new Dialog()
                    {
                        Title = "Empty Client Path",
                        X = Pos.Center(),
                        Y = Pos.Center(),
                    };

                    errorDialog.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(StandardColor.BrightRed, StandardColor.RaisinBlack)));

                    var errorLabel = new Label()
                    {
                        Title = "From Path cannot be empty",
                        Height = 1,
                        X = 1,
                        Y = 1,
                    };

                    errorDialog.Add(errorLabel);
                    errorDialog.AddButton(new() { Title = "Ok" });
                    errorLabel.VerticalScrollBar.Visible = false;
                    _navigationService.RunDialog(errorDialog);

                    e.Handled = true;
                }
            };


            _navigationService.RunDialog(updateWizard);

            //delete button
            if(updateWizard.Result == 1)
            {
                _translations.Remove(_translationListSource[i].Key);
                UpdateListView();
            }

            //update button
            if (updateWizard.Result == 2)
            {
                if (_translationListSource[i].Key != clientPath.Text)
                {
                    _translations.Remove(_translationListSource[i].Key);
                }
                _translations[clientPath.Text] = localPath.Text;
                UpdateListView();
            }
        }

        private string? ShowFolderSelectPopup(string? currentPath)
        {
            var directorySelector = new OpenDialog()
            {
                OpenMode = OpenMode.Directory,
                X = Pos.Center(),
                Y = Pos.Center(),
                AllowsMultipleSelection = false,
                
            };

            if(currentPath != null)
            {
                directorySelector.Path = currentPath;
            }

            string? result = null;

            directorySelector.FilesSelected += (_, ev) =>
            {
                result = ev.Dialog.Path;
            };

            _navigationService.RunDialog(directorySelector);
            return result;

        }

        private void ShowAboutPopup()
        {
            Dialog infoPopup = new Dialog()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Title = "What is this?",
            };

            infoPopup.Padding.Thickness = new Thickness(2, 1, 2, 2);


            Label instructionLabel = new Label()
            {
                X = 0,
                Y = 0,
                Text = "Define rules for translating parts of a path as they appear in the torrent client\n" +
                       "to how they appear on the host machine\n\n" +
                       "Useful for when the client is hosted in Docker with a mount position that differs to the host\n\n" +
                       "e.g. A translation of /Data -> /mnt/disk1 would replace the instance of /Data with /mnt/disk1\n" +
                       "     in any given path, enabling the program to properly compare torrent files to the file list."
            };

            Label autoTranslateLabel = new Label()
            {
                X = 0,
                Y = Pos.Bottom(instructionLabel) + 1,
                Text = "IMPORTANT: Automatic translation is performed when the torrent client is on windows and TOFF is\n" +
                       "running on linux such that e.g. C:\\ gets converted to /c/ prior to applying any translations."
            };
            autoTranslateLabel.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(StandardColor.BrightRed, StandardColor.RaisinBlack)));

            infoPopup.Add(instructionLabel, autoTranslateLabel);
            infoPopup.AddButton(new Button() { Text = "OK" });


            _navigationService.RunDialog(infoPopup);

        }

    }
}
