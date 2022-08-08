using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class PowerSource : IArea
    {
        #region Constants
        public const int POWERSOURCE_BUILDPRICE = 200;
        public const int POWERSOURCE_RANGE = 3;
        #endregion

        private OtherType _type;
        private int _buildPrice;
        private int _range;
        public Enum EnumType { get => _type; set => _type = (OtherType)value; }
        public PowerSource()
        {
            _buildPrice = POWERSOURCE_BUILDPRICE;
            _range = POWERSOURCE_RANGE;
            _type = OtherType.POWERSOURCE;
        }
        public int BuildPrice { get => _buildPrice; } 
        public int Range { get => _range; } 
    }
}
