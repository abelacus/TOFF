using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Models;
using TOFF.Services;

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
                    EditConfigItem((ConfigItemValue)ev.Value);

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
                e.Handled = true;
                _navigationService.NavigateBack();
            };

            Add(configList, backButton);
        }

        private void EditConfigItem(ConfigItemValue item)
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
                Y = 1
            };

            editDialogue.Add(label);
            //string result = "";

            ////replace this with selectables if presets exist
            //if (item.Presets == null)
            //{
                var textField = new TextField()
                {
                    Text = item.Value,
                    X = Pos.Right(label) + 1,
                    Y = 1,
                    Width = Dim.Fill() - 1
                };

                //textField.TextChanged += (s, e) =>
                //{
                //    result = textField.Text.ToString();
                //};
                editDialogue.Add(textField);
            //}

            editDialogue.AddButton(new() { Title = "Cancel" });
            editDialogue.AddButton(new() { Title = "OK" });


            _navigationService.RunDialog(editDialogue);

        }
    }
}
