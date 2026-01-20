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
            Wizard translationWizard = new Wizard()
            {
                Title = "Add Path Translation",
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            WizardStep firstStep = new WizardStep()
            {
                Title = "From Path:",
                NextButtonText = "Next",
                HelpText = "The path you want to translate as it appears in the torrent client. In a Docker Compose file, this would be the 'destination' of a volume mount",
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

            firstStep.Add(clientPathLabel, clientPath);

            translationWizard.AddStep(firstStep);

            WizardStep secondStep = new WizardStep()
            {
                Title = "To Path:",
                HelpText = "The destination path that the path should be translated to. In a Docker Compose file, this would be the part 'source' of a volume mount"
            };

            Label localPathLabel = new Label()
            {
                X = 0,
                Y = 0,
                Title = "To Path:"
            };

            TextField localPath = new TextField()
            {
                X = 0,
                Y = Pos.Bottom(localPathLabel),
                Width = Dim.Percent(50),
            };

            //disable submit on enter when text fields are selected
            //causes issues with duplicated accepting triggers if we don't
            localPath.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Key.Enter)
                {
                    e.Handled = true;
                }
            };

            secondStep.Add(localPathLabel, localPath);
            translationWizard.AddStep(secondStep);

            translationWizard.Accepted += (_, e) =>
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

                    int result = _navigationService.RunDialog(errorDialog);

                    if(errorDialog.Canceled == true || result == 0)
                    {
                        e.Handled = false;
                    }

                    if(result == 1)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
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
            _appState.PathTranslations = translations;

            _navigationService.NavigateBack();
        }

        private void DiscardAndBack()
        {
            _navigationService.NavigateBack();
        }
    }
}
