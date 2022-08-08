using Sea_shark.ViewModel;
using Sea_Shark.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sea_Shark.ViewModel
{
    public class GameField : ViewModelBase
    {
        private string _color;
        private Field _field;
        private string _enumType;
        private int _price;
        private int _size;
        private int _entitiesCount;
        private int _staffCount;
        private string _state;
        private string _commonCost;
        private string _buildingTime;
        private string _buildPrice;
        private string _capacity;
        private string _servingTime;
        private string _repairTime;
        private string _minNumOfPeople;
        private string _gameTime;
        private string _recommendedAdrenalinLevel;
        private string _customersInQueue;
        private string _incrementLevel;
        private string _reductionLevel;
        private string _trashAmount;
        private string _range;
        private string _customersInQueueAmount;
        private string _customersInGameAmount;
        private string _customersInRestaurantAmount;
        private ObservableCollection<Tuple<String, bool, String>> _properties;
        private BitmapImage _image;
        private ImageBrush _imageBrush;

        public int EntitiesCount { get => _entitiesCount; set { _entitiesCount = value; OnPropertyChanged(); } }
        public int StaffCount { get => _staffCount; set { _staffCount = value; OnPropertyChanged(); } }
        public string Color { get => _color; set { _color = value; OnPropertyChanged(); } }
        public Point Position
        {
            get => new Point(_field.Row,_field.Col);
            set
               {
                Point point = value;
                _field.Row = (int)point.X;
                _field.Col = (int)point.Y;
                OnPropertyChanged();
            }
        }
        public string Row { get => _field.Row+""; }

        public int GameFieldSize { get => _size; set { _size = value; OnPropertyChanged(); } }
        
        public ObservableCollection<Tuple<String, bool, String>> Properties { get => _properties; set { _properties = value; OnPropertyChanged(); } }
        
        public String EnumType { get => _enumType; set { _enumType = value; OnPropertyChanged(); } }
        
        public Field Field { get => _field; set { _field = value; OnPropertyChanged(); } }
        
        public String State { get => _state; set { _state = value; OnPropertyChanged(); } }
        
        public String Price
        {
            get => _price + "";
            set 
            {
                int _valueAsInt = -1;
                Int32.TryParse(value, out _valueAsInt);
                _price = _valueAsInt != -1 ? _valueAsInt : 0;
                OnPropertyChanged();
            }
        }
        
        public String CommonCost { get => _commonCost; set { _commonCost = value; OnPropertyChanged(); } }
        
        public String BuildingTime { get => _buildingTime; set { _buildingTime = value; OnPropertyChanged(); } }
        
        public String BuildPrice { get => _buildPrice; set { _buildPrice = value; OnPropertyChanged(); } }
        
        public String Capacity { get => _capacity; set { _capacity = value; OnPropertyChanged(); } }
        
        public String ServingTime { get => _servingTime; set { _servingTime = value; OnPropertyChanged(); } }
        
        public String RepairTime { get => _repairTime; set { _repairTime = value; OnPropertyChanged(); } }
        
        public String MinNumOfPeople { get => _minNumOfPeople; set { _minNumOfPeople = value; OnPropertyChanged(); } }
        
        public String GameTime { get => _gameTime; set { _gameTime = value; OnPropertyChanged(); } }
        
        public String RecommendedAdrenalinLevel { get => _recommendedAdrenalinLevel; set { _recommendedAdrenalinLevel = value; OnPropertyChanged(); } }
        
        public String CustomersInQueue { get => _customersInQueue; set { _customersInQueue = value; OnPropertyChanged(); } }

        // game - emotion
        public String IncrementLevel { get => _incrementLevel; set { _incrementLevel = value; OnPropertyChanged(); } }
        
        // wc - urinary urgenciy, restaurant - hunger
        public String ReductionLevel { get => _reductionLevel; set { _reductionLevel = value; OnPropertyChanged(); } }
        
        public String TrashAmount { get => _trashAmount; set { _trashAmount = value; OnPropertyChanged(); } }
        
        public String Range { get => _range; set { _range = value; OnPropertyChanged(); } }
        
        public String CustomersInQueueAmount { get => _customersInQueueAmount; set { _customersInQueueAmount = value; OnPropertyChanged(); } }
        public String CustomersInGameAmount { get => _customersInGameAmount; set { _customersInGameAmount = value; OnPropertyChanged(); } }
        public String CustomersInRestaurantAmount { get => _customersInRestaurantAmount; set { _customersInRestaurantAmount = value; OnPropertyChanged(); } }

        public BitmapImage Image { 
            get => _image; 
            set
            { 
                _image = value; 
                OnPropertyChanged();
                ImageBrush.ImageSource = value;
                OnPropertyChanged("ImageBrush");
            } 
        }
        public ImageBrush ImageBrush {
            get => _imageBrush;
            set
            {
                _imageBrush = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand StepCommand { get; set; }
    }

    public class TabItem
    {
        private string _name;

        public string Name { get => _name; set => _name = value; }
        public DelegateCommand ClickCommand { get; set; }

    }

    public class AreaItem : ViewModelBase
    {
        private IArea _area;
        private string _name;
        private string _type;
        private string _color;
        private string _price;
        private string _choosenObjectName;
        private int _choosenObjectPrice;
        private int _itemsWidthInTabItem;
        private BitmapImage _areaImage;
        private ImageBrush _areaImageBrush;

        public IArea Area { get => _area; set => _area = value; }
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Type { get => _type; set { _type = value; OnPropertyChanged(); } }
        public string Color { get => _color; set { _color = value; OnPropertyChanged(); } }
        public string Price { get => _price+"$"; set { _price = value; OnPropertyChanged(); } }
        public BitmapImage AreaImage
        {
            get => _areaImage;
            set
            {
                _areaImage = value;
                OnPropertyChanged();
                AreaImageBrush.ImageSource = value;
                OnPropertyChanged("AreaImageBrush");
            }
        }
        public ImageBrush AreaImageBrush
        {
            get => _areaImageBrush;
            set
            {
                _areaImageBrush = value;
                OnPropertyChanged();
            }
        }
        public int ItemsWidthInTabItem { get => _itemsWidthInTabItem; set { _itemsWidthInTabItem = value; OnPropertyChanged(); } }

        public String ChoosenObjectName { get => _choosenObjectName; set { _choosenObjectName = value; } }

        public int ChoosenObjectPrice { get => _choosenObjectPrice; set { _choosenObjectPrice = value; OnPropertyChanged(); } }
    
        public DelegateCommand ChooseItemCommand { get; set; }
    }

    public class EntityItem : ViewModelBase
    {
        private string _id;
        private string _type;
        private string _state;
        private string _position;
        private string _money;
        private string _happiness;
        private string _hunger;
        private string _wcUrge;
        private string _hasCoupon;
        private string _hasTrash;

        public string Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Type { get => _type; set { _type = value; OnPropertyChanged(); } }
        public string State { get => _state; set { _state = value; OnPropertyChanged(); } }
        public string Position { get => _position; set { _position = value; OnPropertyChanged(); } }
        public string Money { get => _money; set { _money = value; OnPropertyChanged(); } }
        public string Happiness { get => _happiness; set { _happiness = value; OnPropertyChanged(); } }
        public string Hunger { get => _hunger; set { _hunger = value; OnPropertyChanged(); } }
        public string WcUrge { get => _wcUrge; set { _wcUrge = value; OnPropertyChanged(); } }
        public string HasCoupon { get => _hasCoupon; set { _hasCoupon = value; OnPropertyChanged(); } }
        public string HasTrash { get => _hasTrash; set { _hasTrash = value; OnPropertyChanged(); } }
    }
}