using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sea_Shark.ViewModel
{
    public class NewGameEventArgs : EventArgs
    {
        private int _tableSize;
        private string _parkName;
        public int TableSize { get => _tableSize; set => _tableSize = value; }
        public string ParkName { get => _parkName; set => _parkName = value; }
        public NewGameEventArgs(string parkname, int tablesize)
        {
            _tableSize = tablesize;
            _parkName = parkname;
        }
    }
}
