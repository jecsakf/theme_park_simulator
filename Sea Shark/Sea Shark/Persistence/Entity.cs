using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public enum EntityState { NOTHING_TO_DO, ON_THE_WAY, WAITING_IN_LINE, DO_SOMETHING }

    public class Entity
    {
        #region Constants

        public const int RADIUS = 5;
        public const int MIN_HAPPINESS = 0;
        public const int MIN_START_VALUE = 80;
        public const int MAX_HAPPINESS = 100;
        public const int MIN_MONEY = 100;
        public const int MAX_MONEY = 1000;
        public const int MIN_HUNGER = 0;
        public const int MAX_HUNGER = 100;
        public const int MIN_WCURGE = 0;
        public const int MAX_WCURGE = 100;
        public const int MIN_ADRENALINE = 0;
        public const int MAX_ADRENALINE = 100;

        #endregion

        #region Private data
        private EntityState _state;
        private EntityType _type;
        private List<Field> _pathToAim;
        private int _happiness;
        private int _money;
        private int _hunger;
        private int _wcUrge;
        private int _adrenaline;
        private bool _hasCoupon;
        private bool _hasTrash;
        private Field _aim;
        private bool _isStay;
        private IArea _lastVisitedArea;
        private int _radius;
        #endregion

        #region Constructor
        public Entity(EntityType type, bool coupon, int parkEntryFee)
        {
            _type = type;
            Random random = new Random();
            _happiness = random.Next(MIN_START_VALUE, MAX_HAPPINESS);
            _money = random.Next(MIN_MONEY, MAX_MONEY);
            _hunger = random.Next(MIN_START_VALUE, MAX_HUNGER);
            _wcUrge = random.Next(MIN_START_VALUE, MAX_WCURGE);
            _adrenaline = random.Next(MIN_ADRENALINE, MAX_ADRENALINE);
            _hasCoupon = coupon;
            _hasTrash = false;
            _radius = RADIUS;

            int chance = parkEntryFee < 15 ? 100 : (parkEntryFee < 60 ? 70 : 30);
            int percent = new Random().Next(1, 101);
            _isStay = percent <= chance ? true : false;
        }
        #endregion

        #region Properties
        public EntityType Type { get => _type; }
        public EntityState State { get => _state; set => _state = value; }
        public List<Field> PathToAim { get => _pathToAim; set => _pathToAim = value; }
        public int Happiness { get => _happiness; set => _happiness = value; }
        public int Money { get => _money; set => _money = value; }
        public int Hunger { get => _hunger; set => _hunger = value; }
        public int WcUrge { get => _wcUrge; set => _wcUrge = value; }
        public int Adrenalin { get => _adrenaline; set => _adrenaline = value; }
        public bool HasCoupon { get => _hasCoupon; set => _hasCoupon = value; }
        public bool HasTrash { get => _hasTrash; set => _hasTrash = value; }
        public Field Aim { get => _aim; set => _aim = value; }
        public bool WannaGoHome { get => _money < 0 || _happiness < 0 || !_isStay; }
        public IArea LastVisitedArea { get => _lastVisitedArea; set => _lastVisitedArea = value; }
        #endregion

        private int NeedsBetweenMinAndMax(int e)
        {
            if (e > 100)
                e = 100;
            else if (e < 0)
                e = 0;
            return e;
        }
        public bool HasEnoughMoney(int h)
        {
            if (_money - h > 0)
            {
                _money -= h;
                return true;
            }
            return false;
        }
        public void AddHappiness(int h)
        {
            _happiness += h;
            _happiness = NeedsBetweenMinAndMax(_happiness);
        }
        public void ReduceHappiness(int h)
        {
            _happiness -= h;
            _happiness = NeedsBetweenMinAndMax(_happiness);
        }
        public void AddHunger(int h)
        {
            _hunger += h;
            _hunger = NeedsBetweenMinAndMax(_hunger);
        }
        public void ReduceHunger(int h)
        {
            _hunger -= h;
            _hunger = NeedsBetweenMinAndMax(_hunger);
        }
        public void AddWcUrge(int wc)
        {
            _wcUrge += wc;
            _wcUrge = NeedsBetweenMinAndMax(_wcUrge);
        }
        public void ReduceWcUrge(int wc)
        {
            _wcUrge -= wc;
            _wcUrge = NeedsBetweenMinAndMax(_wcUrge);
        }
        public int GetAdrenaline()
        {
            return _adrenaline;
        }
        public bool GetCoupon()// Added to the if the entity got a coupon
        {
            return _hasCoupon;
        }
        public void UseCoupon()
        {
            _hasCoupon = false;
        }

        public bool ThrowTrashToBin(Field[,] fields)
        {
            //if there is bin within the radius
            Field entityPosition = fields.Cast<Field>().ToList().Where(f => f.Entities.Contains(this)).FirstOrDefault();
            List<Field> bins = fields.Cast<Field>().ToList().Where(a => a.Area.EnumType.ToString() == OtherType.BIN.ToString()).ToList();

            foreach (var bin in bins)
            {
                if ((Math.Abs(bin.Row - entityPosition.Row) + Math.Abs(bin.Col - entityPosition.Col)) <= _radius)
                {
                    return true;
                }
            }

            //if not throw it to the road
            return false;
        }

        public IArea ChooseAim(ICollection<IArea> areas, Field[,] fields, Point entityCurrentPosition, bool parkIsOpen, bool parkIsDirty)
        {
            List<IArea> emptyTypeAreas = areas.Where(a => a.GetType() == typeof(EmptyField)).Where(a => (OtherType)a.EnumType == OtherType.NONE).ToList();
            List<IArea> powersourceTypeAreas = areas.Where(a => a.GetType() == typeof(PowerSource)).Where(a => (OtherType)a.EnumType == OtherType.POWERSOURCE).ToList();
            List<IArea> binTypeAreas = areas.Where(a => a.GetType() == typeof(OtherType)).Where(a => (OtherType)a.EnumType == OtherType.BIN).ToList();
            List<IArea> roadTypeAreas = areas.Where(a => a.GetType() == typeof(Road)).Where(a => (RoadType)a.EnumType == RoadType.SIDEWALK || (RoadType)a.EnumType == RoadType.BUBBLE).ToList();
            List<IArea> plantTypeAreas = areas.Where(a => a.GetType() == typeof(Plant)).Where(a => (PlantType)a.EnumType == PlantType.CORAL || (PlantType)a.EnumType == PlantType.SEAWEED).ToList();

            // ha nincs étterem, játék, wc és kuka, akkor ne állítsunk be célt
            int sum_of_areas = emptyTypeAreas.Count() + powersourceTypeAreas.Count() + binTypeAreas.Count() + roadTypeAreas.Count() + plantTypeAreas.Count();
            if (sum_of_areas == areas.Count())
            {
                return null;
            }
            else
            {
                return SetAnAim(areas, fields, entityCurrentPosition, parkIsOpen, parkIsDirty);
            }
        }

        private IArea SetAnAim(ICollection<IArea> areas, Field[,] fields, Point entityCurrentPosition, bool parkIsOpen, bool parkIsDirty)
        {
            List<int> values = new List<int>() { _happiness, _hunger, _wcUrge };
            int randValue = values.Min();
            IArea aim = null;

            if (_lastVisitedArea != null && _lastVisitedArea.GetType() == typeof(Restaurant))
                EntityComeOutFromRestaurant(areas, fields, entityCurrentPosition, ref parkIsDirty, ref aim);
            else if (_happiness == randValue)
                TryChoosingGame(areas, parkIsOpen, ref aim);
            else if (_hunger == randValue)
                TryChoosingRestaurant(areas, parkIsOpen, ref aim);
            else
            {
                List<WC> wcareas = areas.Where(a => a.GetType() == typeof(WC)).Select(a => (WC)a).Where(a => a.State != WCState.BUILDING).ToList();
                aim = wcareas.Count != 0 ? wcareas[0] : null;
            }

            return aim;
        }

        private Enum ChooseTypeDependOnExamineValue(int examineValue, List<Enum> possibleBuildingTypes,
                                                    List<Tuple<Enum, int>> buildingTypesWithComparedValue)
        {
            dynamic possibleTypes = possibleBuildingTypes;
            dynamic buildingTypes = buildingTypesWithComparedValue;
            Enum choosenType;

            if (examineValue < 61)
            {
                if (possibleTypes.Contains(buildingTypes[0].Item1))
                    choosenType = buildingTypes[0].Item1;
                else
                    choosenType = possibleTypes[0];
            }
            else if (61 <= examineValue && examineValue < 91)
            {
                if (possibleTypes.Contains(buildingTypes[1].Item1))
                    choosenType = buildingTypes[1].Item1;
                else
                    choosenType = possibleTypes[0];
            }
            else
            {
                if (possibleTypes.Contains(buildingTypes[2].Item1))
                    choosenType = buildingTypes[2].Item1;
                else
                    choosenType = possibleTypes[0];
            }
            return choosenType;
        }

        private Enum ChooseMostAppropiateFromPossibleTypes(List<Enum> possibleBuildingTypes, int examineValue,
                                                           List<Tuple<Enum, int>> buildingTypesWithComparedValue)
        {
            if (possibleBuildingTypes.Count() == 0)
            {
                return null;
            }
            else
            {
                dynamic choosenBuildingType = ChooseTypeDependOnExamineValue(examineValue, possibleBuildingTypes, buildingTypesWithComparedValue);
                return choosenBuildingType;
            }
        }

        private void TryChoosingRestaurant(ICollection<IArea> areas, bool parkIsOpen, ref IArea aim)
        {
            List<(RestaurantType, int)> restaurantPrices = new List<(RestaurantType, int)>()
                                                                { (RestaurantType.FLAMING, Restaurant.FLAMING_PRICE),
                                                                  (RestaurantType.HOT_DOG_STAND, Restaurant.HOT_DOG_STAND_PRICE),
                                                                  (RestaurantType.ICE_CREAM_SHOP, Restaurant.ICE_CREAM_SHOP_PRICE) };
            restaurantPrices.Sort();
            RestaurantType choosenRestaurantType;
            int whichRestaurant = new Random().Next(1, 101);

            List<Restaurant> restaurantAreas;
            if (parkIsOpen)
                restaurantAreas = areas.Where(a => a.GetType() == typeof(Restaurant)).Select(a => (Restaurant)a).Where(a => a.State != RestaurantState.BUILDING).ToList();
            else
                restaurantAreas = areas.Where(a => a.GetType() == typeof(Restaurant)).Select(a => (Restaurant)a).ToList();

            List<RestaurantType> possibleTypes = restaurantAreas.Select(a => (RestaurantType)a.EnumType).ToList();

            List<Enum> possibleTypesAsListEnum = possibleTypes.Select(t => (Enum)t).ToList();
            List<Tuple<Enum, int>> gameTypesAdrenalinAsListEnum = restaurantPrices.Select(t => new Tuple<Enum, int>(t.Item1, t.Item2)).ToList();
            
            var result = ChooseMostAppropiateFromPossibleTypes(possibleTypesAsListEnum, whichRestaurant, gameTypesAdrenalinAsListEnum);
            if(result != null)
            {
                choosenRestaurantType = (RestaurantType)result;
                aim = new Restaurant(choosenRestaurantType);
            }
        }

        private void TryChoosingGame(ICollection<IArea> areas, bool parkIsOpen, ref IArea aim)
        {
            List<Tuple<GameType, int>> gameTypesAdrenalin = new List<Tuple<GameType,int>>()
                                                          { new Tuple<GameType,int>(GameType.DODGEM, Math.Abs(Game.DODGEM_ADRENALIN - _adrenaline)),
                                                            new Tuple<GameType,int>(GameType.ROLLER_COASTER, Math.Abs(Game.ROLLER_COASTER_ADRENALIN - _adrenaline)),
                                                            new Tuple<GameType,int>(GameType.BUNGEE_JUMPING , Math.Abs(Game.BUNGEE_JUMPING_ADRENALIN - _adrenaline)) };

            gameTypesAdrenalin.Sort();

            int whichAdrenalinType = new Random().Next(1, 101);

            List<Game> gameAreas;
            if (parkIsOpen)
                gameAreas = areas.Where(a => a.GetType() == typeof(Game)).Select(a => (Game)a).Where(a => a.State != GameState.BUILDING && a.State != GameState.BROKEN && a.State != GameState.REPAIRING).ToList();
            else
                gameAreas = areas.Where(a => a.GetType() == typeof(Game)).Select(a => (Game)a).ToList();

            List<GameType> possibleTypes = gameAreas.Select(a => (GameType)a.EnumType).ToList();


            List<Enum> possibleTypesAsListEnum = possibleTypes.Select(t => (Enum)t).ToList();
            List<Tuple<Enum,int>> gameTypesAdrenalinAsListEnum = gameTypesAdrenalin.Select(t => new Tuple<Enum, int>(t.Item1, t.Item2)).ToList();
            Enum choosenGameType = ChooseMostAppropiateFromPossibleTypes(possibleTypesAsListEnum, whichAdrenalinType, gameTypesAdrenalinAsListEnum);
            if (choosenGameType == null)
            {
                return;
            }
            aim = new Game((GameType)choosenGameType);
        }

        private void EntityComeOutFromRestaurant(ICollection<IArea> areas, Field[,] fields, Point entityCurrentPosition, ref bool parkIsDirty, ref IArea aim)
        {
            if (ThrowTrashToBin(fields))
            {
                List<Bin> binAreas = areas.Where(a => a.GetType() == typeof(Bin)).Select(a => (Bin)a).ToList();
                aim = binAreas.Count != 0 ? binAreas[0] : null;
            }
            else
            {
                Road entityCP = (Road)fields[entityCurrentPosition.X, entityCurrentPosition.Y].Area;
                entityCP.AddTrash(1);
                parkIsDirty = true;
            }
        }
    }
}
