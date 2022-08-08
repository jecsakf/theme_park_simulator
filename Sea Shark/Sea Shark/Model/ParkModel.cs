using Sea_Shark.Persistence;
using SettlersEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

// Minden internal láthatóságú metódus és property látszódik a publikusokon kívül a tesztben.
[assembly: InternalsVisibleTo("Sea Shark Test")]

namespace Sea_Shark.Model
{
    public class ParkModel
    {
        private ParkFileDataAccess _dataAccess;
        private ParkPersistence _persistence;
        private int _tickRate;
        private int _spawnRate;
        private bool _parkIsDirty; // there is trash on the road
        private bool _campaignIsStarted;
        private int _campaignTimeLeft;

        public event EventHandler<ParkEventArgs> GameAdvanced;
        public event EventHandler<ParkEventArgs> GameOver;
        public ParkPersistence Persistence { get { return _persistence; } }
        public int Time { get => _persistence.Time; set => _persistence.Time = value; }
        public int TickRate { get => _tickRate; set => _tickRate = value; }
        public bool IsGameOver { get => _persistence.AvailableMoney <= 0; }
        public string ParkName { get => _persistence.ParkName; set => _persistence.ParkName = value; }
        public int PlayerMoney
        {
            get => _persistence.AvailableMoney;
            set
            {
                _persistence.AvailableMoney = value;
                if (IsGameOver)
                    GameOver?.Invoke(this, new ParkEventArgs(Time, true));
            }
        } 
        public bool ParkIsDirty { get => _parkIsDirty; set => _parkIsDirty = value; }
        public bool CleanersStopped { get; set; }
        public bool ParkIsOpen
        {
            get => _persistence.ParkIsOpen;
            set
            {
                List<Restaurant> restaurants = _persistence.GetAreas().Where(a => a.GetType() == typeof(Restaurant)).Select(a => (Restaurant)a).ToList();
                List<Game> games = _persistence.GetAreas().Where(a => a.GetType() == typeof(Game)).Select(a => (Game)a).ToList();
                foreach (Restaurant item in restaurants)
                {
                    if (item.IsUnderPower) item.State = RestaurantState.WAITING;
                    else item.State = RestaurantState.NO_POWER;
                }
                foreach (Game item in games)
                {
                    if (item.IsUnderPower) item.State = GameState.WAITING;
                    else item.State = GameState.NO_POWER;
                }
                _persistence.ParkIsOpen = value;
            }
        }
        public int SpawnRate { get => _spawnRate; set => _spawnRate = value; }
        public int CampaignTimeLeft { get => _campaignTimeLeft; set => _campaignTimeLeft = value; }
        public bool CampaignIsStarted { get => _campaignIsStarted; set => _campaignIsStarted = value; }
        public ParkModel(int rowSize, int colSize, ParkFileDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _persistence = new ParkPersistence(rowSize, colSize);
            Time = 0;
            TickRate = 1000; // in ms
            _campaignIsStarted = false;
            _spawnRate = 4;
            ParkIsDirty = false;

            MakeElectricity(_persistence.StartField, PowerSource.POWERSOURCE_RANGE);
        }

        /// <summary>
        /// Ha a kijelölt elemet le lehet tenni a megadott koordinátákra, akkor ez a függvény le is teszi. Beállítja a megadott koordinátának az áramellátását.
        /// </summary>
        /// <param name="row">A cél X koordinátája</param>
        /// <param name="col">A cél Y koordinátája</param>
        /// <param name="selectedItem">A lerakandó elem</param>
        /// <param name="price">A lerakandó elem ára</param>
        /// <returns>0-t ad vissza, ha sikertelen volt a lehelyezés. Egy pozitív egész számot (az elem árát) adja vissza, ha sikeres volt.</returns>
        public int ClickedRoadType(int row, int col, IArea selectedItem, int price)
        {
            if (selectedItem.GetType() == typeof(Staff))
            {
                Staff SelectedStaff = (Staff)selectedItem;
                Staff newStaff = new Staff(SelectedStaff.Type);
                _persistence.AddStaff(row, col, newStaff);
            }
            else if (selectedItem.GetType() == typeof(Game))
            {
                Road tmp = (Road)_persistence.GetField(row, col).Area;
                Game SelectedGame = (Game)selectedItem;
                if (tmp.Type != RoadType.BUBBLE || SelectedGame.Type != GameType.BUNGEE_JUMPING){ return price; }
                Game newGame = new Game(SelectedGame.Type);
                _persistence.GetField(row, col).Area = newGame;
                _persistence.GetAreasWithCoordinates().Remove(new Point(row, col));
                _persistence.GetAreasWithCoordinates().Add(new Point(row, col), _persistence.GetField(row, col).Area);
                if (_persistence.GetField(row, col).HasPower) newGame.IsUnderPower = true;
            }
            price = _persistence.GetField(row, col).Area.BuildPrice;
            return price;
        }

