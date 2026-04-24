using System.Collections.ObjectModel;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Models;
using TOFF.Services;
using TorrentClient.Models;

namespace TOFF.UI.Pages.Options
{
    internal class ClientConfiguration : Window, IPage
    {
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;

        private readonly ConfigItemValue[] _configItems;
        public ClientConfiguration(AppStateService appState, NavigationService navigationService)
        {
            _appState = appState;
            _navigationService = navigationService;

            _configItems =
            [
                new ConfigItemValue("Client API URL", _appState.Preferences.TorrentClientConfig.ApiUrl, null),
                new ConfigItemValue("Requires Authentication", _appState.Preferences.TorrentClientConfig.HasAuthentication.ToString(), new List<string>{"True", "False"}),
                new ConfigItemValue("Username", _appState.Preferences.TorrentClientConfig.Username ?? "", null),
                new ConfigItemValue("Password", _appState.Preferences.TorrentClientConfig.Password ?? "", null)
            ];
            
            Title = "Torrent Client Configuration";

            var configList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                ShowMarks = false,
                MarkMultiple = false,
            };

            configList.SetSource(new ObservableCollection<ConfigItemValue>(_configItems));

            configList.Accepting += (_, e) =>
            {
                if (configList.SelectedItem != null && configList.Source != null)
                {
                    ConfigItemValue entry = (ConfigItemValue)configList.Source.ToList()[(int)configList.SelectedItem]!;

                    string newVal = EditConfigItem(entry);

                    _configItems.First(i => i.Name == entry.Name).Value = newVal;

                }
                e.Handled = true;
            };

            Add(configList);

            Line divider = new Line()
            {
                X = 0,
                Y = Pos.Bottom(configList),
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

            shortcutBar.Add(backShortcut, discardShortcut);

            Add(divider, shortcutBar);            
        }

        private void SaveAndBack()
        {
            var errorDialog = new Dialog()
            {
                Title = "Set required Variables",
                X = Pos.Center(),
                Y = Pos.Center(),
            };

            errorDialog.SetScheme(SchemeManager.GetScheme(Schemes.Error));

            if (bool.Parse(_configItems[1].Value))
            {
                //create dialog if both aren't set, allow if at least one is.
                if (_configItems[2].Value.Length == 0 && _configItems[3].Value.Length == 0)
                {
                    var errorLabel = new Label()
                    {
                        Title = "A username or password must be provided if authentication is required",
                        X = Pos.Center(),
                        Y = Pos.Center(),
                        Height = 1,
                        CanFocus = false,
                    };
                    errorDialog.Add(errorLabel);
                    errorDialog.AddButton(new() { Title = "OK" });
                    _navigationService.RunDialog(errorDialog);
                    return;
                }
            }
            if (_configItems[0].Value.Length == 0)
            {
                var errorLabel = new Label()
                {
                    Title = "API URL cannot be empty.",
                    Height = 1,
                    X = 1,
                    Y = 1,
                };

                errorDialog.Add(errorLabel);
                errorDialog.AddButton(new() { Title = "OK" });
                errorLabel.VerticalScrollBar.Visible = false;
                _navigationService.RunDialog(errorDialog);
                return;
            }


            //update global state and navigate back
            _appState.Preferences.TorrentClientConfig = new TorrentClientConfig()
            {
                ApiUrl = _configItems[0].Value,
                HasAuthentication = bool.Parse(_configItems[1].Value),
                Username = _configItems[2].Value,
                Password = _configItems[3].Value,
            };

            _navigationService.NavigateBack();
        }

        private void DiscardAndBack()
        {
            if (_configItems[0].Value.Length == 0 && (_appState.Preferences.TorrentClientConfig.ApiUrl == null || _appState.Preferences.TorrentClientConfig.ApiUrl.Length == 0))//don't let them return if an api url hasn't been set
            {
                var errorDialog = new Dialog()
                {
                    Title = "Set required Variables",
                    X = Pos.Center(),
                    Y = Pos.Center(),
                };

                errorDialog.SetScheme(SchemeManager.GetScheme(Schemes.Error));

                var errorLabel = new Label()
                {
                    Title = "API URL cannot be empty.",
                    Height = 1,
                    X = 1,
                    Y = 1,
                };

                errorDialog.Add(errorLabel);
                errorDialog.AddButton(new() { Title = "OK" });
                errorLabel.VerticalScrollBar.Visible = false;
                _navigationService.RunDialog(errorDialog);
                return;
            }

            if(_appState.Preferences.TorrentClientConfig.ApiUrl == null || _appState.Preferences.TorrentClientConfig.ApiUrl.Length == 0) {
                _appState.Preferences.TorrentClientConfig.ApiUrl = _configItems[0].Value;
            }

            _navigationService.NavigateBack();
        }

        private string EditConfigItem(ConfigItemValue item)
        {
            var editDialogue = new Dialog()
            {
                Title = $"Editing {item.Name}",
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = 40,
            };

            editDialogue.Padding.Thickness = new Thickness(2, 1, 2, 1);

            var label = new Label()
            {
                Title = "New Value:",
                X = 0,
                Y = 0,
                Height = 1,
            };

            editDialogue.Add(label);
            string result = item.Value;

            ////replace this with selectables if presets exist
            if (item.Presets == null)
            {
                //TODO: figure out why a scrollbar shows to the right of the text field
                var textField = new TextField()
                {
                    Text = item.Value,
                    X = Pos.Right(label) + 1,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = 1,
                };

                textField.TextChanged += (s, e) =>
                {
                    result = textField.Text;
                };

                //required for save on enter
                textField.KeyDown += (_, e) =>
                {
                    if (e.KeyCode == Key.Enter)
                    {
                        editDialogue.Result = 1;
                    }
                };

                editDialogue.Add(textField);
            }
            else
            {
                var optionsSelector = new OptionSelector()
                {
                    X = 0,
                    Y = Pos.Bottom(label),
                    Height = 2,
                    Width = Dim.Fill(),
                };
                optionsSelector.Labels = item.Presets;

                optionsSelector.Value = item.Presets.IndexOf(item.Value);

                optionsSelector.ValueChanged += (_, e) =>
                {
                    result = item.Presets[(int)e.NewValue];
                };

                //required for save on enter
#pragma warning disable TGUI001 // Accepting event handler should set Handled = true. ignore because we still want dialog to close afterwards
                optionsSelector.Accepting += (_, e) =>
                {
                    editDialogue.Result = 1;
                };
#pragma warning restore TGUI001 // Accepting event handler should set Handled = true

                editDialogue.Add(optionsSelector);
            }


            editDialogue.AddButton(new() { Title = "Cancel", IsDefault = false });
            editDialogue.AddButton(new() { Title = "OK", IsDefault = true });


            _navigationService.RunDialog(editDialogue);

            if(editDialogue.Result == null)
            {
                if(editDialogue.Canceled)
                { 
                    return item.Value;
                }
                return result;
            }


            if(editDialogue.Result == 1)
            {
                return result;
            }

            return item.Value;

        }
    }
}
