using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Sea_Shark.Persistence
{
    public class ParkFileDataAccess:IParkFileDataAccess
    {
        public async Task<ParkPersistence> LoadAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                String line = await sr.ReadLineAsync();
                int rowCount = int.Parse(line.Split()[0]);
                int colCount = int.Parse(line.Split()[1]);
                ParkPersistence data = new ParkPersistence(rowCount, colCount);
                data.ParkName = await sr.ReadLineAsync();
                line = await sr.ReadLineAsync();
                data.AvailableMoney = int.Parse(line.Split()[0]);
                data.SatisfactionLevel = int.Parse(line.Split()[1]);
                data.SetEntryFee(int.Parse(line.Split()[2]));
                data.ParkIsOpen = Convert.ToBoolean(line.Split()[3]);
                data.Time = int.Parse(line.Split()[4]);
                data.Areas.Clear();
                Point tmpPoint = new Point();
                for (int i = 0; i < rowCount; i++)
                {
                    line = await sr.ReadLineAsync();
                    for (int j = 0; j < colCount; j++)
                    {
                        tmpPoint.X = i;
                        tmpPoint.Y = j;
                        switch (line.Split()[j].Split(';')[0])
                        {
                            case "Game":
                                Enum.TryParse(line.Split()[j].Split(';')[1], out GameType gameType);
                                data.GetField(i, j).Area = new Game(gameType);
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            case "Restaurant":
                                Enum.TryParse(line.Split()[j].Split(';')[1], out RestaurantType restaurantType);
                                data.GetField(i, j).Area = new Restaurant(restaurantType);
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            case "Road":
                                Enum.TryParse(line.Split()[j].Split(';')[1], out RoadType roadType);
                                data.GetField(i, j).Area = new Road(roadType);
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            case "Plant":
                                Enum.TryParse(line.Split()[j].Split(';')[1], out PlantType plantType);
                                data.GetField(i, j).Area = new Plant(plantType);
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            case "Staff":
                                Enum.TryParse(line.Split()[j].Split(';')[1], out StaffType staffType);
                                data.GetField(i, j).Area = new Staff(staffType);
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            case "PowerSource":
                                data.GetField(i, j).Area = new PowerSource();
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            case "Bin":
                                data.GetField(i, j).Area = new Bin();
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            case "WC":
                                data.GetField(i, j).Area = new WC();
                                data.Areas.Add(tmpPoint, data.GetField(i, j).Area);
                                break;
                            default:
                                break;
                        }
                    }
                }
                int staffCount = int.Parse(await sr.ReadLineAsync());
                for (int i = 0; i < staffCount; i++)
                {
                    line = await sr.ReadLineAsync();
                    Enum.TryParse(line.Split()[0], out StaffType staffType);
                    data.AddStaff(int.Parse(line.Split()[1]), int.Parse(line.Split()[2]), new Staff(staffType));
                }
                int entityCount = int.Parse(await sr.ReadLineAsync());
                Entity tmp;
                for (int i = 0; i < entityCount; i++)
                {
                    line = await sr.ReadLineAsync();
                    Enum.TryParse(line.Split()[0], out EntityType entityType);
                    tmp = new Entity(entityType, bool.Parse(line.Split()[7]), 0);
                    tmp.Money = int.Parse(line.Split()[3]);
                    tmp.Hunger = int.Parse(line.Split()[4]);
                    tmp.WcUrge = int.Parse(line.Split()[5]);
                    tmp.Adrenalin = int.Parse(line.Split()[6]);
                    tmp.HasTrash = bool.Parse(line.Split()[8]);
                    tmp.State = EntityState.NOTHING_TO_DO;
                    data.AddEntity(int.Parse(line.Split()[0]), int.Parse(line.Split()[1]), tmp);
                }
                return data;
            }
                

        }

        public async Task SaveAsync(string path, ParkPersistence data)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(data.RowSize + " " +data.ColSize);
                sw.WriteLine(data.ParkName);
                sw.WriteLine(data.AvailableMoney + " " + data.SatisfactionLevel + " " + data.GetEntryFee() + " " + false /*data.ParkIsOpen*/ + " " + data.Time);
                for (int i = 0; i < data.RowSize; i++)
                {
                    for (int j = 0; j < data.ColSize; j++)
                    {
                        switch (data.GetField(i, j).Area.GetType().Name.ToString())
                        {
                            case "Game":
                                Game tmpG = (Game)data.GetField(i, j).Area;
                                tmpG.EmptyInGameEntities();
                                await sw.WriteAsync("Game;" + tmpG.Type + " ");

                                break;
                            case "Restaurant":
                                Restaurant tmpR = (Restaurant)data.GetField(i, j).Area;
                                tmpR.EmptyInRestaurantEntities();
                                await sw.WriteAsync("Restaurant;" + tmpR.Type + " ");
                                break;
                            case "Road":
                                Road tmpRoad = (Road)data.GetField(i, j).Area;
                                await sw.WriteAsync("Road;" + tmpRoad.Type + " ");
                                break;
                            case "Plant":
                                Plant tmpPlant = (Plant)data.GetField(i, j).Area;
                                await sw.WriteAsync("Plant;" + tmpPlant.Type + " ");
                                break;
                            case "Staff":
                                Staff tmpStaff = (Staff)data.GetField(i, j).Area;
                                await sw.WriteAsync("Staff;" + tmpStaff.Type + " ");
                                break;
                            default:
                                await sw.WriteAsync(data.GetField(i, j).Area.GetType().Name.ToString() + ";NULL ");
                                break;
                        }
                    }
                    await sw.WriteLineAsync();
                }
                await sw.WriteLineAsync(data.GetStaffs().Count().ToString());
                foreach (var item in data.GetStaffs())
                {
                    await sw.WriteLineAsync(item.Key.Type + " " + item.Value.X + " " + item.Value.Y);
                }
                await sw.WriteLineAsync(data.GetEntities().Count().ToString());
                foreach (var item in data.GetEntities())
                {
                    await sw.WriteLineAsync(item.Value.X + " " + item.Value.Y + " " + item.Key.Type + " " + item.Key.Money + " " + item.Key.Hunger + " " + item.Key.WcUrge + " " + item.Key.Adrenalin + " " + item.Key.HasCoupon + " " + item.Key.HasTrash);
                }
            }
        }
    }
}
