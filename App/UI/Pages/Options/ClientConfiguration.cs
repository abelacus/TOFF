using System.Collections.ObjectModel;
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
        public ClientConfiguration(AppStateService appState, NavigationService navigationService)
        {
            _appState = appState;
            _navigationService = navigationService;

            ConfigItemValue[] configItems = new[] {
                new ConfigItemValue("Client API URL", _appState.torrentClientConfig.ApiURL ?? "", null),
                new ConfigItemValue("Requires Authentication", _appState.torrentClientConfig.HasAuthentication.ToString(), new List<string>{"True", "False"}),
                new ConfigItemValue("Username", _appState.torrentClientConfig.Username ?? "", null),
                new ConfigItemValue("Password", _appState.torrentClientConfig.Password ?? "", null),
            };

            ConfigItemValue? currentSelection = null;

            Title = "Torrent Client Configuration";

            var configList = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
                AllowsMarking = false,
                AllowsMultipleSelection = false,
            };

            configList.SetSource(new ObservableCollection<ConfigItemValue>(configItems));

            configList.OpenSelectedItem += (_, ev) =>
            {
                if (ev.Value != null)
                {
                    ConfigItemValue entry = (ConfigItemValue)ev.Value;

                    string newVal = EditConfigItem((ConfigItemValue)ev.Value);

                    configItems.First(e => e.Name == entry.Name).Value = newVal;

                }
            };

            var backButton = new Button()
            {
                Title = "Back",
                X = 1,
                Y = Pos.Bottom(configList),
            };



            backButton.Activating += (_, e) =>
            {
                var errorDialog = new Dialog()
                {
                    Title = "Set required Variables",
                    X = Pos.Center(),
                    Y = Pos.Center(),
                };

                errorDialog.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(Color.BrightRed, Color.Black)));

                if (bool.Parse(configItems[1].Value))
                {
                    //create dialog saying to set unset variables
                    if (configItems[0].Value.Length == 0 || configItems[2].Value.Length == 0 || configItems[3].Value.Length == 0)
                    {
                        var errorLabel = new Label()
                        {
                            Title = "All fields must be set when authentication is required.",
                            X = Pos.Center(),
                            Y = Pos.Center(),
                            Height = 1,
                            CanFocus = false,
                        };
                        errorDialog.Add(errorLabel);
                        errorDialog.AddButton(new() { Title = "OK" });
                        _navigationService.RunDialog(errorDialog);
                    }

                    e.Handled = true;
                    return;
                }
                if (configItems[0].Value.Length == 0)
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
                    e.Handled = true;
                    return;
                }


                //update global state and navigate back
                _appState.torrentClientConfig = new TorrentClientConfig()
                {
                    ApiURL = configItems[0].Value,
                    HasAuthentication = bool.Parse(configItems[1].Value),
                    Username = configItems[2].Value,
                    Password = configItems[3].Value,
                };

                e.Handled = true;
                _navigationService.NavigateBack();
            };

            Add(configList, backButton);
        }

        private string EditConfigItem(ConfigItemValue item)
        {
            var editDialogue = new Dialog()
            {
                Title = $"Editing {item.Name}",
                X = Pos.Center(),
                Y = Pos.Center(),
            };

            var label = new Label()
            {
                Title = "New Value:",
                X = 1,
                Y = 0
            };

            editDialogue.Add(label);
            string result = item.Value;

            ////replace this with selectables if presets exist
            if (item.Presets == null)
            {
                var textField = new TextField()
                {
                    Text = item.Value,
                    X = Pos.Right(label) + 1,
                    Y = 1,
                    Width = Dim.Fill() - 1
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

                optionsSelector.ValueChanged += (_, ev) =>
                {
                    result = item.Presets[(int)ev.Value];
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
                if(editDialogue.Canceled == true)
                { 
                    return item.Value;
                }
                if(editDialogue.Canceled == false)
                {
                    return result;
                }
            }


            if(editDialogue.Result == 1)
            {
                return result;
            }

            return item.Value;

        }
    }
}
