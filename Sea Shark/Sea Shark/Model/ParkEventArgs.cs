using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Model
{
    public class ParkEventArgs
    {
        private int _gameTime;
        private bool _isGameOver;
        public int GameTime { get { return _gameTime; } }
        public bool IsGameOver { get { return _isGameOver; } }

        public ParkEventArgs(int gameTime, bool isGameOver)
        {
            _gameTime = gameTime;
            _isGameOver = isGameOver;
        }
    }
}
