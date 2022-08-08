using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class ParkPersistence
    {
        //private ParkFileDataAccess dataAccess;
        private string _parkName;
        private Field[,] _gameTable;
        private Dictionary<Entity,Point> _entities;
        private Dictionary<Point,IArea> _areas;
        private Dictionary<Staff,Point> _staffs;
        private bool _parkIsOpen;
        private int _time;
        private int _availableMoney;
        private int _entryFee;
        private int _satisfactionLevel;
        private int _rowSize;
        private int _colSize;
        private Point _startField;
        private List<Field> _noPath;

        public ParkPersistence(int rowSize, int colSize)
        {
            _rowSize = rowSize;
            _colSize = colSize;
            _gameTable = new Field[rowSize, colSize];
            _availableMoney = 10000;
            _parkIsOpen = false;
            _time = 0;
            _startField = new Point(rowSize-1, (int)((colSize-1) / 2));
            _entryFee = 25;
            _noPath = new List<Field>();

            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < colSize; j++)
                {
                    _gameTable[i, j] = new Field(i, j, new EmptyField(OtherType.NONE));
                }
            }
            _gameTable[_startField.X, _startField.Y] = new Field(_startField.X, _startField.Y, new Road(RoadType.SIDEWALK));

            _entities = new Dictionary<Entity, Point>();
            _areas = new Dictionary<Point, IArea>();
            _staffs = new Dictionary<Staff, Point>();
        }
        public bool ParkIsOpen { get => _parkIsOpen; set => _parkIsOpen = value; }
        public int Time { get => _time; set => _time = value; }
        public Field GetField(int row, int col) { return _gameTable[row, col]; }
        public Field[,] GetFields() { return _gameTable; }
        public IArea GetArea(int row, int col) { return _areas[new Point(row,col)]; } //Paraméter jó-e hogy "vektorként" tároljuk az areákat?
        public ICollection<IArea> GetAreas() { return _areas.Values; }
        public Dictionary<Point, IArea> Areas { get => _areas; set => _areas = value; }
        public Dictionary<Point, IArea> GetAreasWithCoordinates() { return _areas; }
        public Dictionary<Entity,Point> GetEntities() { return _entities; }
        public List<Field> NoPath { get => _noPath; }
        public Dictionary<Staff,Point> GetStaffs() { return _staffs; }
        public void SetEntryFee(int fee) { _entryFee = fee; }
        public int GetEntryFee() { return _entryFee; }
        public int RowSize { get => _rowSize; set => _rowSize = value; }
        public int ColSize { get => _colSize; set => _colSize = value; }
        public int AvailableMoney { get => _availableMoney; set => _availableMoney = value; }
        public int SatisfactionLevel { get => _satisfactionLevel; set => _satisfactionLevel = value; }
        public string ParkName { get => _parkName; set => _parkName = value; }
        public Point StartField { get => _startField; set => _startField = value; }

        public void AddEntity(int row, int col, Entity e)
        {
            _gameTable[row, col].Entities.Add(e); //field
            _entities.Add(e, new Point(row, col)); //persistence
        }

        public void RemoveEntity(int row, int col, Entity e)
        {
            _gameTable[row, col].Entities.Remove(e); //field
        }

        public void AddStaff(int row, int col, Staff s)
        {
            _gameTable[row, col].Staffs.Add(s); //field
            _staffs.Add(s, new Point(row, col)); //persistence
        }

        public void RemoveStaff(int row, int col, Staff s)
        {
            _gameTable[row, col].Staffs.Remove(s); //field
        }
    }
}
