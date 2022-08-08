using Sea_shark.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sea_Shark.ViewModel
{
    public class NewGameViewModel : ViewModelBase
    {
        private static int _parksCount = 1;
        private int _currentTableSize;
        public String ParkName { get; set; }
        public ObservableCollection<int> TableSizes { get; set; }
        public int CurrentTableSize { get => _currentTableSize; set { _currentTableSize = value; OnPropertyChanged(); } }
        public DelegateCommand NewGameCommand { get; set; }
        public DelegateCommand LoadGamesCommand { get; set; }
        public event EventHandler<NewGameEventArgs> NewGameCreated;
        public event EventHandler TryLoadingGames;
        public NewGameViewModel()
        {
            ParkName = "SeaSharkPark"+_parksCount.ToString();
            _parksCount++;
            TableSizes = new ObservableCollection<int>() { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
            CurrentTableSize = TableSizes.ElementAt(4);
            NewGameCommand = new DelegateCommand(_ => NewGameCreated(this, new NewGameEventArgs(ParkName,CurrentTableSize)));
            LoadGamesCommand = new DelegateCommand(_ => TryLoadingGames(this, EventArgs.Empty));
        }
    }
}
