using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
                CollectionNavigator = null,
            };
            table.Style.AlwaysShowHeaders = true;

            //TODO: revisit when/if the Terminal.Gui TableView gains the ability to visually discern between toggled entries and the one that's been selected with the arrow keys
            //Scheme scheme = new Scheme(SchemeManager.GetScheme("Base"))
            //{
            //    Focus = new Terminal.Gui.Drawing.Attribute(Color.Red, Color.Magenta),
            //    Active = new Terminal.Gui.Drawing.Attribute(Color.Green, Color.BrightYellow),
            //    Disabled = new Terminal.Gui.Drawing.Attribute(Color.Blue, Color.White),
            //};

            table.Style.ColumnStyles[0] = new ColumnStyle
            {
                RepresentationGetter = (value) => value.ToString().Remove(0, _appState.torrentDirectory.Length),
                //ColorGetter = (args) =>
                //{
                //    return scheme;
                //},
                Alignment = Alignment.Fill,
            };

            table.Table = new EnumerableTableSource<FileInformation>(_appState.filesMissingFromClient,
                    new Dictionary<string, Func<FileInformation, object>>()
                    {
                        { "savePath", (p) => p.savePath },
                        { "links", (p) => p.links },
                        { "creationDate", (p) => p.creationDate.ToShortDateString() },
                        { "lastModifiedDate", (p) => p.lastModifiedDate.ToShortDateString() },
                    }
                );
            //TODO: fix ctrl+a selection not working


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
                Key = Key.D.WithCtrl.WithShift.WithAlt,
                Text = "Delete Selected",
            };

            shortcutBar.Add(backShortcut, deleteSelectedShortcut);

            Add(divider, shortcutBar);
        }

        private void StartRemoveJob()
        {
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

            _appState.toBeDeleted = indexes.Select(i => _appState.filesMissingFromClient[i]).ToArray();

            _navigationService.NavigateTo<DeletionPage>(false);
        }
    }
}
