using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using TOFF.Models;
using TOFF.Services;
using TorrentClient;
using TorrentClient.Models;

namespace TOFF.UI.Pages
{
    /// <summary>
    /// Displays a loading bar whilst we query the torrent client for the available torrents and compare with files in the <see cref="Services.AppStateService.torrentDirectory"/>
    /// </summary>
    internal class SearchPage : Window, IPage
    {
        private readonly AppStateService _appState;
        private readonly NavigationService _navigationService;
        private readonly TorrentClientService _clientService;

        private ProgressBar progressBar;
        private Label statusLabel;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public SearchPage(AppStateService appstate, NavigationService navigationService, TorrentClientService torrentClientService)
        {
            _appState = appstate;
            _navigationService = navigationService;
            _clientService = torrentClientService;

            progressBar = new ProgressBar()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = Dim.Percent(60),
                ProgressBarStyle = ProgressBarStyle.Continuous,
                ProgressBarFormat = ProgressBarFormat.Simple,
                Fraction = 0f,
            };

            statusLabel = new Label()
            {
                X = Pos.Left(progressBar),
                Y = Pos.Top(progressBar) - 2,
                Text = "Logging in to client...",
            };

            Add(statusLabel, progressBar);

            Shortcut backShortcut = new Shortcut()
            {
                Y = Pos.AnchorEnd(),
                Key = Key.Esc,
                Action = () => { _cts.Cancel(); _navigationService.NavigateBack(); }
            };

            Add(backShortcut);

            //Do this in a seperate thread because it might take a while
            Task.Run(() =>
            {
                try
                {
                    if (CreateAndConnectTorrentClient(_cts.Token))
                    {
                        GetAndProcessClientData(_cts.Token);
                    }
                }
                catch(OperationCanceledException e)
                {
                    Debug.WriteLine("Cancelled");
                }
            }, _cts.Token);


        }

        private bool CreateAndConnectTorrentClient(CancellationToken token)
        {
            try
            {
                _appState.torrentClient = _clientService.CreateClientInstance(_appState.clientSelection, _appState.torrentClientConfig);
            
                _appState.torrentClient.ConnectToClient().Wait();
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);

                //likely unable to connect
                Dialog errorDialog = new Dialog()
                {
                    Title = "Unable to connect to client",
                    X = Pos.Center(),
                    Y = Pos.Center(),
                };

                errorDialog.SetScheme(new Scheme(new Terminal.Gui.Drawing.Attribute(Color.BrightRed, Color.Black)));

                //validate current settings
                Label errorLabel = new Label()
                {
                    Text = "An error occured while connecting to torrent client\nCheck login details and url and try again",
                    X = Pos.Center() + 1,
                    Y = 1,
                    Height = 2,
                    CanFocus = false,
                };

                errorDialog.Add(errorLabel);
                errorDialog.AddButton(new() { Title = "Ok" });

                errorDialog.Buttons[0].Activating += (_, e) =>
                {
                    _navigationService.NavigateBack();
                };

                if (!token.IsCancellationRequested)
                {
                    _navigationService.RunDialog(errorDialog);
                }

                return false;
            }

        }


        private void GetAndProcessClientData(CancellationToken token)
        {
            //get number of files/directories in torrentDirectory
            int estimatedDirectoryItemCount = Directory.GetFiles(_appState.torrentDirectory!).Length + Directory.GetDirectories(_appState.torrentDirectory!).Length;

            //get number of torrents in Client
            TorrentDetails[] details = _appState.torrentClient.GetTorrentDetails().Result;

            List<FileDetails> allTorrentFiles = new List<FileDetails>();

            App.Invoke(() =>
            {
                progressBar.Fraction = 0.1f;
                statusLabel.Text = "Getting files in torrents...";
            });

            //request data from torrents
            foreach (var needed in details)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (_appState.IgnoreDirectories.Any(e => e == needed.SavePath))
                {
                    continue;
                }

                //migrate the base path to match the current OS form
                //required because e.g. if the torrent client is on windows but the application is on linux, Path.Join will return the working directory + basePath.
                needed.SavePath = ConvertPathToLocalForm(needed.SavePath);

                FileDetails[] fileDetails = _appState.torrentClient.GetFilesForTorrent(needed).Result;

                foreach (FileDetails file in fileDetails)
                {
                    //priority 0 is 'do not download' so we should be good to ignore them.
                    if (file.priority != 0)
                    {
                        file.savePath = TranslatePath(file.savePath);
                        allTorrentFiles.Add(file);
                    }
                }

                App.Invoke(() =>
                {
                    progressBar.Fraction += 1f / details.Length / 2.5f; //2.5 because we want it to be 40% of the bar
                });
            }

            App.Invoke(() =>
            {
                statusLabel.Text = "Getting details of missing files...";
            });

            //walk through torrentDirectory
            var missing = from f in Directory.EnumerateFiles(_appState.torrentDirectory!, "*", SearchOption.AllDirectories)
                          where !allTorrentFiles.Any(e => e.qualifiedPath == f)
                          select f;
      
            List<FileInformation> missingInformation = new List<FileInformation>();
            foreach (var item in missing)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                missingInformation.Add(FileInfoService.GetFileInfo(item));

                App.Invoke(() =>
                {
                    progressBar.Fraction += 1f / missing.Count() / 2.5f;
                });
            }

            _appState.filesMissingFromClient = missingInformation.ToArray();

            //once done, display data in a table.
            if (!token.IsCancellationRequested)
            {
                _navigationService.NavigateTo(typeof(ResultsTablePage), false);
            }
        }

        private string TranslatePath(string path)
        {
            foreach (var item in _appState.PathTranslations)
            {
                if (path.StartsWith(item.Key))
                {
                    path = path.Replace(item.Key, item.Value);
                    break; //not sure why there'd be any case where someone would want to translate an already translated value
                }
            }

            return path;
        }

        private string ConvertPathToLocalForm(string path)
        {
            bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (IsWindows)
            {
                //TODO. need to decide how to handle these. will likely be considerably more difficult to do
                return path;
            }
            else //linux/MacOS. may need additional check, not sure if they behave the same.
            {
                if (path.Contains(@"\")) //double slashes should mean windows url
                {
                    string replaced = path.Replace(@"\", "/");
                    replaced = replaced.Replace(":", "");
                    replaced = replaced.Insert(0, "/");

                    return replaced;
                }
            }

            return path;
        }

    }
}
