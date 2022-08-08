using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class Staff : IArea
    {
        public const int CLEANER_PRICE = 200;
        public const int MAINTENANCE_PRICE = 300;

        //Maybe we can create a state enum for Staff, to check that a staff state, example: staff is free or repairing or cleaning at the time 
        private StaffState _state;
        private StaffType _type;
        private int _price;
        private Point _destination;
        private Game _gameToRepair;
        private Point _endOfAimRoad;
        private List<Field> _pathToAim;

        public Enum EnumType { get => _type; set => _type = (StaffType)value; }

        public Staff(StaffType type)
        {
            _type = type;
            _price = type == StaffType.CLEANER ? CLEANER_PRICE : MAINTENANCE_PRICE;
            _state = StaffState.NOTHING_TO_DO;
            Destination = new Point(-1, -1);
            EndOfRoad = new Point();
        }

        public Point EndOfRoad { get => _endOfAimRoad; set => _endOfAimRoad = value; }
        public List<Field> PathToAim { get => _pathToAim; set => _pathToAim = value; }
        public StaffType Type { get => _type; }
        public StaffState State { get => _state; set => _state = value; }
        public Point Destination { get => _destination; set => _destination = value; }
        public Game GameToRepair { get => _gameToRepair; set => _gameToRepair = value; }
        public int BuildPrice { get => _price; }

        public Point ChooseDirectionToGo(Field[,] gameTable, int gameTableSize, Field currentLocation)
        {
            Point cl = new Point(currentLocation.Row, currentLocation.Col);
            Field left = null;
            Field up = null;
            Field right = null;
            Field down = null;

            if (cl.Y > 0)
                left = gameTable[cl.X, cl.Y - 1];
            if (cl.Y < gameTableSize-1 )
                right = gameTable[cl.X, cl.Y + 1];
            if (cl.X > 0)
                up = gameTable[cl.X - 1, cl.Y];
            if (cl.X < gameTableSize - 1)
                down = gameTable[cl.X + 1, cl.Y];

            List<Field> possibleDirections = new List<Field>() { left, up, right, down };
            Field choosenDirection;
            List<Field> directions = new List<Field>(possibleDirections);
            foreach (var direction in directions)
            {
                if (direction == null || direction.Area.GetType() != typeof(Road))
                    possibleDirections.Remove(direction);
            }
            directions = new List<Field>(possibleDirections);

            Point difference = new Point();
            int rand = new Random().Next(possibleDirections.Count());
            choosenDirection = possibleDirections[rand];
            
            int xDif;
            int yDiff;

            if (currentLocation.Row - choosenDirection.Row == 0) xDif = 0;
            else if (currentLocation.Row - choosenDirection.Row == 1) xDif = -1;
            else xDif = 1;

            if (currentLocation.Col - choosenDirection.Col == 0) yDiff = 0;
            else if (currentLocation.Col - choosenDirection.Col == 1) yDiff = -1;
            else yDiff = 1;

            difference = new Point(xDif, yDiff);

            Field endOfRoad = choosenDirection;
            bool found = false;
            while(endOfRoad.Row + difference.X < gameTableSize && endOfRoad.Row + difference.X > -1 && endOfRoad.Col + difference.Y < gameTableSize && endOfRoad.Col + difference.Y > -1 && !found)
            {
                Field nextRoad = gameTable[endOfRoad.Row + difference.X, endOfRoad.Col + difference.Y];
                if (nextRoad.Area.GetType() == typeof(Road))
                    endOfRoad = nextRoad;
                else
                    found = true;
            }

            return new Point(endOfRoad.Row, endOfRoad.Col);
        }
    }
}
