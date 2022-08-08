using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class Field : SettlersEngine.IPathNode<Object>
    {
        private IArea _area;
        private List<Entity> _entities;
        private List<Staff> _staffs;
        private bool _hasPower;
        private int _row;
        private int _col;
        public Field(int row, int col, IArea area)
        {
            _row = row;
            _col = col;
            _area = area;
            _hasPower = false;
            _entities = new List<Entity>();
            _staffs = new List<Staff>();
        }

        public IArea Area { get => _area; set => _area = value; }
        public int Row { get => _row; set => _row = value; }
        public int Col { get => _col; set => _col = value; }
        public bool HasPower
        {
            get => _hasPower;
            set
            {
                if (_area != null && _area.GetType() == typeof(Game))
                {
                    Game game = (Game)_area;
                    game.IsUnderPower = value;
                }
                if (_area != null && _area.GetType() == typeof(Restaurant))
                {
                    Restaurant restaurant = (Restaurant)_area;
                    restaurant.IsUnderPower = value;
                }
                _hasPower = value;
            }
        }
        public List<Entity> Entities { get => _entities; }
        public List<Staff> Staffs { get => _staffs; }

        public void AddEntity(Entity e) { _entities.Add(e); }
        public void RemoveEntity(Entity e) { _entities.Remove(e); }
        public void AddStaff(Staff s) { _staffs.Add(s); }
        public void RemoveStaff(Staff s) { _staffs.Remove(s); }
        public bool IsWalkable(object inContext)
        {
            return Area.GetType() == typeof(Road);
        }
    }
}
