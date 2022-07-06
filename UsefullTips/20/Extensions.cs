#region Usings

using System.Collections.Generic;
using System.Linq;

#endregion

namespace UsefullTips._20
{
    public static class Extensions
    {
        public static void CheckPointInsideAndSave(this List<Vec2Int> collection, int x, int y)
        {
            if (x >= 0 && y >= 0 && x < 80 && y < 80)
            {
                collection.Add(new Vec2Int(x, y));
            }
        }

        public static bool CheckPointInside(this Vec2Int point)
        {
            return point.X >= 0 && point.Y >= 0 && point.X < 80 && point.Y < 80;
        }

        public static List<List<Vec2Int>> GetMappingAround(this Entity entity, int buildingSize)
        {
            var _1 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x;
                    var _y = entity.Position.Y + y + 1;
                    _1.CheckPointInsideAndSave(_x, _y);
                }
            }

            var _2 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x + 1;
                    var _y = entity.Position.Y + y;
                    _2.CheckPointInsideAndSave(_x, _y);
                }
            }

            var _3 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x + 1;
                    var _y = entity.Position.Y + y + 1 - buildingSize;
                    _3.CheckPointInsideAndSave(_x, _y);
                }
            }

            var _4 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x;
                    var _y = entity.Position.Y + y - buildingSize;
                    _4.CheckPointInsideAndSave(_x, _y);
                }
            }

            var _5 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x + 1 - buildingSize;
                    var _y = entity.Position.Y + y - buildingSize;
                    _5.CheckPointInsideAndSave(_x, _y);
                }
            }

            var _6 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x - buildingSize;
                    var _y = entity.Position.Y + y + 1 - buildingSize;
                    _6.CheckPointInsideAndSave(_x, _y);
                }
            }

            var _7 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x - buildingSize;
                    var _y = entity.Position.Y + y;
                    _7.CheckPointInsideAndSave(_x, _y);
                }
            }

            var _8 = new List<Vec2Int>();
            for (var x = 0; x < buildingSize; x++)
            {
                for (var y = 0; y < buildingSize; y++)
                {
                    var _x = entity.Position.X + x + 1 - buildingSize;
                    var _y = entity.Position.Y + y + 1;
                    _8.CheckPointInsideAndSave(_x, _y);
                }
            }

            return new List<List<Vec2Int>> {_1, _2, _3, _4, _5, _6, _7, _8};
        }

        public static bool HasPlaceToBuildAround(this List<List<Vec2Int>> buildMap, World around, int buildingSize)
        {
            return buildMap.Any(l => l.Count == buildingSize * buildingSize && !l.Any(s => around.NotFreePoints.Any(np => np.X == s.X && np.Y == s.Y)));
        }

        private static bool CanBuildHere(this List<Vec2Int> buildMap, World around, int buildingSize)
        {
            var vec2Ints = buildMap.ToList();
            return vec2Ints.Count.Equals(buildingSize * buildingSize) && !vec2Ints.Any(s => around.NotFreePoints.Any(np => np.X == s.X && np.Y == s.Y));
        }

        public static Vec2Int GetFirstPointToBuildAround(this List<List<Vec2Int>> buildMap, Entity worker, World around, int buildingSize)
        {
            var pointToBuild = new Vec2Int();

            if (buildMap.ElementAt(0).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X, worker.Position.Y + 1);

                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            if (buildMap.ElementAt(1).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X + 1, worker.Position.Y);
                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            if (buildMap.ElementAt(2).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X + 1, worker.Position.Y + 1 - buildingSize);
                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            if (buildMap.ElementAt(3).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X, worker.Position.Y - buildingSize);
                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            if (buildMap.ElementAt(4).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X + 1 - buildingSize, worker.Position.Y - buildingSize);
                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            if (buildMap.ElementAt(5).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X - buildingSize, worker.Position.Y - buildingSize + 1);
                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            if (buildMap.ElementAt(6).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X - buildingSize, worker.Position.Y);
                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            if (buildMap.ElementAt(7).CanBuildHere(around, buildingSize))
            {
                pointToBuild = new Vec2Int(worker.Position.X + 1 - buildingSize, worker.Position.Y + 1);
                if (CheckPointInside(pointToBuild))
                {
                    return pointToBuild;
                }
            }

            return pointToBuild;
        }

        public static Vec2Int GetFirstPointToBuildNear(this List<Vec2Int> freePoints, World around, int buildingSize)
        {
            var pointsWhereCanBuild = new List<Vec2Int>();

            foreach (var freePoint in around.FreePoints)
            {
                var map = new List<Vec2Int>();
                for (int x = 0; x < buildingSize; x++)
                {
                    for (int y = 0; y < buildingSize; y++)
                    {
                        var _x = freePoint.X + x;
                        var _y = freePoint.Y + y;
                        map.CheckPointInsideAndSave(_x, _y);
                    }
                }

                if (map.CanBuildHere(around, buildingSize))
                {
                    pointsWhereCanBuild.Add(freePoint);
                }
            }

            return pointsWhereCanBuild
                   .OrderBy(p => p.X + p.Y)
                   .ElementAtOrDefault(0);
        }
    }
}