        /// <summary>
        /// Ha a kijelölt elemet le lehet tenni a megadott koordinátákra, akkor ez a függvény le is teszi. Beállítja a megadott koordinátának a környezetében az áramellátást.
        /// </summary>
        /// <param name="row">A cél X koordinátája</param>
        /// <param name="col">A cél Y koordinátája</param>
        /// <param name="selectedItem">A lerakandó elem</param>
        /// <param name="price">A lerakandó elem ára</param>
        /// <returns>0-t ad vissza, ha sikertelen volt a lehelyezés. Egy pozitív egész számot (az elem árát) adja vissza, ha sikeres volt.</returns>
        public int ClickedEmptyFieldType(int row, int col, IArea SelectedItem, ref int price)
        {
            Type buildingClassType = SelectedItem.GetType();
            List<String> buildingsWhichBuildNextToRoad = new List<String>() { "Game", "Restaurant", "Bin", "WC"};
            List<String> buildingsWhichHasType = new List<String>() { "Game", "Restaurant", "Road", "Plant"};

            if (buildingClassType.Name == "Staff") return price;

            if (buildingClassType.Name == "Game")
                if ((GameType)SelectedItem.EnumType.CompareTo(GameType.BUNGEE_JUMPING) == 0) return price;
            if (buildingClassType.Name == "Road")
                if (NeighbourRoad(row, col, RoadType.SIDEWALK).Count() == 0 && NeighbourRoad(row, col, RoadType.BUBBLE).Count() == 0) { return price; }

            else if (buildingsWhichBuildNextToRoad.Contains(buildingClassType.Name))
                if (NeighbourRoad(row, col, RoadType.SIDEWALK).Count() == 0) { return price; }

            if (buildingsWhichHasType.Contains(buildingClassType.Name))
                _persistence.GetField(row, col).Area = (IArea)Activator.CreateInstance(buildingClassType,SelectedItem.EnumType);
            else
                _persistence.GetField(row, col).Area = (IArea)Activator.CreateInstance(buildingClassType);

            _persistence.GetAreasWithCoordinates().Add(new Point(row, col), _persistence.GetField(row, col).Area);
            price = _persistence.GetField(row, col).Area.BuildPrice;

            MakePowerIfPossible(row, col, buildingClassType);

            return price;
        }

        /// <summary>
        /// A megadott koordinátán ha játék vagy étterem van, akkor ad nekik áramot, ha áramgenerátor van a koordinátán, akkor hatósugáron belül árammal látja el az mezőket.
        /// </summary>
        /// <param name="row">A vizsgált mező X koordinátája.</param>
        /// <param name="col">A vizsgált mező Y koordinátája.</param>
        /// <param name="buildingClassType">A mező típusa.</param>
        public void MakePowerIfPossible(int row, int col, Type buildingClassType)
        {
            if (buildingClassType.Name == "Game")
                SetPowerOnIfPossible(row, col);

            if (buildingClassType.Name == "Restaurant")
                SetPowerOnIfPossible(row, col);

            if (buildingClassType.Name == "PowerSource")
                MakeElectricity(new Point(row, col), PowerSource.POWERSOURCE_RANGE);
        }

        /// <summary>
        /// Az adott koordinátára megpróbálja beállítani az áramot. Így a rajta lévő épület is áram alá kerül.
        /// </summary>
        /// <param name="row">A vizsgált mező X koordinátája.</param>
        /// <param name="col">A vizsgált mező Y koordinátája.</param>
        public void SetPowerOnIfPossible(int row, int col)
        {
            if (_persistence.GetField(row, col).HasPower)
            {
                dynamic newItem = _persistence.GetField(row, col).Area;
                newItem.IsUnderPower = true;
            }
        }

        /// <summary>
        /// A megadott koordinátán megpróbálja lehelyezni a kiválasztott elemet.
        /// </summary>
        /// <param name="row">A vizsgált mező X koordinátája.</param>
        /// <param name="col">A vizsgált mező Y koordinátája.</param>
        /// <param name="selectedItem">A lerakandó elem</param>
        /// <returns>0-t ad vissza, ha sikertelen volt a lehelyezés. Egy pozitív egész számot (az elem árát) adja vissza, ha sikeres volt.</returns>
        public int Step(int row, int col, IArea selectedItem)
        {
            int price = 0;
            if (_persistence.GetField(row, col).Area.GetType() == typeof(Road))
            {
                price = ClickedRoadType(row, col, selectedItem, price);
            }
            else if (_persistence.GetField(row, col).Area.GetType() == typeof(EmptyField))
            {
                price = ClickedEmptyFieldType(row, col, selectedItem, ref price);
            }
            return price;
        }

        /// <summary>
        /// A megadott helyen lévő építményhez visszaadja a bejáratok listáját.
        /// </summary>
        /// <param name="row">A vizsgált mező X koordinátája.</param>
        /// <param name="col">A vizsgált mező Y koordinátája.</param>
        /// <param name="type">A keresendő bejárati út típusa.</param>
        /// <returns>A bejáratok koordinátáit tartalmazó listát.</returns>
        private List<Point> NeighbourRoad(int row, int col, RoadType type)
        {
            List<Point> neighbours = new List<Point>();
            int fromRow = row >= 1 ? row - 1 : row;
            int toRow = row < _persistence.RowSize - 1 ? row + 1 : row;
            int fromCol = col >= 1 ? col - 1 : col;
            int toCol = col < _persistence.ColSize - 1 ? col + 1 : col;
            int[,] whereToCheck = new int[4, 2] { { fromRow, col }, { toRow, col }, { row, fromCol }, { row, toCol } };
            for (int i = 0; i < 4; i++)
            {
                if (_persistence.GetField(whereToCheck[i, 0], whereToCheck[i, 1]).Area.GetType() == typeof(Road))
                {
                    Road tmp = (Road)_persistence.GetField(whereToCheck[i, 0], whereToCheck[i, 1]).Area;
                    if (tmp.Type == type) { neighbours.Add(new Point(whereToCheck[i, 0], whereToCheck[i, 1])); }
                }
            }
            return neighbours;
        }
        
