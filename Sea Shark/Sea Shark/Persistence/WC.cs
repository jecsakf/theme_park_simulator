using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class WC : IArea
    {
        #region Constants
        public const int WC_BUILDPRICE = 100;
        public const int WC_REGULARCOST = 25;
        public const int WC_PRICE = 2;
        #endregion

        private WCState _state;
        private OtherType _type;
        private int _buildPrice;
        private int _regularCost;
        private int _price;
        public Enum EnumType { get => _type; set => _type = (OtherType)value; }

        public WC()
        {
            // amíg zárva van a park, addig a state WAITING legyen a játék létrehozásakor, utána legyen BUILDING
            _state = WCState.WAITING;
            _buildPrice = WC_BUILDPRICE;
            _regularCost = WC_REGULARCOST;
            _price = WC_PRICE;
            _type = OtherType.WC;
        }
        public int Price { get => _price; set => _price = value; }
        public int CommonCost { get => _regularCost; set => _regularCost = value; }
        public int BuildPrice { get => _buildPrice; }
        public WCState State { get => _state; set => _state = value; }

    }
}
