using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public class Game : IArea
    {
        #region Constants
        public const int DODGEM_CAPACITY = 6;
        public const int ROLLER_COASTER_CAPACITY = 10;
        public const int BUNGEE_JUMPING_CAPACITY = 3;
        public const int DODGEM_MINCAPACITY = 3;
        public const int ROLLER_COASTER_MINCAPACITY = 4;
        public const int BUNGEE_JUMPING_MINCAPACITY = 2;
        public const int DODGEM_PRICE = 10;
        public const int ROLLER_COASTER_PRICE = 15;
        public const int BUNGEE_JUMPING_PRICE = 20;
        public const int DODGEM_REGULARCOST = 50;
        public const int ROLLER_COASTER_REGULARCOST = 80;
        public const int BUNGEE_JUMPING_REGULARCOST = 100;
        public const int DODGEM_OPERATINGCOST = 15;
        public const int ROLLER_COASTER_OPERATINGCOST = 30;
        public const int BUNGEE_JUMPING_OPERATINGCOST = 20;
        public const int DODGEM_BUILDPRICE = 350;
        public const int ROLLER_COASTER_BUILDPRICE = 450;
        public const int BUNGEE_JUMPING_BUILDPRICE = 550;
        public const int DODGEM_BUILDTIME = 20;
        public const int ROLLER_COASTER_BUILDTIME = 25;
        public const int BUNGEE_JUMPING_BUILDTIME = 35;
        public const int DODGEM_GAMETIME = 20;
        public const int ROLLER_COASTER_GAMETIME = 25;
        public const int BUNGEE_JUMPING_GAMETIME = 35;
        public const int DODGEM_REPAIRTIME = 15;
        public const int ROLLER_COASTER_REPAIRTIME = 20;
        public const int BUNGEE_JUMPING_REPAIRTIME = 25;
        public const int DODGEM_HAPPINESS = 60;
        public const int ROLLER_COASTER_HAPPINESS = 65;
        public const int BUNGEE_JUMPING_HAPPINESS = 70;
        public const int DODGEM_ADRENALIN = 15;
        public const int ROLLER_COASTER_ADRENALIN = 40;
        public const int BUNGEE_JUMPING_ADRENALIN = 70;
        #endregion

        private GameType _type;
        private GameState _state;
        private Queue<Entity> _queueOut;
        private List<Entity> _entityIn;
        private int _capacity;
        private int _minNumOfPeople;
        private int _price;
        private int _regularCost;
        private int _operatingCost;
        private int _buildPrice;
        private int _buildTime;
        private int _gameTime;
        private int _repairTime;
        private int _happinessIncrementLevel;
        private int _adrenalin;
        private int _stateTime;
        private bool _maintenanceGoToRepair;
        private bool _isUnderPower;

        public Enum EnumType { get => _type; set => _type = (GameType)value; }

        public Game(GameType type)
        {
            _type = type;
            _state = GameState.BUILDING;
            _isUnderPower = false;
            _queueOut = new Queue<Entity>();
            _entityIn = new List<Entity>();
            switch (type)
            {
                case GameType.DODGEM:
                    _capacity = DODGEM_CAPACITY;
                    _minNumOfPeople = DODGEM_MINCAPACITY;
                    _price = DODGEM_PRICE;
                    _regularCost = DODGEM_REGULARCOST;
                    _operatingCost = DODGEM_OPERATINGCOST;
                    _buildPrice = DODGEM_BUILDPRICE;
                    _buildTime = DODGEM_BUILDTIME;
                    _gameTime = DODGEM_GAMETIME;
                    _repairTime = DODGEM_REPAIRTIME;
                    _happinessIncrementLevel = DODGEM_HAPPINESS;
                    _adrenalin = DODGEM_ADRENALIN;
                    break;
                case GameType.ROLLER_COASTER:
                    _capacity = ROLLER_COASTER_CAPACITY;
                    _minNumOfPeople = ROLLER_COASTER_MINCAPACITY;
                    _price = ROLLER_COASTER_PRICE;
                    _regularCost = ROLLER_COASTER_REGULARCOST;
                    _operatingCost = ROLLER_COASTER_OPERATINGCOST;
                    _buildPrice = ROLLER_COASTER_BUILDPRICE;
                    _buildTime = ROLLER_COASTER_BUILDTIME;
                    _gameTime = ROLLER_COASTER_GAMETIME;
                    _repairTime = ROLLER_COASTER_REPAIRTIME;
                    _happinessIncrementLevel = ROLLER_COASTER_HAPPINESS;
                    _adrenalin = ROLLER_COASTER_ADRENALIN;
                    break;
                case GameType.BUNGEE_JUMPING:
                    _capacity = BUNGEE_JUMPING_CAPACITY;
                    _minNumOfPeople = BUNGEE_JUMPING_MINCAPACITY;
                    _price = BUNGEE_JUMPING_PRICE;
                    _regularCost = BUNGEE_JUMPING_REGULARCOST;
                    _operatingCost = BUNGEE_JUMPING_OPERATINGCOST;
                    _buildPrice = BUNGEE_JUMPING_BUILDPRICE;
                    _buildTime = BUNGEE_JUMPING_BUILDTIME;
                    _gameTime = BUNGEE_JUMPING_GAMETIME;
                    _repairTime = BUNGEE_JUMPING_REPAIRTIME;
                    _happinessIncrementLevel = BUNGEE_JUMPING_HAPPINESS;
                    _adrenalin = BUNGEE_JUMPING_ADRENALIN;
                    break;
            }
            _stateTime = _buildTime;
        }

        public GameType Type { get => _type; }
        public GameState State { get => _state; set => _state = value; }
        public int StateTime { get => _stateTime; set => _stateTime = value; }
        public bool MaintenanceGoToRepair { get => _maintenanceGoToRepair; set => _maintenanceGoToRepair = value; }
        public bool IsUnderPower
        {
            get => _isUnderPower;
            set
            {
                _isUnderPower = value;
            }
        }
        public int Price { get => _price; set => _price = value; }
        public int CommonCost { get => _regularCost; set => _regularCost = value;  }
        public int OperatingCost { get => _operatingCost; set => _operatingCost = value; }
        public int BuildPrice { get => _buildPrice; }
        public int BuildingTime { get => _buildTime;}
        public int Capacity { get => _capacity; }
        public int MinNumOfPeople { get => _minNumOfPeople; set => _minNumOfPeople = value; }
        public int GameTime { get => _gameTime;}
        public int RepairTime { get => _repairTime;}
        public int IncrementLevel { get => _happinessIncrementLevel;}
        public int RecommendedAdrenalinLevel { get => _adrenalin;}
        public Queue<Entity> CustomersInQueue { get => _queueOut; set => _queueOut = value;  }
        public int CustomersInQueueAmount { get => CustomersInQueue.Count; }
        public List<Entity> CustomersInGame { get => _entityIn; set => _entityIn = value; }
        public int CustomersInGameAmount { get => CustomersInGame.Count; }

        private void ChangeMinNumOfPeople(int newValue)
        {
            if (newValue < _capacity)
                _minNumOfPeople = newValue;
        }

        private void ChangePrice(int newPrice) => _price = newPrice;

        private void ChangeState(GameState newState) => _state = newState;

        // Instead of changeQueueOut() and changeEntityIn()
        private void ChangeGamePlayers(Entity visitor)//changed the local variables to the corresponding properties to be more readable.
        {
            if (_state == GameState.BUILDING)
            {
                CustomersInQueue.Enqueue(visitor);
                ChangeState(GameState.WAITING);
            }
            else if (_state == GameState.WAITING)
            {
                CustomersInQueue.Enqueue(visitor);
                if (CustomersInQueue.Count >= _minNumOfPeople)
                {
                    int i = 0;
                    // The possible values intervallum of how many people get the next round: [minNumOfPeople;capacity]
                    while (_queueOut.Count > 0 && i < _capacity)
                    {
                        CustomersInGame.Add(_queueOut.Dequeue());
                        i++;
                    }
                    ChangeState(GameState.WORKING);
                }
            }
            else if (_state == GameState.WORKING)
            {
                CustomersInQueue.Enqueue(visitor);
            }
            else if (_state == GameState.FINISHING_ROUND)
            {
                
                CustomersInGame.Clear();
                if (CustomersInQueue.Count >= _minNumOfPeople)
                {
                    int i = 0;
                    while (CustomersInQueue.Count > 0 && i < _capacity)
                    {
                        CustomersInGame.Add(CustomersInQueue.Dequeue());
                        i++;
                    }
                    ChangeState(GameState.WORKING);
                }
                else
                    ChangeState(GameState.WAITING);
            }
        }
        public bool EnoughPlayers()
        {
            RefreshQueue();
            return CustomersInQueue.Count >= _minNumOfPeople;
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
        public void StartGame()
        {            
            FillEmptySeats();
            foreach(var item in CustomersInGame)
            {
                item.State = EntityState.DO_SOMETHING;
            }
            _stateTime = _gameTime;
        }
        public void FillEmptySeats()
        {
            int i = CustomersInGame.Count();
            while (CustomersInQueue.Count > 0 && i < _capacity)
            {
                CustomersInGame.Add(CustomersInQueue.Dequeue());
                i++;
            }
            
        }

        public List<Entity> EmptyInGameEntities()
        {
            List<Entity> entities = new List<Entity>(CustomersInGame);
            CustomersInGame.Clear();
            return entities;

        }
        
    }
}
