using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class Restaurant : IArea
    {
        #region Constants
        public const int FLAMING_CAPACITY = 8;
        public const int HOT_DOG_STAND_CAPACITY = 6;
        public const int ICE_CREAM_SHOP_CAPACITY = 5;
        public const int FLAMING_PRICE = 15;
        public const int HOT_DOG_STAND_PRICE = 10;
        public const int ICE_CREAM_SHOP_PRICE = 5;
        public const int FLAMING_REGULARCOST = 40;
        public const int HOT_DOG_STAND_REGULARCOST = 30;
        public const int ICE_CREAM_SHOP_REGULARCOST = 25;
        public const int FLAMING_OPERATINGCOST = 7;
        public const int HOT_DOG_OPERATINGCOST = 5;
        public const int ICE_CREAM_SHOP_OPERATINGCOST = 2;
        public const int FLAMING_BUILDPRICE = 250;
        public const int HOT_DOG_STAND_BUILDPRICE = 200;
        public const int ICE_CREAM_SHOP_BUILDPRICE = 150;
        public const int FLAMING_BUILDTIME = 15;
        public const int HOT_DOG_STAND_BUILDTIME = 10;
        public const int ICE_CREAM_SHOP_BUILDTIME = 10;
        public const int FLAMING_SERVICETIME = 6;
        public const int HOT_DOG_STAND_SERVICETIME = 8;
        public const int ICE_CREAM_SHOP_SERVICETIME = 2;
        public const int FLAMING_WELLBEING= 40;
        public const int HOT_DOG_STAND_WELLBEING = 45;
        public const int ICE_CREAM_SHOP_WELLBEING = 30;
        #endregion

        private RestaurantType _type;
        private RestaurantState _state;
        private Queue<Entity> _queueOut;
        private List<Entity> _entityIn;
        private int _capacity;
        private int _price;
        private int _regularCost;
        private int _operatingCost;
        private int _buildPrice;
        private int _buildTime;
        private int _serviceTime;
        private int _hungerReductionLevel;
        private int _stateTime;
        private bool _isUnderPower;

        public Enum EnumType { get => _type; set => _type = (RestaurantType)value; }
        public RestaurantState State { get => _state; set => _state = value; }
        public int StateTime { get => _stateTime; set => _stateTime = value; }
        public int Price { get => _price; set => _price = value; }
        public int CommonCost { get => _regularCost; set => _regularCost = value; }
        public int OperatingCost { get => _operatingCost; set => _operatingCost = value; }
        public int BuildPrice { get => _buildPrice; }
        public int BuildingTime { get => _buildTime; }
        public int Capacity { get => _capacity; }
        public int ServingTime { get => _serviceTime; }
        public int ReductionLevel { get => _hungerReductionLevel; }
        public Queue<Entity> CustomersInQueue { get => _queueOut; set => _queueOut = value; }
        public int CustomersInQueueAmount { get => CustomersInQueue.Count; }
        public List<Entity> CustomersInRestaurant { get => _entityIn; set => _entityIn = value; }
        public int CustomersInRestaurantAmount { get => CustomersInRestaurant.Count; }
        public bool IsUnderPower
        {
            get => _isUnderPower;
            set
            {
                _isUnderPower = value;
            }
        }

        public Restaurant(RestaurantType type)
        {
            _type = type;
            _state = RestaurantState.BUILDING;
            _isUnderPower = false;
            _queueOut = new Queue<Entity>();
            _entityIn = new List<Entity>();
            switch (type)
            {
                case RestaurantType.FLAMING:
                    _capacity = FLAMING_CAPACITY;
                    _price = FLAMING_PRICE;
                    _regularCost = FLAMING_REGULARCOST;
                    _operatingCost = FLAMING_OPERATINGCOST;
                    _buildPrice = FLAMING_BUILDPRICE;
                    _buildTime = FLAMING_BUILDTIME;
                    _serviceTime = FLAMING_SERVICETIME;
                    _hungerReductionLevel = FLAMING_WELLBEING;
                    break;
                case RestaurantType.HOT_DOG_STAND:
                    _capacity = HOT_DOG_STAND_CAPACITY;
                    _price = HOT_DOG_STAND_PRICE;
                    _regularCost = HOT_DOG_STAND_REGULARCOST;
                    _operatingCost = HOT_DOG_OPERATINGCOST;
                    _buildPrice = HOT_DOG_STAND_BUILDPRICE;
                    _buildTime = HOT_DOG_STAND_BUILDTIME;
                    _serviceTime = HOT_DOG_STAND_SERVICETIME;
                    _hungerReductionLevel = HOT_DOG_STAND_WELLBEING;
                    break;
                case RestaurantType.ICE_CREAM_SHOP:
                    _capacity = ICE_CREAM_SHOP_CAPACITY;
                    _price = ICE_CREAM_SHOP_PRICE;
                    _regularCost = ICE_CREAM_SHOP_REGULARCOST;
                    _operatingCost = ICE_CREAM_SHOP_OPERATINGCOST;
                    _buildPrice = ICE_CREAM_SHOP_BUILDPRICE;
                    _buildTime = ICE_CREAM_SHOP_BUILDTIME;
                    _serviceTime = ICE_CREAM_SHOP_SERVICETIME;
                    _hungerReductionLevel = ICE_CREAM_SHOP_WELLBEING;
                    break;
            }
            _stateTime = _buildTime;
        }

        public RestaurantType Type { get => _type; }

        private void ChangePrice(int newValue) => _price = newValue;
        private void ChangeState(RestaurantState newState) => _state = newState;

        // Instead of changeQueueOut() and changeEntityIn()
        // When serviceTime passes, an event invited and remove visitor from entityIn (then call this method)
        private void ChangeRestaurantVisitors(Entity visitor)
        {
            if (_state == RestaurantState.BUILDING || _state == RestaurantState.WAITING)
            {
                _entityIn.Add(visitor);
                ChangeState(RestaurantState.WORKING);
            }
            else if (_state == RestaurantState.WORKING)
            {
                if (_entityIn.Count < _capacity)
                    if (_queueOut.Count != 0)
                    {
                        _entityIn.Add(_queueOut.Dequeue());
                        _queueOut.Enqueue(visitor);
                    }
                    else
                        _entityIn.Add(visitor);
                else
                {
                    _queueOut.Enqueue(visitor);
                }
            }
        }
        private void RefreshQueue()
        {
            Queue<Entity> tmp = new Queue<Entity>();
            foreach (var item in CustomersInQueue)
            {
                if (item.State != EntityState.ON_THE_WAY)
                {
                    tmp.Enqueue(item);
                }
            }
            CustomersInQueue = tmp;
        }

        public bool EntityInQueue()
        {
            return CustomersInQueue.Count >= 1;
        }
        public void StartServing()
        {
            RefreshQueue();
            FillEmptySeats();
            foreach (var item in CustomersInRestaurant)
            {
                item.State = EntityState.DO_SOMETHING;
            }
            _stateTime = _serviceTime;
        }
        public void FillEmptySeats()
        {
            int i = CustomersInRestaurant.Count();
            while (CustomersInQueue.Count > 0 && i < _capacity)
            {
                CustomersInRestaurant.Add(CustomersInQueue.Dequeue());
                i++;
            }
        }
        public List<Entity> EmptyInRestaurantEntities()
        {
            List<Entity> entities = new List<Entity>(CustomersInRestaurant);
            CustomersInRestaurant.Clear();
            return entities;
        }
    }
}
