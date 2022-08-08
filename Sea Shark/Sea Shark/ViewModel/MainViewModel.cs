using Sea_Shark.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sea_shark.ViewModel;
using System.Windows;
using Sea_Shark.Persistence;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;

namespace Sea_Shark.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Private data
        private ParkModel _model;
        private ObservableCollection<GameField> _gameFields;
        private ObservableCollection<TabItem> _tabItems;
        private ObservableCollection<AreaItem> _areasInTabItem;
        private ObservableCollection<IArea> _areas;
        private ObservableCollection<Tuple<String, bool, String>> _currentShownProperties;
        private ObservableCollection<EntityItem> _allEntities;
        private List<(Point, RoadType)> _roads;
        private ObservableCollection<Tuple<String, bool, String>> _properties;
        private int _parkSize;
        private int _parkEntryFee;
        private int _customersCount;
        private int _staffCount;
        private int _satisfactionLevel;
        private string _stateObjectName;
        private int _gameTableRowSize;
        private int _gameTableColSize;
        private string _choosenAreaTypeName;
        private string _build;
        private AreaItem _selectedArea;
        private BitmapImage _imgPath;
        private static int _nextEntityId = 1;
        private Staff _selectedCleaner;
        private Field _selectedCleanerPos;
        private int _price;
        private int _minPeople;
        private Field _clickedField;
        #endregion

        #region DelegateCommands

        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand ExitGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand KeyDownCommand { get; private set; }
        public DelegateCommand StartStopCommand { get; private set; }
        public DelegateCommand ChangeBuildingModeCommand { get; set; }
        public DelegateCommand StartCampaignCommand { get; set; }
        public DelegateCommand OpenParkCommand { get; set; }
        public DelegateCommand StartMovingCommand { get; set; }
        public DelegateCommand GameSpeedDownCommand { get; set; }
        public DelegateCommand GameSpeedUpCommand { get; set; }
        public DelegateCommand SetPriceCommand { get; set; }
        public DelegateCommand SetMinNumOfPeopleCommand { get; set; }
        #endregion

        #region Events

        public event EventHandler NewGame;

        public event EventHandler LoadGame;

        public event EventHandler SaveGame;

        public event EventHandler ExitGame;

        public event EventHandler StartStop;

        public event EventHandler ParkIsOpened;

        public event EventHandler GameSpeedUp;

        public event EventHandler GameSpeedDown;

        public event EventHandler StartCampaign;

        public event EventHandler GameEnd;

        #endregion

        #region Properties

        public ObservableCollection<GameField> GameFields { get => _gameFields; set => _gameFields = value; }

        public ObservableCollection<TabItem> TabItems { get => _tabItems; set => _tabItems = value; }

        public ObservableCollection<AreaItem> AreasInTabItem { get => _areasInTabItem; set => _areasInTabItem = value; }

        public int ItemsCountInTabItem { get => _areasInTabItem.Count; }

        public int TabItemWidth { get => _areasInTabItem.Count * 150; }

        public ObservableCollection<IArea> Areas { get => _areas; set => _areas = value; }

        public ObservableCollection<Tuple<String, bool, String>> CurrentShownProperties { get => _currentShownProperties; set { _currentShownProperties = value; OnPropertyChanged(); } }

        public ObservableCollection<EntityItem> AllEntities { get => _allEntities; set { _allEntities = value; OnPropertyChanged(); } }

        public int RowCount { get; set; }

        public int ColumnCount { get; set; }

        public int GameFieldSizeV { get; set; }

        public String ParkName { get => _model.ParkName; set { _model.ParkName = value; OnPropertyChanged(); } }

        public int ParkSize { get => _parkSize; set => _parkSize = value; }

        public int PlayerMoney { get => _model.PlayerMoney; set { _model.PlayerMoney = value; OnPropertyChanged(); } }

        public int ParkEntryFee { get => _model.Persistence.GetEntryFee(); set { _model.Persistence.SetEntryFee(value < 1 ? 1 : value); OnPropertyChanged(); } }
        
        public int CustomersCount { get => _model.Persistence.GetEntities().Count; }

        public int StaffCount { get => _staffCount; set { _staffCount = value; OnPropertyChanged(); } }

        public int SatisfactionLevel
        {
            get
            {
                int satisfactionSum = 0;
                foreach (var entity in _model.Persistence.GetEntities())
                {
                    satisfactionSum += entity.Key.Happiness;
                }
                if (satisfactionSum < 0)
                    return 0;
                return _model.Persistence.GetEntities().Count == 0 ? 0 : (int)(satisfactionSum / _model.Persistence.GetEntities().Count);
            }
        }

        public String StateObjectName { get => _stateObjectName; set { _stateObjectName = value; OnPropertyChanged(); } }

        public String ChoosenAreaTypeName { get => _choosenAreaTypeName; set { _choosenAreaTypeName = value; OnPropertyChanged(); } }

        public String GameTime { get { return TimeSpan.FromSeconds(_model.Time).ToString("g"); } }

        public String Build { get => _build; set { _build = value; OnPropertyChanged(); } }

        public int Price { get => _price; set => _price = value; }
        public int MinPeople { get => _minPeople; set => _minPeople = value; }

        #endregion

        #region Constructor
        public MainViewModel(ParkModel model)
        {
            _model = model;
            _model.GameAdvanced += new EventHandler<ParkEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<ParkEventArgs>(Model_GameOver);

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            ExitGameCommand = new DelegateCommand(param => OnExitGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            StartStopCommand = new DelegateCommand(param => OnStartStop());
            GameSpeedUpCommand = new DelegateCommand(param => OnGameSpeedUp());
            GameSpeedDownCommand = new DelegateCommand(param => OnGameSpeedDown());
            StartCampaignCommand = new DelegateCommand(param => OnStartCampaign());
            ChangeBuildingModeCommand = new DelegateCommand(param => OnChangeBuildingModeCommand());
            OpenParkCommand = new DelegateCommand(_ => OpenPark());
            StartMovingCommand = new DelegateCommand(_ => StopCleaners());
            SetPriceCommand = new DelegateCommand(_ => SetThePrice());
            SetMinNumOfPeopleCommand = new DelegateCommand(_ => SetMinPeople());

            Build = "Start building";
            int _tableWidth = 800;
            int _tableHeight = 600;
            GameFieldSizeV = _tableWidth / _model.Persistence.ColSize;

            _allEntities = new ObservableCollection<EntityItem>();
            _roads = new List<(Point, RoadType)>();

            SetBaseValuesPropertiesList();
            AreasInTabItem = new ObservableCollection<AreaItem>();

            CreateTable();
            CreateTabMenu();
        }

        private void SetMinPeople()
        {
            if (_clickedField == null)
            {
                return;
            }
            if (_clickedField.Area.GetType().Name == "Game")
            {
                dynamic field = _clickedField.Area;
                field.MinNumOfPeople= _minPeople < 1 ? 1 : _minPeople;
                OnPropertyChanged("CurrentShownProperties");
            }
        }

        private void SetThePrice()
        {
            if (_clickedField == null)
            {
                return;
            }
            if (_clickedField.Area.GetType().Name == "Game" 
                || _clickedField.Area.GetType().Name == "Restaurant"
                || _clickedField.Area.GetType().Name == "WC")
            {
                dynamic field = _clickedField.Area;
                field.Price = _price < 1 ? 1 : _price;
                OnPropertyChanged("CurrentShownProperties");
            }
        }

        #endregion

        #region Public methods
        public void RefreshTable()
        {
            OnPropertyChanged("PlayerMoney");
            OnPropertyChanged("CurrentShownProperties");
            OnPropertyChanged("CustomersCount");
            OnPropertyChanged("SatisfactionLevel");

            foreach (GameField gameField in GameFields)
            {
                GameField updatedGameField = gameField;
                SetBaseValuesPropertiesList();
                string imgPath = "";
                SetGameFieldsCommonProperties(ref updatedGameField, ref imgPath);
                UpdateAreaSpecificProperties(ref updatedGameField, imgPath);
            }
        }

        #endregion

        #region Private methods

        private void StopCleaners()
        {
            _model.StopOrStartAllCleaners(StaffState.STOP, true);
        }

        private void SetBaseValuesPropertiesList()
        {
            _properties = new ObservableCollection<Tuple<String, bool, String>>
            {
                new Tuple<string, bool, String>("State",false, ""), // 0
                new Tuple<string, bool, String>("Price",false, ""), // 1
                new Tuple<string, bool, String>("CommonCost",false, ""), // 2
                new Tuple<string, bool, String>("BuildingTime",false, ""), // 3
                new Tuple<string, bool, String>("BuildPrice",false, ""), // 4
                new Tuple<string, bool, String>("Capacity",false, ""), // 5
                new Tuple<string, bool, String>("ServingTime",false, ""), // 6
                new Tuple<string, bool, String>("RepairTime",false, ""), // 7
                new Tuple<string, bool, String>("MinNumOfPeople",false, ""), // 8
                new Tuple<string, bool, String>("GameTime",false, ""), // 9
                new Tuple<string, bool, String>("RecommendedAdrenalinLevel",false, ""), // 10
                new Tuple<string, bool, String>("CustomersInQueueAmount",false, ""), // 11
                new Tuple<string, bool, String>("IncrementLevel",false, ""), // 12
                new Tuple<string, bool, String>("ReductionLevel",false, ""), // 13
                new Tuple<string, bool, String>("TrashAmount ",false, ""), // 14
                new Tuple<string, bool, String>("Range",false, ""), // 15
                new Tuple<string, bool, String>("CustomersInGameAmount",false, ""), // 16
                new Tuple<string, bool, String>("CustomersInRestaurantAmount",false, ""), // 17
            };
        }

        private void SetColorsAndUsedPropertiesForAreas(string areaType, ref string color, ref List<int> _whichProperties)
        {
            switch (areaType)
            {
                case "EmptyField":
                    color = "#311b92"; break;
                case "Game":
                    _whichProperties = new List<int> { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 16 };
                    color = "LightSeaGreen"; break;
                case "Restaurant":
                    _whichProperties = new List<int> { 0, 1, 2, 3, 4, 5, 6, 11, 13, 17 };
                    color = "Plum"; break;
                case "Road":
                    _whichProperties = new List<int> { 4, 14 };
                    color = "Gray"; break;
                case "Staff":
                    color = "LightGreen"; break;
                case "Plant":
                    _whichProperties = new List<int> { 4, 15 };
                    color = "PeachPuff"; break;
                case "Bin":
                    _whichProperties = new List<int> { 4 };
                    color = "PeachPuff"; break;
                case "WC":
                    _whichProperties = new List<int> { 1, 2, 4 };
                    color = "PeachPuff"; break;
                case "PowerSource":
                    _whichProperties = new List<int> { 4, 15 };
                    color = "PeachPuff"; break;
            }
        }

        private void CreateTable()
        {
            GameFields = new ObservableCollection<GameField>();
            for (int i = 0; i < _model.Persistence.RowSize; i++)
            {
                for (int j = 0; j < _model.Persistence.ColSize; j++)
                {
                    List<int> _whichProperties = new List<int> { };
                    string areaType = _model.Persistence.GetField(i, j).Area.GetType().Name;
                    string color = "";
                    string basePath = "Images";
                    string ext = ".png";
                    string imgPath = areaType == "Road" ? basePath + "/sidewalk" + ext : basePath + "/empty" + ext;

                    if (areaType == "Road")
                    {
                        _roads.Add((new Point(i, j), (RoadType)_model.Persistence.GetField(i, j).Area.EnumType));
                    }

                    SetColorsAndUsedPropertiesForAreas(areaType, ref color, ref _whichProperties);

                    foreach (var item in _whichProperties)
                        _properties[item] = new Tuple<string, bool, string>(_properties[item].Item1, true, _properties[item].Item3);

                    GameField newGameField = new GameField
                    {
                        Field = _model.Persistence.GetField(i, j),
                        EnumType = _model.Persistence.GetField(i, j).Area.EnumType.ToString(),
                        Color = color,
                        Position = new Point(i, j),
                        GameFieldSize = GameFieldSizeV,
                        StepCommand = new DelegateCommand(param => StepGame((Point)param)),
                        Properties = _properties,
                        ImageBrush = new ImageBrush(),
                        Image = new BitmapImage(new Uri(imgPath, UriKind.Relative)),
                };
                    SetBaseValueForAreaSpecificProperties(ref newGameField);
                    UpdateAreaSpecificProperties(ref newGameField, imgPath);
                    GameFields.Add(newGameField);
                }
            }
        }

        private void CreateTabMenu()
        {
            List<String> _itemNames = new List<string> { "Games", "Restaurants", "Pavements", "Staff", "Others" };

            TabItems = new ObservableCollection<TabItem>();
            foreach (String name in _itemNames)
            {
                TabItems.Add(new TabItem
                {
                    Name = name,
                    ClickCommand = new DelegateCommand(param => ShowTabItems(param))
                });
            }

            Areas = new ObservableCollection<IArea>();
            _areas.Add(new Game(GameType.DODGEM));
            _areas.Add(new Game(GameType.ROLLER_COASTER));
            _areas.Add(new Game(GameType.BUNGEE_JUMPING));
            _areas.Add(new Restaurant(RestaurantType.FLAMING));
            _areas.Add(new Restaurant(RestaurantType.HOT_DOG_STAND));
            _areas.Add(new Restaurant(RestaurantType.ICE_CREAM_SHOP));
            _areas.Add(new Road(RoadType.SIDEWALK));
            _areas.Add(new Road(RoadType.BUBBLE));
            _areas.Add(new Staff(StaffType.CLEANER));
            _areas.Add(new Staff(StaffType.MAINTENANCE));
            _areas.Add(new Plant(PlantType.CORAL));
            _areas.Add(new Plant(PlantType.SEAWEED));
            _areas.Add(new Bin());
            _areas.Add(new WC());
            _areas.Add(new PowerSource());
        }

        private void SetupAllEntitiesList()
        {
            AllEntities.Clear();
            foreach (var item in _model.Persistence.GetEntities())
            {
                Entity entity = item.Key;
                Point currentPocation = new Point(item.Value.X, item.Value.Y);
                EntityItem newEntityItem = new EntityItem
                {
                    Id = _nextEntityId + "",
                    Type = entity.Type == EntityType.HUMAN ? "Human" : "Shark",
                    State = entity.State.ToString().ToLower().Replace('_', ' '),
                    Position = "(" + item.Value.X + "," + item.Value.Y + ")",
                    Money = entity.Money + "",
                    Happiness = entity.Happiness + "",
                    Hunger = entity.Hunger + "",
                    WcUrge = entity.WcUrge + "",
                    HasCoupon = entity.HasCoupon == true ? "Igen" : "Nem",
                    HasTrash = entity.HasTrash == true ? "Igen" : "Nem"
                };
                _nextEntityId++;
                AllEntities.Add(newEntityItem);
            }
            OnPropertyChanged("AllEntities");
        }

        private void OpenPark()
        {
            if (!_model.ParkIsOpen)
            {
                ParkIsOpened?.Invoke(this, EventArgs.Empty);
                _model.ParkIsOpen = true;
            }
        }

        private void OnChangeBuildingModeCommand()
        {
            if (_build == "Start building") _build = "Stop building";
            else _build = "Start building";
            OnPropertyChanged("Build");
        }

        private void ShowTabItems(object param)
        {
            AreasInTabItem = new ObservableCollection<AreaItem>();
            List<Type> types = new List<Type>();
            switch (param)
            {
                case "Games": types.Add(typeof(Game)); break;
                case "Restaurants": types.Add(typeof(Restaurant)); break;
                case "Pavements": types.Add(typeof(Road)); break;
                case "Staff": types.Add(typeof(Staff)); break;
                case "Others":
                    types.Add(typeof(Bin));
                    types.Add(typeof(Plant));
                    types.Add(typeof(WC));
                    types.Add(typeof(PowerSource));
                    break;
            }

            SetShownItems(param, types);
            OnPropertyChanged("AreasInTabItem");
            OnPropertyChanged("TabItemWidth");
        }

        private void SetShownItems(object param, List<Type> types)
        {
            foreach (IArea area in _areas)
            {
                string color = "Aqua";

                switch (param)
                {
                    case "Games": color = "LightSeaGreen"; break;
                    case "Restaurants": color = "Plum"; break;
                    case "Pavements": color = "Gray"; break;
                    case "Staff": color = "LightGreen"; break;
                    case "Others": color = "PeachPuff"; break;
                }

                string imgPath = "";
                switch (area.EnumType.ToString())
                {
                    case "DODGEM": imgPath = "Images/dodge.png"; break;
                    case "ROLLER_COASTER": imgPath = "Images/circleswing.png"; break;
                    case "BUNGEE_JUMPING": imgPath = "Images/bungee.png"; break;
                    case "FLAMING": imgPath = "Images/flaming.png"; break;
                    case "HOT_DOG_STAND": imgPath = "Images/hotfish.png"; break;
                    case "ICE_CREAM_SHOP": imgPath = "Images/smoothie.png"; break;
                    case "SIDEWALK": imgPath = "Images/sidewalk.png"; break;
                    case "BUBBLE": imgPath = "Images/bubble.png"; break;
                    case "CORAL": imgPath = "Images/coral.png"; break;
                    case "SEAWEED": imgPath = "Images/seaweed.png"; break;
                    case "CLEANER": imgPath = "Images/person1.png"; break;
                    case "MAINTENANCE": imgPath = "Images/person2.png"; break;
                    case "BIN": imgPath = "Images/bin.png"; break;
                    case "WC": imgPath = "Images/wc.png"; break;
                    case "POWERSOURCE": imgPath = "Images/electric.png"; break;
                }

                if (types.Contains(area.GetType()))
                    _areasInTabItem.Add(new AreaItem
                    {
                        Area = area,
                        Name = area.GetType().Name,
                        Type = area.EnumType.ToString(),
                        Color = color,
                        AreaImageBrush = new ImageBrush(),
                        AreaImage = new BitmapImage(new Uri(imgPath, UriKind.Relative)),
                        Price = area.BuildPrice + "",
                        ItemsWidthInTabItem = 60,
                        ChooseItemCommand = new DelegateCommand(param => ShowAreaItems(param))
                    });
            }
        }

        private void ShowAreaItems(object param)
        {
            foreach (AreaItem areaItem in _areasInTabItem)
                if (areaItem.Type.Equals(param))
                    _selectedArea = areaItem;
        }

        private void SetGameFieldsCommonProperties(ref GameField gameField, ref string imgPath)
        {
            gameField.Field = _model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y);
            gameField.EnumType = gameField.Field.Area.EnumType.ToString();
            gameField.EntitiesCount = _model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Entities.Count();
            gameField.StaffCount = _model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Staffs.Count();

            string color = "";
            switch (_model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Area.EnumType.ToString())
            {
                case "DODGEM": imgPath = ImagePathForGame(gameField, "dodge"); break;
                case "ROLLER_COASTER": imgPath = ImagePathForGame(gameField, "circleswing"); break;
                case "BUNGEE_JUMPING": imgPath = ImagePathForGame(gameField, "bungee"); break;
                case "FLAMING": imgPath = ImagePathForRestaurant(gameField, "flaming"); break;
                case "HOT_DOG_STAND": imgPath = ImagePathForRestaurant(gameField, "hotfish"); break;
                case "ICE_CREAM_SHOP": imgPath = ImagePathForRestaurant(gameField, "smoothie"); break;
                case "SIDEWALK": imgPath = ImagePathForRoad(gameField); break;
                case "BUBBLE": imgPath = ImagePathForRoad(gameField); break;
                case "CORAL": imgPath = "Images/coral.png"; break;
                case "SEAWEED": imgPath = "Images/seaweed.png"; break;
                case "CLEANER": imgPath = ImagePathForRoad(gameField); break;
                case "MAINTENANCE": imgPath = ImagePathForRoad(gameField); break;
                case "BIN": imgPath = "Images/bin.png"; break;
                case "WC": imgPath = "Images/wc.png"; break;
                case "POWERSOURCE": imgPath = "Images/electric.png"; break;
                case "NONE": imgPath = "Images/empty.png"; break;
            }

            // update area specific properties
            List<int> _whichProperties = new List<int> { };
            String areaType = _model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Area.GetType().Name;
            SetColorsAndUsedPropertiesForAreas(areaType, ref color, ref _whichProperties);
            gameField.Color = color;
            gameField.Properties = SetUsedProperties(_whichProperties, gameField);
        }

        private string ImagePathForGame(GameField gameField, string gameType)
        {
            Game game = (Game)_model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Area;
            string imagePath;
            string basePath = "Images/Image_variables/buildings/";
            switch (game.State)
            {
                case GameState.BUILDING: imagePath = basePath + "build/" + gameType + "-build.png"; break;
                case GameState.WAITING: imagePath = basePath + "wait/" + gameType + "-wait.png"; break;
                case GameState.BROKEN: imagePath = basePath + "broke/" + gameType + "-broke.png"; break;
                case GameState.REPAIRING: imagePath = basePath + "fix/" + gameType + "-fix.png"; break;
                case GameState.NO_POWER: imagePath = basePath + "nopower/" + gameType + "-nopower.png"; break;
                case GameState.WORKING: imagePath = "Images/" + gameType + ".png"; break;
                default: imagePath = "Images/" + gameType + ".png"; break;
            }
            return imagePath;
        }

        private string ImagePathForRestaurant(GameField gameField, string restaurantType)
        {
            Restaurant restaurant = (Restaurant)_model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Area;
            string imagePath = "";
            string basePath = "Images/Image_variables/buildings/";
            switch (restaurant.State)
            {
                case RestaurantState.BUILDING: imagePath = basePath + "build/" + restaurantType + "-build.png"; break;
                case RestaurantState.WAITING: imagePath = basePath + "wait/" + restaurantType + "-wait.png"; break;
                case RestaurantState.NO_POWER: imagePath = basePath + "nopower/" + restaurantType + "-nopower.png"; break;
                case RestaurantState.WORKING: imagePath = "Images/" + restaurantType + ".png"; break;
                default: imagePath = "Images/" + restaurantType + ".png"; break;
            }
            return imagePath;
        }

        private string ImagePathForRoad(GameField gameField)
        {
            int allCreaturesCount = 0;
            string imagePath = "Images/empty.png";
            string basePath = "Images/Image_variables/";
            dynamic roadOrStaff = _model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Area;
            RoadType currentRoadType = (RoadType)gameField.Field.Area.EnumType;
            string roadType = currentRoadType == RoadType.SIDEWALK ? "sidewalk" : "bubble";
            List<string> entitiesType = _model.Persistence.GetField((int)gameField.Position.X,
                (int)gameField.Position.Y).Entities.Select(e => e.Type.ToString()).ToList();
            int humanCounts = GetEntityCounts(entitiesType, "HUMAN", ref allCreaturesCount);
            int sharkCounts = GetEntityCounts(entitiesType, "SHARK", ref allCreaturesCount);
            List<string> staffTypes = _model.Persistence.GetField((int)gameField.Position.X,
                (int)gameField.Position.Y).Staffs.Select(e => e.Type.ToString()).ToList();
            int cleanerCounts = GetEntityCounts(staffTypes, "CLEANER", ref allCreaturesCount);
            int maintenanceCounts = GetEntityCounts(staffTypes, "MAINTENANCE", ref allCreaturesCount);

            if (allCreaturesCount == 0) // üres út vagy bubble 
            {
                if (gameField.TrashAmount == "" || gameField.TrashAmount == "0")
                {
                    imagePath = "Images/" + roadType + ".png";
                }
                else
                {
                    imagePath = basePath + "buildings/trash/" + roadType + ".png";
                }
            }
            else if (cleanerCounts + maintenanceCounts == 0) // csak baloldalt vannak
            {
                if (humanCounts != 0 && sharkCounts == 0) // 1 vagy 2 ember
                    imagePath = basePath + roadType + "-" + humanCounts + "human.png";
                else if (humanCounts == 0 && sharkCounts != 0) // 1 vagy 2 cápa
                    imagePath = basePath + roadType + "-" + sharkCounts + "shark.png";
                else if (humanCounts != 0 && sharkCounts != 0 && allCreaturesCount == 2) // 1-1 mindkettőből
                    imagePath = basePath + roadType + "-1human-1shark.png";
                else if (humanCounts != 0 && sharkCounts != 0 && allCreaturesCount > 2) // több mint 3 összesen
                    imagePath = basePath + roadType + "-2human-1shark.png";
                else
                    imagePath = basePath + roadType + "-2human-1shark.png";
            }
            else if (humanCounts + sharkCounts == 0) // csak jobboldalt vannak
            {
                if (cleanerCounts != 0 && maintenanceCounts == 0) // 1 vagy 2 takarító
                    imagePath = basePath + roadType + "-" + cleanerCounts + "cleaner.png";
                else if (cleanerCounts == 0 && maintenanceCounts != 0) // 1 vagy 2 karbantartó
                    imagePath = basePath + roadType + "-" + maintenanceCounts + "maintenance.png";
                else if (cleanerCounts != 0 && maintenanceCounts != 0 && allCreaturesCount > 1) // 2 vagy több különböző összesen
                    imagePath = basePath + roadType + "-1cleaner-1maintenance.png";
                else
                    imagePath = basePath + roadType + "-1cleaner-1maintenance.png";
            }
            else // mindkét oldalon vannak
            {
                if (sharkCounts == 0 && maintenanceCounts == 0)
                    imagePath = basePath + roadType + "-1human-1cleaner.png";
                else if (sharkCounts == 0 && cleanerCounts == 0)
                    imagePath = basePath + roadType + "-1human-1maintenance.png";
                else if (humanCounts == 0 && maintenanceCounts == 0)
                    imagePath = basePath + roadType + "-1shark-1cleaner.png";
                else if (humanCounts == 0 && cleanerCounts == 0)
                    imagePath = basePath + roadType + "-1shark-1maintenance.png";
                else if (maintenanceCounts == 0 && cleanerCounts == 1)
                    imagePath = basePath + roadType + "-1human-1shark-1cleaner.png";
                else if (maintenanceCounts == 0 && cleanerCounts > 1)
                    imagePath = basePath + roadType + "-1human-1shark-2cleaner.png";
                else if (cleanerCounts == 0 && maintenanceCounts == 1)
                    imagePath = basePath + roadType + "-1human-1shark-1maintenance.png";
                else if (cleanerCounts == 0 && maintenanceCounts > 1)
                    imagePath = basePath + roadType + "-1human-1shark-2maintenance.png";
                else
                    imagePath = basePath + roadType + "-1human-1shark-1cleaner-1maintenance.png";
            }
            return imagePath;
        }

        private int GetEntityCounts(List<string> entityList, string entityType, ref int allCreatersCount)
        {
            int entityCounts = entityList.Where(i => i == entityType).Count();
            entityCounts = entityCounts > 2 ? 2 : entityCounts;
            allCreatersCount += entityCounts;
            return entityCounts;
        }

        private ObservableCollection<Tuple<String, bool, String>> SetUsedProperties(List<int> _whichProperties, GameField gameField)
        {
            foreach (var itemIdx in _whichProperties)
            {
                String propertyName = _properties[itemIdx].Item1;
                bool show = true;
                String propertyValue = gameField.GetType().GetProperty(propertyName)?.GetValue(gameField, null) as String;
                _properties[itemIdx] = new Tuple<string, bool, string>(propertyName, show, propertyValue);
            }

            ObservableCollection<Tuple<String, bool, String>> usedProps = new ObservableCollection<Tuple<string, bool, string>>();
            foreach (var prop in _properties)
            {
                if (prop.Item2)
                    usedProps.Add(prop);
            }

            return usedProps;
        }

        private void SetBaseValueForAreaSpecificProperties(ref GameField gameField)
        {
            gameField.State = "";
            gameField.Price = "";
            gameField.CommonCost = "";
            gameField.BuildingTime = "";
            gameField.BuildPrice = "";
            gameField.Capacity = "";
            gameField.ServingTime = "";
            gameField.RepairTime = "";
            gameField.MinNumOfPeople = "";
            gameField.GameTime = "";
            gameField.RecommendedAdrenalinLevel = "";
            gameField.CustomersInQueueAmount = "";
            gameField.IncrementLevel = "";
            gameField.ReductionLevel = "";
            gameField.TrashAmount = "";
            gameField.Range = "";
            gameField.CustomersInGameAmount = "";
            gameField.CustomersInRestaurantAmount = "";
        }

        private void UpdateAreaSpecificProperties(ref GameField gameField, string imgPath)
        {
            OnPropertyChanged("PlayerMoney");
            OnPropertyChanged("CurrentShownProperties");
            OnPropertyChanged("CustomersCount");
            OnPropertyChanged("SatisfactionLevel");
            SetupAllEntitiesList();

            dynamic building = _model.Persistence.GetField((int)gameField.Position.X, (int)gameField.Position.Y).Area;
            Type type = building.GetType();
            gameField.State = HasThisProperty(type, "State") ? building.State.ToString() : "";
            gameField.Price = HasThisProperty(type, "Price") ? building.Price.ToString() : "";
            gameField.CommonCost = HasThisProperty(type, "CommonCost") ? building.CommonCost.ToString() : "";
            gameField.BuildingTime = HasThisProperty(type, "BuildingTime") ? building.BuildingTime.ToString() : "";
            gameField.BuildPrice = HasThisProperty(type, "BuildPrice") ? building.BuildPrice.ToString() : "";
            gameField.Capacity = HasThisProperty(type, "Capacity") ? building.Capacity.ToString() : "";
            gameField.ServingTime = HasThisProperty(type, "ServingTime") ? building.ServingTime.ToString() : "";
            gameField.RepairTime = HasThisProperty(type, "RepairTime") ? building.RepairTime.ToString() : "";
            gameField.MinNumOfPeople = HasThisProperty(type, "MinNumOfPeople") ? building.MinNumOfPeople.ToString() : "";
            gameField.GameTime = HasThisProperty(type, "GameTime") ? building.GameTime.ToString() : "";
            gameField.RecommendedAdrenalinLevel = HasThisProperty(type, "RecommendedAdrenalinLevel") ? building.RecommendedAdrenalinLevel.ToString() : "";
            gameField.CustomersInQueueAmount = HasThisProperty(type, "CustomersInQueueAmount") ? building.CustomersInQueueAmount.ToString() : "";
            gameField.IncrementLevel = HasThisProperty(type, "IncrementLevel") ? building.IncrementLevel.ToString() : "";
            gameField.ReductionLevel = HasThisProperty(type, "ReductionLevel") ? building.ReductionLevel.ToString() : "";
            gameField.TrashAmount = HasThisProperty(type, "TrashAmount") ? building.TrashAmount.ToString() : "";
            gameField.Range = HasThisProperty(type, "Range") ? building.Range.ToString() : "";
            gameField.CustomersInGameAmount = HasThisProperty(type, "CustomersInGameAmount") ? building.CustomersInGameAmount.ToString() : "";
            gameField.CustomersInRestaurantAmount = HasThisProperty(type, "CustomersInRestaurantAmount") ? building.CustomersInRestaurantAmount.ToString() : "";
            gameField.ImageBrush = new ImageBrush();
            gameField.Image = new BitmapImage(new Uri(imgPath, UriKind.Relative));
        }

        private bool HasThisProperty(Type type, string propertyName)
        {
            foreach (var item in type.GetProperties())
            {
                if (item.Name.Equals(propertyName))
                    return true;
            }
            return false;
        }

        private void StepGame(Point param)
        {
            if (_build == "Stop building")
            {
                if (_selectedArea != null)
                {
                    BuildTheSelectedBuilding(param);
                }
            }
            else // if we clicked an object on the table and we don't want to build
            {
                if (_model.ParkIsOpen)
                {
                    _clickedField = _model.Persistence.GetField((int)param.X, (int)param.Y);

                    if (_clickedField.Area.GetType().Name != "EmptyField")
                    {
                        foreach (var gameField in _gameFields)
                            if (gameField.Position == param)
                                CurrentShownProperties = gameField.Properties; // set the appropiate properties visibality
                    }

                // images
                // ObservableCollection<Tuple<String, bool, String>> usedProps = new ObservableCollection<Tuple<string, bool, string>>();
                // foreach (var prop in _properties)
                // {
                //     if (prop.Item2)
                //
                // cleancode/methods
                    if (_clickedField.Area.GetType().Name == "Road")
                    {
                        ClickedOnFieldInStep(_clickedField, param);
                    }
                }
            }
        }

        private void ClickedOnFieldInStep(Field clickedField, Point param)
        {
            if (_selectedCleaner != null)
            {
                _model.StopOrStartAllCleaners(StaffState.AUTONOMOUS_WALKING, false);
                _model.SetTransferedCleaner(_selectedCleaner, (int)param.X, (int)param.Y);
                _selectedCleaner = null;
            }

            if (clickedField.Staffs.Count != 0 && _model.CleanersStopped && _selectedCleaner == null)
            {
                _selectedCleaner = clickedField.Staffs[0];
                _selectedCleanerPos = clickedField;
            }
        }

        private void BuildTheSelectedBuilding(Point param)
        {
            IArea area = null;
            switch (_selectedArea.Name)
            {
                case "Game": area = new Game((GameType)_selectedArea.Area.EnumType); break;
                case "Restaurant": area = new Restaurant((RestaurantType)_selectedArea.Area.EnumType); break;
                case "Road": area = new Road((RoadType)_selectedArea.Area.EnumType); break;
                case "Staff": area = new Staff((StaffType)_selectedArea.Area.EnumType); break;
                case "Plant": area = new Plant((PlantType)_selectedArea.Area.EnumType); break;
                case "Bin": area = new Bin(); break;
                case "WC": area = new WC(); break;
                case "PowerSource": area = new PowerSource(); break;
            }

            if (area != null)
            {
                int price = _model.Step((int)param.X, (int)param.Y, area);
                if (price != 0)
                {
                    if (area.GetType().ToString() == "Road")
                    {
                        _roads.Add((new Point((int)param.X, (int)param.Y), (RoadType)_model.Persistence.GetField((int)param.X, (int)param.Y).Area.EnumType));
                    }

                    PlayerMoney -= price;
                    RefreshTable();
                }
            }
        }

        #endregion

        #region Model methods

        private void Model_GameOver(object sender, ParkEventArgs e)
        {
            OnPropertyChanged("GameTime");
            RefreshTable();
            GameEnd?.Invoke(this, EventArgs.Empty);
        }

        private void Model_GameAdvanced(object sender, ParkEventArgs e)
        {
            OnPropertyChanged("GameTime");
            RefreshTable();
        }

        private void OnStartStop()
        {
            if (StartStop != null)
                StartStop(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            if (LoadGame != null)
                LoadGame(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }
        private void OnNewGame()
        {
            if (NewGame != null)
                NewGame(this, EventArgs.Empty);
        }

        private void OnGameSpeedUp()
        {
            if (GameSpeedUp != null)
                GameSpeedUp(this, EventArgs.Empty);
        }
        private void OnGameSpeedDown()
        {
            if (GameSpeedDown != null)
                GameSpeedDown(this, EventArgs.Empty);
        }
        private void OnStartCampaign()
        {
            if (StartCampaign != null)
                StartCampaign(this, EventArgs.Empty);
        }

        #endregion
    }
}
