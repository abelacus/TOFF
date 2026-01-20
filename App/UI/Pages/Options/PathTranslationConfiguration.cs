using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Services;

namespace TOFF.UI.Pages.Options
{
    internal class PathTranslationConfiguration : Window, IPage
    {
        private readonly NavigationService _navigationService;
        private readonly AppStateService _appState;

        private Dictionary<string, string> translations;

        public PathTranslationConfiguration(AppStateService appState, NavigationService navigationService)
        {
            _appState = appState;
            _navigationService = navigationService;

            translations = _appState.PathTranslations;

            ListView translationList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
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

            shortcutBar.Add(addNewShortcut, backShortcut, discardShortcut);
            shortcutBar.SetFocus();

            Add(divider, shortcutBar);

        }

        private void AddNewTranslation()
        {
            Dialog translationWizard = new Dialog()
            {
                Title = "Add Path Translation",
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

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
                Width = Dim.Percent(50),
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
                Width = Dim.Percent(50),
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

            clientPath.HasFocusChanged += (_, e) =>
            {
                if (e.NewFocused == clientPath)
                {
                    description.Text = "The path you want to translate as it appears in the torrent client. In a Docker Compose file, this would be the 'destination' of a volume mount";
                }
            };

            localPath.HasFocusChanged += (_, e) =>
            {
                if (e.NewFocused == localPath)
                {
                    description.Text = "The destination path that the path should be translated to. In a Docker Compose file, this would be the part 'source' of a volume mount";
                }
            };



            translationWizard.Add(description, clientPathLabel, clientPath, localPathLabel, localPath);
            translationWizard.AddButton(cancelButton);
            translationWizard.AddButton(submitButton);

            submitButton.Activating += (_, e) =>
            {
                if(clientPath.Text.Length == 0)
                {
                    e.Handled = false;
                    return;
                }

                if(translations.ContainsKey(clientPath.Text))
                { 
                    Dialog errorDialog = new Dialog()
                    {
                        Title = "Key already exists",
                        X = Pos.Center(),
                        Y = Pos.Center(),
                    };

                    errorDialog.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(Color.BrightRed, Color.Black)));

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

                    if(errorDialog.Canceled == true || errorDialog.Result == 0)
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
                translations[clientPath.Text] = localPath.Text;
            }

        }

        private void SaveAndBack()
        {
            //TODO: save changes to app state
            _appState.PathTranslations = translations;

            _navigationService.NavigateBack();
        }

        private void DiscardAndBack()
        {
            _navigationService.NavigateBack();
        }
    }
}
