using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class Bin : IArea
    {
        #region Constants
        public const int BIN_BUILDPRICE = 50;
        #endregion

        private OtherType _type;
        private int _buildPrice;

        public Enum EnumType { get => _type; set => _type = (OtherType)value; }

        public Bin()
        {
            _buildPrice = BIN_BUILDPRICE;
            _type = OtherType.BIN;
        }

        public int BuildPrice { get => _buildPrice;}
    }
}