        /// <summary>
        /// Lekezeli a játék előrehaladását.
        /// </summary>
        public void OnTick()
        {
            if (ParkIsOpen)
            {
                ManageEntities();
                ManageStaffs();
                ManageBuildings();
                ManageCampaign();

                if (Time % _spawnRate == 0) CreateNewEntity();
                if(Time % 10 == 0) DeductCommonCosts();

                if (ParkIsDirty)
                {
                    foreach (var item in _persistence.GetEntities())
                        item.Key.ReduceHappiness(1);
                }

                Time++;
                OnGameAdvanced();
            }
            else
            {
                BuildingsStateTurnToWaitingFromBuilding();
            }
        }

        /// <summary>
        /// Az épületek állapotát állítja be building-ről waiting-re, ha még nincs nyitva a park.
        /// </summary>
        internal void BuildingsStateTurnToWaitingFromBuilding()
        {
            foreach (var item in _persistence.GetAreasWithCoordinates())
            {
                switch (item.Value.GetType().Name.ToString())
                {
                    case "Game":
                        Game game = (Game)item.Value;
                        game.State = GameState.WAITING;
                        game.StateTime = 1;
                        break;
                    case "Restaurant":
                        Restaurant restaurant = (Restaurant)item.Value;
                        restaurant.State = RestaurantState.WAITING;
                        restaurant.StateTime = 1;
                        break;
                }
            }
        }

        /// <summary>
        /// Az állapotokat kezeli a játékokban, éttermekben, növényeknél, árramforrásoknál.
        /// </summary>
        private void ManageBuildings()
        {
            foreach (var item in _persistence.GetAreasWithCoordinates())//onTick for buildings
            {
                Point currentLocation = item.Key;
                IArea building = item.Value;
                switch (item.Value.GetType().Name.ToString())
                {
                    case "Game":
                        ManageGames(building, currentLocation);
                        break;
                    case "Restaurant":
                        ManageRestaurants(building, currentLocation);
                        break;
                    case "Plant":
                        ManagePlants(building, currentLocation);
                        break;
                    case "PowerSource":
                        MakeElectricity(currentLocation, PowerSource.POWERSOURCE_RANGE);//betolteshez atrakni
                        break;
                }
            }
        }

        /// <summary>
        /// Az étterem állapotát kezeli.
        /// </summary>
        /// <param name="building">Maga az étterem típusa</param>
        /// <param name="currentLocation">Az étterem pozíciója</param>
        private void ManageRestaurants(IArea building, Point currentLocation)
        {
            Restaurant restaurant = (Restaurant)building;
            //WORKING(one at a time), BUILDING, WAITING(no one in queue)
            if (restaurant.StateTime == 0)
            {
                switch (restaurant.State)
                {
                    case RestaurantState.WAITING:
                        ManageRestaurantStateWaiting(ref restaurant);
                        break;
                    case RestaurantState.WORKING:
                        ManageRestaurantStateWorking(ref restaurant, currentLocation);
                        break;
                    case RestaurantState.BUILDING:
                        if (restaurant.IsUnderPower)
                            restaurant.State = RestaurantState.WAITING;
                        else
                        {
                            restaurant.State = RestaurantState.NO_POWER;
                        }
                        restaurant.StateTime = 1;
                        break;
                    case RestaurantState.NO_POWER:
                        if (restaurant.IsUnderPower)
                            restaurant.State = RestaurantState.WAITING;
                        restaurant.StateTime = 1;
                        break;
                }
            }
            restaurant.StateTime--;
        }

        /// <summary>
        /// Megnöveli a hatósugaron belüli entitások hangulatát
        /// </summary>
        /// <param name="building"></param>
        /// <param name="currentLocation"></param>
        private void ManagePlants(IArea building, Point currentLocation)
        {
            Plant plant = (Plant)building;
            for (int i = currentLocation.X - plant.Range; i < currentLocation.X + plant.Range + 1; i++)
            {
                for (int j = currentLocation.Y - plant.Range; j < currentLocation.Y + plant.Range + 1; j++)
                {
                    if (-1 < i && i < _persistence.RowSize)
                    {
                        if (-1 < j && j < _persistence.ColSize)
                            foreach (var e in _persistence.GetFields()[i, j].Entities)
                            {
                                e.AddHappiness(1);
                            }
                    }
                }
            }
        }

        /// <summary>
        /// A kampány idejét kezeli.
        /// </summary>
        internal void ManageCampaign()
        {
            if (_campaignIsStarted)
            {
                if (_campaignTimeLeft == 0)
                {
                    _spawnRate = 4;
                    _campaignIsStarted = false;
                }
                else
                    _campaignTimeLeft--;                
            }
        }

        /// <summary>
        /// Az étterem működési folyamatát végzi el.
        /// </summary>
        /// <param name="restaurant">Az étterem referenciája.</param>
        /// <param name="currentLocation">Az étterem pozíciója</param>
        private void ManageRestaurantStateWorking(ref Restaurant restaurant, Point currentLocation)
        {
            List<Entity> backToField = restaurant.EmptyInRestaurantEntities();
            PutEntitiesBackToTheEntry(backToField, currentLocation, restaurant.ReductionLevel, "Restaurant");

            PlayerMoney -= restaurant.OperatingCost;
            restaurant.State = RestaurantState.WAITING;
            restaurant.StateTime = 1;
        }

        /// <summary>
        /// Az étterem várakozását végzi el.
        /// </summary>
        /// <param name="restaurant">Az étterem referenciája.</param>
        private void ManageRestaurantStateWaiting(ref Restaurant restaurant)
        {
            if (restaurant.EntityInQueue())
            {
                restaurant.StartServing();
                restaurant.State = RestaurantState.WORKING;
            }
            else
                restaurant.StateTime = 1;
        }

