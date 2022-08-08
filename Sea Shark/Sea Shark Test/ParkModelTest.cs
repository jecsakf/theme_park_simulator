using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sea_Shark.Model;
using Sea_Shark.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Sea_Shark_Test
{
    [TestClass]
    public class ParkModelTest
    {
        private ParkModel _model;
        private Entity _entity;
        private Point _entityPos;
        private int _startPosX;
        private int _startPosY;

        [TestInitialize]
        public void InitModel()
        {
            _model = new ParkModel(9, 9, new ParkFileDataAccess());
            _entity = new Entity(EntityType.HUMAN, false, 0);
            _startPosX = _model.Persistence.StartField.X;
            _startPosY = _model.Persistence.StartField.Y;
            _entityPos = new Point(_startPosX, _startPosY);
            _model.Persistence.GetEntities().Add(_entity,_entityPos);
        }

        [TestMethod]
        public void TestStepRoadPutDown()
        {
            _model.Step(_startPosX, _startPosY, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY - 1, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY - 1).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY - 1).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY + 1, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY + 1).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY + 1).Area, typeof(Road));

            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX - 1, _startPosY).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX - 1, _startPosY).Area, typeof(Road));

            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX - 2, _startPosY).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX - 2, _startPosY).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY - 2, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY - 2).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY - 2).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY - 3, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY - 3).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY - 3).Area, typeof(Road));
        }

        [TestMethod]
        public void TestStepRoadPutOnSame()
        {
            _model.Step(_startPosX, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY - 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 1, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY - 1).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY - 1).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY + 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY + 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY + 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY + 1, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY + 1, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY + 1).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY + 1).Area, typeof(Road));

            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX - 1, _startPosY).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX - 1, _startPosY).Area, typeof(Road));

            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX - 2, _startPosY).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX - 2, _startPosY).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY - 2, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 2, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 2, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 2, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 2, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY - 2).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY - 2).Area, typeof(Road));

            _model.Step(_startPosX, _startPosY - 3, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 3, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 3, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 3, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX, _startPosY - 3, new Road(RoadType.SIDEWALK));
            Assert.IsNotNull(_model.Persistence.GetField(_startPosX, _startPosY - 3).Area);
            Assert.IsInstanceOfType(_model.Persistence.GetField(_startPosX, _startPosY - 3).Area, typeof(Road));

        }

        [TestMethod]
        public void TestChooseAim()
        {
            _model.Step(_startPosX - 0, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 3, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 4, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 5, _startPosY, new Game(GameType.ROLLER_COASTER));
            var gameArea = _model.Persistence.GetField(_startPosX - 5, _startPosY).Area as Game;
            gameArea.State = GameState.WAITING;

            _entity.ReduceHappiness(20);

            IArea aim = _entity.ChooseAim(_model.Persistence.GetAreas(), _model.Persistence.GetFields(), _entityPos, true, false);
            
            Assert.IsNotNull(aim);
            Assert.IsInstanceOfType(aim, typeof(Game));
        }

        [TestMethod]
        public void TestChooseAimAndGoThere()
        {
            _model.Step(_startPosX - 0, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 3, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 4, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 5, _startPosY, new Game(GameType.ROLLER_COASTER));
            _model.Step(_startPosX - 2, _startPosY - 1, new Restaurant(RestaurantType.HOT_DOG_STAND));
            _model.Step(_startPosX - 3, _startPosY + 1, new WC());

            Game gameArea = _model.Persistence.GetField(_startPosX - 5, _startPosY).Area as Game;
            gameArea.State = GameState.WAITING;

            _entity.ReduceHappiness(20);
            Assert.IsNull(_entity.Aim);
            Assert.IsNull(_entity.PathToAim);
            Assert.AreEqual(EntityState.NOTHING_TO_DO, _entity.State);

            _model.ChooseAimAndGoThere(_entity, _entityPos);

            List<Field> pathToAim = new List<Field>()
            {
                _model.Persistence.GetField(_startPosX - 0, _startPosY),
                _model.Persistence.GetField(_startPosX - 1, _startPosY),
                _model.Persistence.GetField(_startPosX - 2, _startPosY),
                _model.Persistence.GetField(_startPosX - 3, _startPosY),
                _model.Persistence.GetField(_startPosX - 4, _startPosY)
            };

            Assert.AreEqual(_model.Persistence.GetField(_startPosX - 5, _startPosY), _entity.Aim);
            Assert.AreEqual(EntityState.ON_THE_WAY, _entity.State);
            for (int i = 0; i < pathToAim.Count(); i++)
            {
                Assert.AreEqual(pathToAim[i], _entity.PathToAim[i]);
            }
        }

        [TestMethod]
        public void TestCampaign()
        {
            Assert.AreEqual(false, _model.CampaignIsStarted);
            Assert.AreEqual(4, _model.SpawnRate);

            _model.StartCampaign();
            Assert.AreEqual(9000, _model.PlayerMoney);
            Assert.AreEqual(true, _model.CampaignIsStarted);
            Assert.AreEqual(30, _model.CampaignTimeLeft);
            Assert.AreEqual(2, _model.SpawnRate);

            _model.ManageCampaign();
            Assert.AreEqual(29, _model.CampaignTimeLeft);

            _model.CampaignTimeLeft = 0;
            _model.ManageCampaign();
            Assert.AreEqual(false, _model.CampaignIsStarted);
            Assert.AreEqual(4, _model.SpawnRate);
        }

        [TestMethod]
        public void TestBuildingsStateTurnToWaitingFromBuilding()
        {
            _model.Step(_startPosX - 0, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 3, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 4, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 4, _startPosY - 1, new Restaurant(RestaurantType.FLAMING));
            _model.Step(_startPosX - 5, _startPosY, new Game(GameType.ROLLER_COASTER));
            var gameArea = _model.Persistence.GetField(_startPosX - 5, _startPosY).Area as Game;
            var restaurantArea = _model.Persistence.GetField(_startPosX - 4, _startPosY - 1).Area as Restaurant;

            Assert.IsNotNull(gameArea);
            Assert.IsNotNull(restaurantArea);

            Assert.AreEqual(GameState.BUILDING, gameArea.State);
            Assert.AreEqual(RestaurantState.BUILDING, restaurantArea.State);
            _model.BuildingsStateTurnToWaitingFromBuilding();
            Assert.AreEqual(GameState.WAITING, gameArea.State);
            Assert.AreEqual(RestaurantState.WAITING, restaurantArea.State);

        }

        [TestMethod]
        public void TestParkIsOpen()
        {
            _model.Step(_startPosX - 0, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 1, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 2, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 3, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 4, _startPosY, new Road(RoadType.SIDEWALK));
            _model.Step(_startPosX - 5, _startPosY, new Game(GameType.ROLLER_COASTER));
            var gameArea = _model.Persistence.GetField(_startPosX - 5, _startPosY).Area as Game;
            Assert.AreEqual(GameState.BUILDING, gameArea.State);
            Assert.AreEqual(false,_model.ParkIsOpen);
            _model.ParkIsOpen = true;
            Assert.AreEqual(GameState.NO_POWER, gameArea.State);
        }
    }
}
