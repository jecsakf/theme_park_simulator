using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class Road : IArea
    {
        private const int SIDEWALK_BUILDPRICE = 10;
        private const int BUBBLE_BUILDPRICE = 20;

        private RoadType _type;
        private int _trashAmount;
        private int _buildPrice;

        public Enum EnumType { get => _type; set => _type = (RoadType)value; }

        public Road(RoadType type)
        {
            _type = type;
            _trashAmount = 0;
            _buildPrice = _type == RoadType.SIDEWALK ? SIDEWALK_BUILDPRICE : BUBBLE_BUILDPRICE;
        }
        public RoadType Type { get => _type; }
        public int BuildPrice { get => _buildPrice; set => _buildPrice = value; }
        public int TrashAmount { get => _trashAmount; set => _trashAmount = value; }
        public int ReduceTrash(int e)
        {
            return _trashAmount - e;
        }
        public int AddTrash(int e)
        {
            return _trashAmount + e;
        }
    }
}