        /// <summary>
        /// A játék állapotait kezeli.
        /// </summary>
        /// <param name="building">Kezelendő játék.</param>
        /// <param name="currentLocation">A kezelendő játék koordinátája.</param>
        private void ManageGames(IArea building, Point currentLocation)
        {
            Game game = (Game)building;
            if (game.StateTime == 0)//amikor lejár a statetime
            {
                switch (game.State)
                {
                    case GameState.BUILDING:
                        game.StateTime = 1;
                        if (!game.IsUnderPower)
                        {
                            game.State = GameState.NO_POWER;
                            break;
                        }                            
                        game.State = GameState.WAITING;                        
                        break;
                    case GameState.WORKING:
                        game.State = GameState.FINISHING_ROUND;
                        game.StateTime = 1;
                        break;
                    case GameState.WAITING:
                        ManageGameStateWaiting(ref game);
                        break;
                    case GameState.FINISHING_ROUND:
                        ManageGameStateFinishingRound(ref game, currentLocation);
                        break;
                    case GameState.BROKEN:
                        ManageGameStateBroken(ref game, currentLocation);
                        break;
                    case GameState.REPAIRING:
                        ManageGameStateRepairing(ref game, currentLocation);
                        break;
                    case GameState.NO_POWER:
                        game.StateTime = 1;
                        if (game.IsUnderPower)
                            game.State = GameState.WAITING;
                        break;
                }
            }
            game.StateTime--;
        }

        /// <summary>
        /// A játék helyreállítási folyamatát végzi el.
        /// </summary>
        /// <param name="game">A játék referenciája.</param>
        /// <param name="currentLocation">A játék pozíciója</param>
        private void ManageGameStateRepairing(ref Game game, Point currentLocation)
        {
            Game noref_game = game;
            Staff repairGuy = _persistence.GetStaffs().Select(d => d.Key).Where(s => s.GameToRepair == noref_game).FirstOrDefault();
            repairGuy.State = StaffState.NOTHING_TO_DO;
            repairGuy.GameToRepair = null;
            repairGuy.Destination = new Point(-1, -1);

            game.MaintenanceGoToRepair = false;
            game.State = GameState.WAITING;
            game.StateTime = 1;
        }

        /// <summary>
        /// Ez a függvény keres karbantartót a játékhoz.
        /// </summary>
        /// <param name="game">A játék referenciája.</param>
        /// <param name="currentLocation">A játék pozíciója</param>
        private void ManageGameStateBroken(ref Game game, Point currentLocation)
        {
            if (!game.MaintenanceGoToRepair)
            {
                Staff nearestMaintance = NearestMaintenance(currentLocation);
                if (nearestMaintance != null)
                    game.MaintenanceGoToRepair = true;
                else
                {
                    game.MaintenanceGoToRepair = false;
                    game.StateTime = 1;
                }
            }
        }

        /// <summary>
        /// A játékidő végének a folyamatát végzi el.
        /// </summary>
        /// <param name="game">A játék referenciája.</param>
        /// <param name="currentLocation">A játék pozíciója</param>
        private void ManageGameStateFinishingRound(ref Game game, Point currentLocation)
        {
            List<Entity> backToField = game.EmptyInGameEntities();
            PutEntitiesBackToTheEntry(backToField, currentLocation, game.IncrementLevel, "Game");

            PlayerMoney -= game.OperatingCost;
            Random rand = new Random();
            int broken = rand.Next(1);
            if (broken == 0)
            {
                GameBroke(ref game, currentLocation);
            }
            else
            {
                game.State = GameState.WAITING;
                game.StateTime = 1;
            }
        }

        /// <summary>
        /// A játék elromlásának a folyamatát végzi el.
        /// </summary>
        /// <param name="game">A játék referenciája.</param>
        /// <param name="currentLocation">A játék pozíciója</param>
        private void GameBroke(ref Game game, Point currentLocation)
        {
            game.State = GameState.BROKEN;
            game.StateTime = 1;

            Staff nearestMaintance = NearestMaintenance(currentLocation); // beállítjuk az utat és a célt
            if (nearestMaintance != null)
                game.MaintenanceGoToRepair = true;
            else
                game.MaintenanceGoToRepair = false;
        }

        /// <summary>
        /// A tevékenység végén ez a függvény fogja az entitásokat visszahelyezni a épületbejárathoz, és lekezeli az entitás tulajdonságait.
        /// </summary>
        /// <param name="backToField">A visszahelyezendő entitások listája</param>
        /// <param name="currentLocation">Az épület koordinátája.</param>
        /// <param name="incrementLevel">Ennyivel változtatja meg a szükségletét</param>
        /// <param name="buildingType">Az épület típus. (játék vagy étterem)</param>
        private void PutEntitiesBackToTheEntry(List<Entity> backToField, Point currentLocation, int incrementLevel, string buildingType)
        {
            foreach (var entity in backToField)
            {
                if (buildingType == "Restaurant")
                {
                    entity.Hunger += incrementLevel;
                    entity.WcUrge += incrementLevel;
                    entity.HasTrash = true;
                }
                else
                    entity.Happiness += incrementLevel;

                entity.State = EntityState.NOTHING_TO_DO;
                List<Point> bubble = NeighbourRoad(currentLocation.X, currentLocation.Y, RoadType.BUBBLE);
                List<Point> side = NeighbourRoad(currentLocation.X, currentLocation.Y, RoadType.SIDEWALK);
                List<Point> tmp = bubble.Count == 0 ? side : bubble;
                _persistence.GetField(tmp[0].X, tmp[0].Y).AddEntity(entity);
            }
        }

