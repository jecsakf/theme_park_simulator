using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class Plant : IArea
    {
        #region Constants
        public const int SEAWEED_BUILDPRICE = 100;
        public const int CORAL_BUILDPRICE = 200;
        public const int SEAWEED_RANGE = 2;
        public const int CORAL_RANGE = 4;
        #endregion

        private PlantType _type;
        private int _range;
        private int _buildPrice; //buildPrice instead of repairPrice

        public Enum EnumType { get => _type; set => _type = (PlantType)value; }

        public Plant(PlantType type)
        {
            _type = type;
            switch (_type)
            {
                case PlantType.SEAWEED:
                    _range = SEAWEED_RANGE;
                    _buildPrice = SEAWEED_BUILDPRICE;
                    break;
                case PlantType.CORAL:
                    _range = CORAL_RANGE;
                    _buildPrice = CORAL_BUILDPRICE;
                    break;
            }
        }

        public PlantType Type { get => _type; }
        public int BuildPrice { get => _buildPrice; }
        public int Range { get => _range; }
    }
}
