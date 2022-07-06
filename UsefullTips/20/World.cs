#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace UsefullTips._20
{
    public class World
    {
        public BehaviorType Behavior { get; private set; }
        public Vec2Int Zero { get; }
        public Vec2Int SquareOfMyInterests { get; private set; }

        public readonly double[] aggressiveBehaviorUnitsRatio = {0.2, 0.4, 0.4};

        public readonly double[] passiveBehaviorUnitsRatio = {0.6, 0.2, 0.2};
        public readonly double[] passiveBehaviorPlusUnitsRatio = {0.2, 0.4, 0.4};

        public double[] unitsRatio;
        public int PopulationProvide { get; private set; }
        public int PopulationUse { get; private set; }
        public int PopulationFree => PopulationProvide - PopulationUse;

        public bool NeedBuildBuilders => MyUnitsWorkersCount < PopulationProvide * unitsRatio[0]
                                         && Me.Resource >= WorkerUnitCost;

        public bool NeedBuildRanged => !NeedBuildBuilders &&
                                       MyUnitsRangedCount < PopulationProvide * unitsRatio[1]
                                       && Me.Resource >= RangedUnitCost;

        public bool NeedBuildMelee => !NeedBuildBuilders &&
                                      MyUnitsMeleesCount < PopulationProvide * unitsRatio[2]
                                      && Me.Resource >= MeleeUnitCost;

        public bool NeedBuildBuildingWorkers => !MyBuildingsWorkers.Any() &&
                                                Me.Resource >= WorkersBuildingCost;

        public bool NeedBuildBuildingRanged => !MyBuildingsRanged.Any() &&
                                               Me.Resource >= RangedBuildingCost &&
                                               !NeedBuildBuildingWorkers;

        public bool NeedBuildHouse => !RepairNeeds &&
                                      PopulationFree <= 2 &&
                                      Me.Resource >= HouseBuildingCost &&
                                      !NeedBuildBuildingWorkers &&
                                      !NeedBuildBuildingRanged;

        public bool NeedBuildTurrets => !RepairNeeds &&
                                        MyBuildingsHousesCount >= MyUnitsTurretsCount &&
                                        Me.Resource >= TurretUnitCost &&
                                        !NeedBuildHouse &&
                                        !NeedBuildBuildingWorkers &&
                                        !NeedBuildBuildingRanged;

        public bool RepairNeeds { get; set; }

        #region Costs

        public int WorkerUnitCost { get; private set; }
        public int RangedUnitCost { get; private set; }
        public int MeleeUnitCost { get; private set; }
        public int TurretUnitCost { get; private set; }
        public int HouseBuildingCost { get; private set; }
        public int WorkersBuildingCost { get; private set; }
        public int RangedBuildingCost { get; private set; }
        public int MeleeBuildingCost { get; private set; }
        public int WallBuildingCost { get; private set; }

        #endregion

        public IEnumerable<Entity> SpiceMilange { get; private set; }
        public List<Entity> BusySpiceMilange { get; private set; }

        public List<Vec2Int> Points { get; }

        public List<Vec2Int> NotFreePoints { get; private set; }
        public List<Vec2Int> FreePoints { get; private set; }


        #region Enemy

        public IEnumerable<Player> EnemyPlayers { get; private set; }
        public IEnumerable<Entity> EnemyEntities { get; private set; }
        public IEnumerable<Entity> EnemyBuildings => EnemyBuildingsWalls.Union(EnemyBuildingsHouses).Union(EnemyBuildingsWorkers).Union(EnemyBuildingsMelees).Union(EnemyBuildingsRanged).ToList();

        public IEnumerable<Entity> EnemyBuildingsWalls => EnemyEntities.Where(e => e.EntityType == EntityType.Wall).ToArray();
        public IEnumerable<Entity> EnemyBuildingsHouses => EnemyEntities.Where(e => e.EntityType == EntityType.House).ToArray();
        public IEnumerable<Entity> EnemyBuildingsWorkers => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderBase).ToArray();
        public IEnumerable<Entity> EnemyBuildingsMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeBase).ToArray();
        public IEnumerable<Entity> EnemyBuildingsRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedBase).ToArray();
        public IEnumerable<Entity> EnemyUnits => EnemyUnitsTurrets.Union(EnemyUnitsWorkers).Union(EnemyUnitsMelees).Union(EnemyUnitsRanged).ToList();

        public IEnumerable<Entity> EnemyUnitsTurrets => EnemyEntities.Where(e => e.EntityType == EntityType.Turret).ToArray();
        public IEnumerable<Entity> EnemyUnitsWorkers => EnemyEntities.Where(e => e.EntityType == EntityType.BuilderUnit).ToArray();
        public IEnumerable<Entity> EnemyUnitsMelees => EnemyEntities.Where(e => e.EntityType == EntityType.MeleeUnit).ToArray();
        public IEnumerable<Entity> EnemyUnitsRanged => EnemyEntities.Where(e => e.EntityType == EntityType.RangedUnit).ToArray();

        #endregion

        #region Me

        public Player Me { get; private set; }
        public IEnumerable<Entity> MyEntities { get; private set; }
        public IEnumerable<Entity> MyBuildings => MyBuildingsRanged.Union(MyBuildingsMelees).Union(MyBuildingsWorkers).Union(MyBuildingsHouses).Union(MyBuildingsWalls).Union(MyUnitsTurrets).ToList();
        public IEnumerable<Entity> MyBuildingsBroken { get; private set; }
        public IEnumerable<Entity> MyUnitsBroken { get; private set; }
        public IEnumerable<Entity> MyBuildingsWalls => MyEntities.Where(e => e.EntityType == EntityType.Wall).ToArray();
        public IEnumerable<Entity> MyBuildingsHouses => MyEntities.Where(e => e.EntityType == EntityType.House).ToArray();
        public IEnumerable<Entity> MyBuildingsWorkers => MyEntities.Where(e => e.EntityType == EntityType.BuilderBase).ToArray();
        public IEnumerable<Entity> MyBuildingsMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeBase).ToArray();
        public IEnumerable<Entity> MyBuildingsRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedBase).ToArray();
        public IEnumerable<Entity> MyUnits => MyUnitsWorkers.Union(MyUnitsMelees).Union(MyUnitsRanged).Union(MyUnitsTurrets).ToList();
        public IEnumerable<Entity> MyUnitsTurrets => MyEntities.Where(e => e.EntityType == EntityType.Turret).ToArray();
        public IEnumerable<Entity> MyUnitsWorkers => MyEntities.Where(e => e.EntityType == EntityType.BuilderUnit).ToArray();
        public int MyUnitsWorkersCount { get; set; }
        public int MyUnitsRangedCount { get; set; }
        public int MyUnitsMeleesCount { get; set; }
        public int MyBuildingsHousesCount { get; set; }
        public int MyUnitsTurretsCount { get; set; }
        public IEnumerable<Entity> MyUnitsMelees => MyEntities.Where(e => e.EntityType == EntityType.MeleeUnit).ToArray();
        public IEnumerable<Entity> MyUnitsRanged => MyEntities.Where(e => e.EntityType == EntityType.RangedUnit).ToArray();
        public Entity MyTopBuilding;

        #endregion

        public World()
        {
            Zero = new Vec2Int(0, 0);
            Points = new List<Vec2Int>();
            for (int x = 0; x < 80; x++)
            {
                for (int y = 0; y < 80; y++)
                {
                    Points.Add(new Vec2Int(x, y));
                }
            }
        }

        public void Scan(PlayerView view)
        {
            Me = view.Players.Single(p => p.Id == view.MyId);
            EnemyPlayers = view.Players.Where(p => p.Id != view.MyId).ToArray();
            SpiceMilange = view.Entities.Where(e => e.EntityType == EntityType.Resource).ToArray();
            BusySpiceMilange = new List<Entity>();
            EnemyEntities = view.Entities.Where(e => e.PlayerId != view.MyId && e.EntityType != EntityType.Resource).ToArray();
            MyEntities = view.Entities.Where(e => e.PlayerId == view.MyId).ToArray();
            MyBuildingsBroken = MyBuildings.Where(b => b.Health < view.EntityProperties.Single(ep => ep.Key == b.EntityType).Value.MaxHealth).Union(MyUnitsTurrets.Where(t => t.Health < view.EntityProperties.Single(ep => ep.Key == t.EntityType).Value.MaxHealth));
            MyUnitsBroken = MyUnits.Where(b => b.Health < view.EntityProperties.Single(ep => ep.Key == b.EntityType).Value.MaxHealth);
            RepairNeeds = MyBuildingsBroken.Any();
            MyUnitsWorkersCount = MyUnitsWorkers.Count();
            MyUnitsRangedCount = MyUnitsRanged.Count();
            MyUnitsMeleesCount = MyUnitsMelees.Count();
            MyBuildingsHousesCount = MyBuildingsHouses.Count();
            MyUnitsTurretsCount = MyUnitsTurrets.Count();

            // todo delete -1 in another round
            HouseBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.House).Value.InitialCost;
            WorkersBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.BuilderBase).Value.InitialCost;
            RangedBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.RangedBase).Value.InitialCost;
            MeleeBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.MeleeBase).Value.InitialCost;
            WallBuildingCost = view.EntityProperties.Single(ep => ep.Key == EntityType.Wall).Value.InitialCost;
            WorkerUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.BuilderUnit).Value.InitialCost + MyUnitsWorkers.Count() - 1;
            RangedUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.RangedUnit).Value.InitialCost + MyUnitsRanged.Count() - 1;
            MeleeUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.MeleeUnit).Value.InitialCost + MyUnitsMelees.Count() - 1;
            TurretUnitCost = view.EntityProperties.Single(ep => ep.Key == EntityType.Turret).Value.InitialCost + MyUnitsTurrets.Count() - 1;

            PopulationProvide = 0;
            PopulationUse = 0;
            foreach (var entity in MyEntities)
            {
                PopulationProvide += view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.PopulationProvide;
                PopulationUse += view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.PopulationUse;
            }

            NotFreePoints = new List<Vec2Int>();
            foreach (var entity in view.Entities)
            {
                var size = view.EntityProperties.Single(ep => ep.Key == entity.EntityType).Value.Size;

                if (MyBuildings.Contains(entity))
                {
                    for (var x = 0; x < size + 2; x++)
                    {
                        for (var y = 0; y < size + 2; y++)
                        {
                            var _x = entity.Position.X + x - 1;
                            var _y = entity.Position.Y + y - 1;
                            NotFreePoints.CheckPointInsideAndSave(_x, _y);
                        }
                    }
                }
                else
                {
                    for (var x = 0; x < size; x++)
                    {
                        for (var y = 0; y < size; y++)
                        {
                            NotFreePoints.Add(new Vec2Int(entity.Position.X + x,
                                                          entity.Position.Y + y));
                        }
                    }
                }
            }

            // var soi_x = 0;
            // var soi_y = 0;
            // foreach (var ett in MyEntities)
            // {
            //     if (ett.Position.X > soi_x)
            //     {
            //         soi_x = ett.Position.X;
            //     }
            //
            //     if (ett.Position.Y > soi_y)
            //     {
            //         soi_y = ett.Position.Y;
            //     }
            // }
            //
            // SquareOfMyInterests = new Vec2Int(soi_x + 1, soi_y + 1);
            //
            // FreePoints = Points.Except(NotFreePoints).ToList();

            var sum = 0;
            foreach (var building in MyBuildings)
            {
                if (building.Position.X + building.Position.Y > sum)
                {
                    sum = building.Position.X + building.Position.Y;
                    MyTopBuilding = building;
                }
            }

            ChooseBehavior(view);
        }

        private void ChooseBehavior(PlayerView view)
        {
            if (EnemyEntities.Any() &&
                (MyEntities.Any(e => GetDistance(GetNearestEntity(e, PlayerType.Enemy).Position, e.Position) < 10)))
                // ||
                // EnemyUnits.Any(e => e.Position.X <= SquareOfMyInterests.X && e.Position.Y <= SquareOfMyInterests.Y)))
            {
                Behavior = BehaviorType.Aggressive;
                unitsRatio = aggressiveBehaviorUnitsRatio;
            }
            else
            {
                Behavior = BehaviorType.Passive;
                unitsRatio = passiveBehaviorUnitsRatio;
                if (view.CurrentTick > 250)
                {
                    unitsRatio = passiveBehaviorPlusUnitsRatio;
                }
            }
        }

        public Entity GetNearestEntityOfType(Vec2Int sourcePoint, PlayerType playerType, EntityType type, int index = 0)
        {
            IEnumerable<Entity> targetCollection;

            switch (playerType)
            {
                case PlayerType.My:
                    switch (type)
                    {
                        case EntityType.Wall:
                            targetCollection = MyBuildingsWalls;
                            break;
                        case EntityType.House:
                            targetCollection = MyBuildingsHouses;
                            break;
                        case EntityType.BuilderBase:
                            targetCollection = MyBuildingsWorkers;
                            break;
                        case EntityType.BuilderUnit:
                            targetCollection = MyUnitsWorkers;
                            break;
                        case EntityType.MeleeBase:
                            targetCollection = MyBuildingsMelees;
                            break;
                        case EntityType.MeleeUnit:
                            targetCollection = MyUnitsMelees;
                            break;
                        case EntityType.RangedBase:
                            targetCollection = MyBuildingsRanged;
                            break;
                        case EntityType.RangedUnit:
                            targetCollection = MyUnitsRanged;
                            break;
                        case EntityType.Resource:
                            targetCollection = SpiceMilange;
                            break;
                        case EntityType.Turret:
                            targetCollection = MyUnitsTurrets;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }

                    break;
                case PlayerType.Enemy:
                    switch (type)
                    {
                        case EntityType.Wall:
                            targetCollection = EnemyBuildingsWalls;
                            break;
                        case EntityType.House:
                            targetCollection = EnemyBuildingsHouses;
                            break;
                        case EntityType.BuilderBase:
                            targetCollection = EnemyBuildingsWorkers;
                            break;
                        case EntityType.BuilderUnit:
                            targetCollection = EnemyUnitsWorkers;
                            break;
                        case EntityType.MeleeBase:
                            targetCollection = EnemyBuildingsMelees;
                            break;
                        case EntityType.MeleeUnit:
                            targetCollection = EnemyUnitsMelees;
                            break;
                        case EntityType.RangedBase:
                            targetCollection = EnemyBuildingsRanged;
                            break;
                        case EntityType.RangedUnit:
                            targetCollection = EnemyUnitsRanged;
                            break;
                        case EntityType.Resource:
                            targetCollection = SpiceMilange;
                            break;
                        case EntityType.Turret:
                            targetCollection = EnemyUnitsTurrets;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null);
            }

            double distance1 = 1000;
            double distance2 = 1000;
            var nearestEntity = new Entity();
            var nearestEntity2 = new Entity();

            foreach (var ett in targetCollection) // todo to LINQ
            {
                var dst = GetDistance(ett.Position, sourcePoint);
                if (dst < distance1)
                {
                    distance1 = dst;
                    nearestEntity = ett;
                }
                else if (dst < distance2)
                {
                    distance2 = dst;
                    nearestEntity2 = ett;
                }
            }

            return index == 0
                       ? nearestEntity
                       : nearestEntity2;
        }

        public Entity GetNearestEntity(Entity sourceEntity, PlayerType playerType)
        {
            IEnumerable<Entity> targetCollection;

            switch (playerType)
            {
                case PlayerType.My:
                    targetCollection = MyEntities;
                    break;
                case PlayerType.Enemy:
                    targetCollection = EnemyEntities;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerType), playerType, null);
            }

            double distanceBetween = 1000;
            var nearestEntity = new Entity();

            foreach (var ett in targetCollection) // todo to LINQ
            {
                var dst = GetDistance(ett.Position, sourceEntity.Position);
                if (dst < distanceBetween)
                {
                    distanceBetween = dst;
                    nearestEntity = ett;
                }
            }

            return nearestEntity;
        }

        public Entity GetNearestNotBusySpice(Entity sourceEntity)
        {
            var targetCollection = SpiceMilange.Except(BusySpiceMilange);

            double distanceBetween = 1000;
            var nearestEntity = new Entity();

            foreach (var ett in targetCollection) // todo to LINQ
            {
                var dst = GetDistance(ett.Position, sourceEntity.Position);
                if (dst < distanceBetween)
                {
                    distanceBetween = dst;
                    nearestEntity = ett;
                }
            }

            BusySpiceMilange.Add(nearestEntity);
            return nearestEntity;
        }

        private double GetDistance(Vec2Int one, Vec2Int two)
        {
            var distX = two.X - one.X;
            var distY = two.Y - one.Y;
            return Math.Sqrt(distX * distX + distY * distY);
        }
    }
}