        /// <summary>
        /// A játék várakozási folyamatát végzi el.
        /// </summary>
        /// <param name="game">A játék referenciája.</param>
        private void ManageGameStateWaiting(ref Game game)
        {
            //game.fillEmptySeats();
            if (game.EnoughPlayers())
            {
                game.StartGame();
                game.State = GameState.WORKING;
            }
            else
                game.StateTime = 1;
        }

        /// <summary>
        /// Levonja az épületek rendszeres költségeit.
        /// </summary>
        private void DeductCommonCosts()
        {
            foreach (var item in _persistence.GetAreasWithCoordinates())//onTick for buildings
            {
                switch (item.Value.GetType().Name.ToString())
                {
                    case "Game":
                        Game game = (Game)item.Value;
                        PlayerMoney -= game.CommonCost;
                        break;
                    case "Restaurant":
                        Restaurant restaurant = (Restaurant)item.Value;
                        PlayerMoney -= restaurant.CommonCost;
                        break;
                }
            }
        }

        /// <summary>
        /// Új entitást hoz létre úti céllal és útvonallal.
        /// </summary>
        private void CreateNewEntity()
        {
            Random rand = new Random();
            int lucky = rand.Next(2);
            EntityType entityType = lucky == 0 ? EntityType.HUMAN : EntityType.SHARK;
            bool entityHasCoupon;
            lucky = rand.Next(101);
            if (_campaignIsStarted)
            {
                entityHasCoupon = lucky < 60 ? true : false;
            }
            else
            {
                entityHasCoupon = false;
            }
            Entity newEntity = new Entity(entityType, entityHasCoupon, _persistence.GetEntryFee());
            if (newEntity.HasEnoughMoney(_persistence.GetEntryFee()))//only if the entities has enough money, they can join the park.
            {
                _persistence.AvailableMoney += _persistence.GetEntryFee();//the park gets the entry fee
                _persistence.AddEntity(_persistence.StartField.X, _persistence.StartField.Y, newEntity);
                ChooseAimAndGoThere(newEntity, _persistence.StartField);
            }
            else EntityGoHome(newEntity);
        }

        /// <summary>
        /// A takarítók és karbantartók autonóm járását végzi el.
        /// </summary>
        private void ManageStaffs()
        {
            foreach (var item in _persistence.GetStaffs())
            {
                Staff staff = item.Key;
                Point currentLocation = item.Value;

                if (staff.State == StaffState.NOTHING_TO_DO)
                {
                    staff.EndOfRoad = staff.ChooseDirectionToGo(_persistence.GetFields(), _persistence.RowSize, _persistence.GetField(currentLocation.X, currentLocation.Y));
                    staff.PathToAim = new List<Field>(FindPath(_persistence.GetStaffs().FirstOrDefault(e => e.Key == staff).Value, staff.EndOfRoad));
                    staff.State = StaffState.AUTONOMOUS_WALKING;
                }

                if (staff.State == StaffState.AUTONOMOUS_WALKING || staff.State == StaffState.ON_THE_WAY)
                {
                    int index = staff.PathToAim.IndexOf(_persistence.GetField(currentLocation.X, currentLocation.Y))+1;
                    _persistence.GetField(currentLocation.X, currentLocation.Y).RemoveStaff(staff);

                    if (index != staff.PathToAim.Count())
                        GoStraight(staff, currentLocation, index);
                    else if (_persistence.GetStaffs()[staff] == new Point(staff.PathToAim.Last().Row, staff.PathToAim.Last().Col))
                        StaffIsArrived(ref staff, currentLocation);
                }
            }
        }

        /// <summary>
        /// Áthelyezi a kiválasztott takarítót a megadott koordinátára.
        /// </summary>
        /// <param name="_selectedCleaner">A takarító.</param>
        /// <param name="newX">A cél X koordinátája</param>
        /// <param name="newY">A cél Y koordinátája</param>
        public void SetTransferedCleaner(Staff _selectedCleaner, int newX, int newY)
        {
            Point newLocation = new Point(newX, newY);
            var staffWithCoordinate = _persistence.GetStaffs().FirstOrDefault(s => s.Key == _selectedCleaner);
            Staff staff = staffWithCoordinate.Key;
            Point currentLocation = staffWithCoordinate.Value;

            _persistence.GetField(currentLocation.X, currentLocation.Y).RemoveStaff(staff);

            _persistence.GetStaffs()[staff] = newLocation;
            _persistence.GetField(newLocation.X, newLocation.Y).AddStaff(staff);
            staff.State = StaffState.NOTHING_TO_DO;
        }

        /// <summary>
        /// Ha a személyzet is karbantartó volt, akkor megjavítja a játékot, miután megérkezett.
        /// Ha takarító volt, akkor megy tovább.
        /// </summary>
        /// <param name="staff">A személyzet aki megérkezett</param>
        /// <param name="currentLocation">Ahova érkezett</param>
        private void StaffIsArrived(ref Staff staff, Point currentLocation)
        {
            _persistence.GetStaffs()[staff] = currentLocation;
            _persistence.GetField(currentLocation.X, currentLocation.Y).AddStaff(staff);
            Staff noref_staff = staff;
            if (staff.State == StaffState.ON_THE_WAY)
            {
                staff.State = StaffState.REPAIRING;
                staff.GameToRepair.State = GameState.REPAIRING;
                staff.GameToRepair.StateTime = staff.GameToRepair.RepairTime;
            }
            else
            {
                staff.EndOfRoad = staff.ChooseDirectionToGo(_persistence.GetFields(), _persistence.RowSize, _persistence.GetField(currentLocation.X, currentLocation.Y));
                staff.PathToAim = new List<Field>(FindPath(_persistence.GetStaffs().FirstOrDefault(e => e.Key == noref_staff).Value, staff.EndOfRoad));
                staff.State = StaffState.AUTONOMOUS_WALKING;
            }
        }

