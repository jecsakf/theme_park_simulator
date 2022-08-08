using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sea_Shark.Persistence
{
    public interface IArea
    {
        public int BuildPrice { get; }
        public Enum EnumType { get; set; }
    }

    public class EmptyField : IArea
    {
        private OtherType _type;
        public Enum EnumType { get => _type; set => _type = (OtherType)value; }
        public int BuildPrice { get => 0; }

        public EmptyField(OtherType type)
        {
            _type = type;
        }
    }

    public enum GameType { DODGEM, ROLLER_COASTER, BUNGEE_JUMPING }
    public enum GameState { WORKING, BUILDING, WAITING, BROKEN, REPAIRING, FINISHING_ROUND, NO_POWER }
    public enum RestaurantType { ICE_CREAM_SHOP, FLAMING, HOT_DOG_STAND }
    public enum RestaurantState { WORKING, BUILDING, WAITING, NO_POWER }
    public enum RoadType { SIDEWALK, BUBBLE }
    public enum PlantType { CORAL, SEAWEED }
    public enum StaffType { MAINTENANCE, CLEANER }
    public enum StaffState { NOTHING_TO_DO, AUTONOMOUS_WALKING, ON_THE_WAY, REPAIRING, STOP}
    public enum OtherType { NONE, BIN, POWERSOURCE, WC }
    public enum WCState { BUILDING, WORKING, WAITING}
    public enum EntityType { HUMAN, SHARK }

}
