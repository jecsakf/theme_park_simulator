using Sea_Shark.Model;
using Sea_Shark.View;
using Sea_Shark.ViewModel;
using Sea_Shark.Persistence;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;

namespace Sea_Shark
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DispatcherTimer _timer;
        private ParkModel _model;
        private ParkFileDataAccess _dataAccess;
        private MainWindow _mainWindow;
        private MainViewModel _mainViewModel;
        private NewGameViewModel _newGameViewModel;
        private NewGameWindow _newGameWindow;
        public App()
        {
            NewGameWindow_Show();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += new EventHandler(TimerTick);

        }

        private void NewGameWindow_Show()
        {
            _newGameViewModel = new NewGameViewModel();
            _newGameViewModel.NewGameCreated += NewGameViewModel_NewGameCreated;
            _newGameViewModel.TryLoadingGames += NewGameViewModel_TryLoadingGames;
            _newGameWindow = new NewGameWindow();
            _newGameWindow.DataContext = _newGameViewModel;
            _newGameWindow.Show();
        }

        private async void NewGameViewModel_TryLoadingGames(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                _dataAccess = new ParkFileDataAccess();
                _model = new ParkModel(1, 1, _dataAccess);
                await _model.LoadGameAsync(ofd.FileName);
                _mainViewModel = new MainViewModel(_model);
                _mainWindow = new MainWindow();
                _mainWindow.DataContext = _mainViewModel;
                _mainViewModel.LoadGame += new EventHandler(LoadGame);
                _mainViewModel.SaveGame += new EventHandler(SaveGame);
                _mainViewModel.NewGame += new EventHandler(NewGame);
                _mainViewModel.GameSpeedUp += new EventHandler(GameSpeedUp);
                _mainViewModel.GameSpeedDown += new EventHandler(GameSpeedDown);
                _mainViewModel.StartCampaign += new EventHandler(StartCampaign);

                _mainWindow.Show();
                _timer.Start();
                _newGameWindow.Close();
            }
        }

        private void NewGameViewModel_NewGameCreated(object sender, NewGameEventArgs e)
        {
            _dataAccess = new ParkFileDataAccess();
            _model = new ParkModel((int)e.TableSize,(int)e.TableSize,_dataAccess);
            _mainViewModel = new MainViewModel(_model);
            _mainViewModel.ParkName = e.ParkName;
            _mainViewModel.RowCount = (int)e.TableSize;
            _mainViewModel.ColumnCount = (int)e.TableSize;
            _mainViewModel.ParkIsOpened += StartTimer;

            _mainWindow = new MainWindow();
            _mainWindow.DataContext = _mainViewModel;
            _mainViewModel.LoadGame += new EventHandler(LoadGame);
            _mainViewModel.SaveGame += new EventHandler(SaveGame);
            _mainViewModel.NewGame += new EventHandler(NewGame);
            _mainViewModel.GameSpeedUp += new EventHandler(GameSpeedUp);
            _mainViewModel.GameSpeedDown += new EventHandler(GameSpeedDown);
            _mainViewModel.StartCampaign += new EventHandler(StartCampaign);
            _mainViewModel.GameEnd += new EventHandler(GameOver);
            _mainWindow.Show();
            _timer.Start();
            _newGameWindow.Close();
        }

        private void GameOver(object sender, EventArgs e)
        {
            _timer.Stop();
            MessageBox.Show("Game over!" + Environment.NewLine +
                                "Sorry, you're broke ",
                                "SeaSharkPark",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
        }

        private async void LoadGame(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                await _model.LoadGameAsync(ofd.FileName);
                _mainViewModel = new MainViewModel(_model);
                _timer.Start();
                _mainWindow.DataContext = _mainViewModel;
                _mainViewModel.LoadGame += new EventHandler(LoadGame);
                _mainViewModel.SaveGame += new EventHandler(SaveGame);
                _mainViewModel.NewGame += new EventHandler(NewGame);
                _mainViewModel.GameSpeedUp += new EventHandler(GameSpeedUp);
                _mainViewModel.GameSpeedDown += new EventHandler(GameSpeedDown);
                _mainViewModel.StartCampaign += new EventHandler(StartCampaign);
                _mainViewModel.RefreshTable();
            }


        }
        private async void SaveGame(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == true)
            {
                await _model.SaveGameAsync(sfd.FileName);
            }
        }

        private void NewGame(object sender, EventArgs e)
        {
            NewGameWindow_Show();
            _mainWindow.Close();
        }

        private void StartTimer(object sender, EventArgs e)
        {          
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _model.OnTick();
        }

        private void GameSpeedUp(object sender, EventArgs e)
        {
            if (_timer != null)
            {
                switch (_model.TickRate)
                {
                    case 1000:
                        _model.TickRate = 500;
                        _timer.Interval = TimeSpan.FromMilliseconds(_model.TickRate);
                        break;
                    case 2000:
                        _model.TickRate = 1000;
                        _timer.Interval = TimeSpan.FromMilliseconds(_model.TickRate);
                        break;
                }
            }
        }

        private void GameSpeedDown(object sender, EventArgs e)
        {
            if (_timer != null)
            {
                switch (_model.TickRate)
                {
                    case 1000:
                        _model.TickRate = 2000;
                        _timer.Interval = TimeSpan.FromMilliseconds(_model.TickRate);
                        break;
                    case 500:
                        _model.TickRate = 1000;
                        _timer.Interval = TimeSpan.FromMilliseconds(_model.TickRate);
                        break;
                }
            }
        }
        private void StartCampaign(object sender, EventArgs e)
        {
            _model.StartCampaign();
        }
    }
}