        /// <summary>
        /// Egyenesen megy, amíg tud egy adott irányba
        /// </summary>
        /// <param name="staff">A sétáló személyzet</param>
        /// <param name="currentLocation">A személyzet aktuiális pozíciója</param>
        /// <param name="index">Az útvonalának melyik pontjánál tart.</param>
        private void GoStraight(Staff staff, Point currentLocation, int index)
        {
            currentLocation = new Point(staff.PathToAim[index].Row, staff.PathToAim[index].Col);
            _persistence.GetStaffs()[staff] = currentLocation;
            _persistence.GetField(currentLocation.X, currentLocation.Y).AddStaff(staff);

            if (staff.Type == StaffType.CLEANER)
            {
                Road currentRoad = (Road)_persistence.GetField(currentLocation.X, currentLocation.Y).Area;
                if (currentRoad.TrashAmount > 0)
                    currentRoad.TrashAmount = 0;
            }
        }

        /// <summary>
        /// Kezeli az entitások állapotát és szükségleteit.
        /// </summary>
        private void ManageEntities()
        {
            foreach (var item in _persistence.GetEntities())
            {
                Entity entity = item.Key;
                Point currentLocation = item.Value;

                if (entity.WannaGoHome) EntityGoHome(entity);

                entity.ReduceHappiness(1);
                entity.AddWcUrge(1);//add instead of reduce :D
                entity.ReduceHunger(1);

                ManageEntityState(ref entity, ref currentLocation);
            }
        }

        /// <summary>
        /// Az entitások állapotát kezeli.
        /// </summary>
        /// <param name="entity">Az entitás referenciája.</param>
        /// <param name="currentLocation">Az entitás pozíciója.</param>
        private void ManageEntityState(ref Entity entity, ref Point currentLocation)
        {
            if (entity.State == EntityState.NOTHING_TO_DO)
                ChooseAimAndGoThere(entity, currentLocation);
            else if (entity.State == EntityState.WAITING_IN_LINE)
                entity.ReduceHappiness(1);
            else if (entity.State == EntityState.ON_THE_WAY)
            {
                int index = entity.PathToAim.IndexOf(_persistence.GetField(currentLocation.X, currentLocation.Y));
                _persistence.GetField(currentLocation.X, currentLocation.Y).RemoveEntity(entity);

                if (index + 1 != entity.PathToAim.Count())
                {
                    currentLocation = new Point(entity.PathToAim[index + 1].Row, entity.PathToAim[index + 1].Col);
                    _persistence.GetEntities()[entity] = currentLocation;
                    _persistence.GetField(currentLocation.X, currentLocation.Y).AddEntity(entity);
                }

                if (_persistence.GetEntities()[entity] == new Point(entity.PathToAim.Last().Row, entity.PathToAim.Last().Col))
                {
                    if(_persistence.GetEntities()[entity] == _persistence.StartField) EntityStepOutFromPark(entity, currentLocation);
                    else EntityArrivesAim(ref entity, currentLocation);
                }
                    
            }
        }

        /// <summary>
        /// Az entitás úti célhoz érkezését kezeli le.
        /// </summary>
        /// <param name="entity">Az entitás referenciája</param>
        /// <param name="currentLocation">Az entitás pozíciója</param>
        private void EntityArrivesAim(ref Entity entity, Point currentLocation)
        {
            entity.LastVisitedArea = entity.Aim.Area;
            entity.PathToAim = _persistence.NoPath;
            entity.State = EntityState.NOTHING_TO_DO;
            // ManageBuildingWhenEntityArrivesThere metódusokat össze lehet vonni egybe
            switch (entity.Aim.Area.GetType().Name.ToString())
            {
                case "Game":
                    ManageGameWhenEntityArrivesThere(ref entity, currentLocation);
                    break;
                case "Restaurant":
                    ManageRestaurantWhenEntityArrivesThere(ref entity, currentLocation);
                    break;
                case "WC":
                    ManageWCEntityWhenArrivesThere(ref entity, currentLocation);
                    break;
                case "Bin":
                    entity.HasTrash = false;
                    entity.State = EntityState.NOTHING_TO_DO;
                    break;
            }
        }

        /// <summary>
        /// Amikor WC-hez ér az entitás.
        /// </summary>
        /// <param name="entity">Az entitás referenciája</param>
        /// <param name="currentLocation">Az entitás pozíciója</param>
        private void ManageWCEntityWhenArrivesThere(ref Entity entity, Point currentLocation)
        {
            WC aimWC = (WC)entity.Aim.Area;
            if (entity.HasEnoughMoney(aimWC.Price))
            {
                PlayerMoney += aimWC.Price;
                entity.WcUrge = 0;
                entity.State = EntityState.NOTHING_TO_DO;
            }
            else
            {
                entity.State = EntityState.NOTHING_TO_DO;
            }
        }

        /// <summary>
        /// Amikor étteremhez ér az entitás.
        /// </summary>
        /// <param name="entity">Az entitás referenciája</param>
        /// <param name="currentLocation">Az entitás pozíciója</param>
        private void ManageRestaurantWhenEntityArrivesThere(ref Entity entity, Point currentLocation)
        {
            Restaurant aimRestaurant = (Restaurant)entity.Aim.Area;
            if (entity.HasEnoughMoney(aimRestaurant.Price))
            {
                PlayerMoney += aimRestaurant.Price;
                entity.State = EntityState.WAITING_IN_LINE;
                aimRestaurant.CustomersInQueue.Enqueue(entity);
                _persistence.RemoveEntity(currentLocation.X, currentLocation.Y, entity);
            }
            else
            {
                entity.State = EntityState.NOTHING_TO_DO;
            }
        }

        /// <summary>
        /// Amikor játékhoz ér az entitás.
        /// </summary>
        /// <param name="entity">Az entitás referenciája</param>
        /// <param name="currentLocation">Az entitás pozíciója</param>
        private void ManageGameWhenEntityArrivesThere(ref Entity entity, Point currentLocation)
        {
            Game aimGame = (Game)entity.Aim.Area;
            if (aimGame.State == GameState.WORKING || aimGame.State == GameState.WAITING || aimGame.State == GameState.FINISHING_ROUND)
            {
                if (entity.HasEnoughMoney(aimGame.Price))
                {
                    PlayerMoney += aimGame.Price;
                    entity.State = EntityState.WAITING_IN_LINE;
                    aimGame.CustomersInQueue.Enqueue(entity);
                    _persistence.RemoveEntity(currentLocation.X, currentLocation.Y, entity);
                }
                else
                    entity.State = EntityState.NOTHING_TO_DO;
            }
            else
                entity.State = EntityState.NOTHING_TO_DO;
        }

        /// <summary>
        /// Elindítja a kampányt.
        /// </summary>
        public void StartCampaign()
        {
            _campaignIsStarted = true;
            _campaignTimeLeft = 30;
            PlayerMoney -= 1000;
            _spawnRate = 2;
        }

        /// <summary>
        /// Ez a függvény állítja be a hatósugaron belüli fieldeknek az áramot.
        /// </summary>
        /// <param name="powerSource">A lerakott árramforrás</param>
        /// <param name="range">Hatósugár</param>
        private void MakeElectricity(Point powerSource, int range)
        {
            for (int i = powerSource.X - range; i < powerSource.X + range + 1; i++)
            {
                for (int j = powerSource.Y - range; j < powerSource.Y + range + 1; j++)
                {
                    if(-1 < i && i < _persistence.RowSize)
                    {
                        if(-1 < j && j < _persistence.ColSize)
                            _persistence.GetFields()[i, j].HasPower = true;
                    }
                }
            }
        }

        /// <summary>
        /// Megállítja vagy elindítja az összes takarítót.
        /// </summary>
        /// <param name="staffState">Ez a célállapot</param>
        /// <param name="stopThem">Megadja, hogy megálljanak-e a takarítók.</param>
        public void StopOrStartAllCleaners(StaffState staffState, bool stopThem)
        {
            foreach (var staff in _persistence.GetStaffs().Keys)
                if(staff.Type == StaffType.CLEANER)
                    staff.State = staffState;

            CleanersStopped = stopThem;
        }

        /// <summary>
        /// Betölti a perzisztencián keresztül a mentett játékot.
        /// </summary>
        /// <param name="path">A mentett játék elérési útvonala.</param>
        /// <returns></returns>
        public async Task LoadGameAsync(string path)
        {
            _persistence = await _dataAccess.LoadAsync(path);
        }

        /// <summary>
        /// Elmenti a perzisztencián keresztül a játék állapotát.
        /// </summary>
        /// <param name="path">A mentés útvonala.</param>
        /// <returns>Task</returns>
        /// <exception cref="InvalidOperationException">Nincs hozzáférésed</exception>
        public async Task SaveGameAsync(string path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("Nincs hozzáférésed!");
            await _dataAccess.SaveAsync(path, _persistence);
        }

        /// <summary>
        /// Megpróbálja megkeresni a legközelebbi szabad karbantartót, ha talál ilyet, akkor beállítja neki a játékhoz vezető útvonalat.
        /// </summary>
        /// <param name="gameCoordination">A javítandó játék</param>
        /// <returns>Visszaadja a legközelebb lévő szabad karbantartót.</returns>
        private Staff NearestMaintenance(Point gameCoordination)
        {
            List<Staff> freeMaintenances = _persistence.GetStaffs().Select(d => d.Key).Where(s => s.Type == StaffType.MAINTENANCE && s.Destination == new Point(-1,-1)).ToList();
            Staff nearestGuy = null;
            List<Field> shortestPath = new List<Field>();
            int shortestPathCount = _persistence.RowSize * _persistence.RowSize;

            SearchNearestMaintenance(freeMaintenances, ref shortestPathCount, ref shortestPath, ref nearestGuy, gameCoordination);
            SetNearestMaintenance(ref nearestGuy, shortestPath, gameCoordination);

            return nearestGuy;
        }

        /// <summary>
        /// Megpróbálja megkeresni a legközelebbi szabad karbantartót.
        /// </summary>
        /// <param name="freeMaintenances">A szabad karbantartók listája.</param>
        /// <param name="shortestPathCount">A legrövidebb útnak a hossza</param>
        /// <param name="shortestPath">A legrövidebb útvonal.</param>
        /// <param name="nearestGuy">A legközelebbi karbantartó</param>
        /// <param name="gameCoordination">A javítandó játék</param>
        public void SearchNearestMaintenance(List<Staff> freeMaintenances, ref int shortestPathCount, ref List<Field> shortestPath, ref Staff nearestGuy, Point gameCoordination)
        {
            foreach (var m in freeMaintenances)
            {
                RoadType searchedRoadType = _persistence.GetAreasWithCoordinates().Where(d => d.Key == gameCoordination).Select(d => (Game)d.Value).First().Type == GameType.BUNGEE_JUMPING ? RoadType.BUBBLE : RoadType.SIDEWALK;
                Point entrance = NeighbourRoad(gameCoordination.X, gameCoordination.Y, searchedRoadType).First();
                List<Field> pathToGame = new List<Field>(FindPath(_persistence.GetStaffs().FirstOrDefault(e => e.Key == m).Value, entrance));
                if (pathToGame.Count() < shortestPathCount)
                {
                    shortestPathCount = pathToGame.Count();
                    shortestPath = pathToGame;
                    nearestGuy = m;
                }
            }
        }

        /// <summary>
        /// Beállítja neki a játékhoz vezető útvonalat
        /// </summary>
        /// <param name="nearestGuy">A legközelebbi karbantartó</param>
        /// <param name="shortestPath">A legrövidebb útvonal.</param>
        /// <param name="gameCoordination">A javítandó játék</param>
        public void SetNearestMaintenance(ref Staff nearestGuy, List<Field> shortestPath, Point gameCoordination)
        {
            if (nearestGuy != null)
            {
                nearestGuy.Destination = new Point(shortestPath.Last().Row, shortestPath.Last().Col);
                nearestGuy.PathToAim = shortestPath;
                nearestGuy.State = StaffState.ON_THE_WAY;
                nearestGuy.GameToRepair = _persistence.GetAreasWithCoordinates().Where(d => d.Key == gameCoordination).Select(d => (Game)d.Value).First();
            }
        }

        /// <summary>
        /// Kiválaszt az adott entitásnak egy úticélt, és beállítja az útvonalat.
        /// </summary>
        /// <param name="entity">Az entitás maga.</param>
        /// <param name="entityPosition">Az entitás pozíciója</param>
        public void ChooseAimAndGoThere(Entity entity, Point entityPosition)
        {
            IArea aim = entity.ChooseAim(_persistence.GetAreas(), _persistence.GetFields(), entityPosition, ParkIsOpen, ParkIsDirty);
            if (aim == null)
            {
                EntityGoHome(entity);
            }
            else
            {
                Point aimCoordinates = _persistence.StartField;
                RoadType searchedNeighbourRoadType = RoadType.SIDEWALK;

                SetAimCoordinates(aim, ref aimCoordinates);
                if (aim.EnumType.GetType() == typeof(GameType) && aim.EnumType.CompareTo(GameType.BUNGEE_JUMPING) == 0)
                    searchedNeighbourRoadType = RoadType.BUBBLE;

                entity.Aim = _persistence.GetField(aimCoordinates.X, aimCoordinates.Y);
                Debug.WriteLine(entity.Money.ToString() + " " + entity.Aim.Row.ToString() + " " + entity.Aim.Col.ToString() + " " + entity.Aim.Area.EnumType);
                Point aimNeighbourRoad = NeighbourRoad(aimCoordinates.X, aimCoordinates.Y, searchedNeighbourRoadType).First();
                entity.PathToAim = new List<Field>(FindPath(_persistence.GetEntities().FirstOrDefault(e => e.Key == entity).Value, aimNeighbourRoad));
                entity.State = EntityState.ON_THE_WAY;
            }
        }

        /// <summary>
        /// Hazairányítja az entitást.
        /// </summary>
        /// <param name="entity">Az entitás maga</param>
        private void EntityGoHome(Entity entity)
        {
            entity.Aim = _persistence.GetField(_persistence.StartField.X, _persistence.StartField.Y);
            entity.PathToAim = new List<Field>(FindPath(_persistence.GetEntities().FirstOrDefault(e => e.Key == entity).Value, _persistence.StartField));
            entity.State = EntityState.ON_THE_WAY;
        }

        /// <summary>
        /// Az entitás elhagyja a parkot, ha el szeretne menni.
        /// </summary>
        /// <param name="entity">Az entitás maga</param>
        /// <param name="entityPosition">Az entitás pozíciója</param>
        private void EntityStepOutFromPark(Entity entity, Point entityPosition)
        {
            _persistence.GetField(entityPosition.X, entityPosition.Y).RemoveEntity(entity);
            _persistence.RemoveEntity(entityPosition.X, entityPosition.Y, entity);
            _persistence.GetEntities().Remove(entity);
        }

        /// <summary>
        /// Beáálítja a célnak a koordinátáit.
        /// </summary>
        /// <param name="aim">A cél.</param>
        /// <param name="aimCoordinates">A cél koordinátái</param>
        public void SetAimCoordinates(IArea aim, ref Point aimCoordinates)
        {
            dynamic aimBuilding = aim;
            dynamic building = _persistence.GetAreas().Where(a => a.GetType() == aimBuilding.GetType()).FirstOrDefault(a => a.EnumType.CompareTo(aimBuilding.EnumType) == 0);
            aimCoordinates = _persistence.GetAreasWithCoordinates().FirstOrDefault(d => d.Value == building).Key;
        }

        /// <summary>
        /// A* algoritmussal keres egy útvonala az egyik helyről a másikra.
        /// </summary>
        /// <param name="fromPos">Kiindulás koordinátái</param>
        /// <param name="toPos">Cél koordinátái</param>
        /// <returns>Az útvonal mezőinek listája.</returns>
        public IEnumerable<Field> FindPath(Point fromPos, Point toPos)
        {
            MySolver<Field, Object> aStar = new MySolver<Field, Object>(_persistence.GetFields());
            IEnumerable<Field> path = aStar.Search(fromPos, toPos, null); // útnak kell lennie mindkettő Pointnak
            return path;
        }

        /// <summary>
        /// Az idő múlásának eseménye.
        /// </summary>
        private void OnGameAdvanced()
        {
            if (GameAdvanced != null)
                GameAdvanced(this, new ParkEventArgs(Time,IsGameOver));
        }
    }

    public class MySolver<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
           
        }

        public MySolver(TPathNode[,] inGrid)
            : base(inGrid)
        {
        }
    }
}
