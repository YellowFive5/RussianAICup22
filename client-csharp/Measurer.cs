#region Usings

using System;
using System.Linq;
using System.Numerics;
using AiCup22.CustomModel;
using AiCup22.Model;

#endregion

namespace AiCup22;

public class Measurer
{
    public World World { get; }
    public DebugInterface DebugInterface { get; }
    public readonly double[] WeaponRanges = { 30.3, 20.3, 40.4 };
    public const double InZonePointCoefficient = 0.1;
    public const double UnitRadius = 1.0;

    public Measurer(World world, DebugInterface debugInterface)
    {
        World = world;
        DebugInterface = debugInterface;
    }

    public static double GetDistanceBetween(Vec2 a, Vec2 b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    public (Vec2 direction, Vec2 velocity) GetSmartDirectionVelocity(CustomUnit from,
                                                                     Vec2 to,
                                                                     Vec2 targetVelocity = default,
                                                                     bool invertedVelocity = false)
    {
        var nearestCollisionObject = World.Objects.Cast<CustomItem>()
                                          .Union(World.MyTeammates)
                                          .OrderBy(o => GetDistanceBetween(from.Position, o.Position))
                                          .FirstOrDefault();
        var collisionRadius = nearestCollisionObject.Radius + from.Radius * 1.0;
        var collisioned = GetDistanceBetween(from.Position, nearestCollisionObject.Position) <= collisionRadius;

        var invertCoefficient = invertedVelocity && !collisioned
                                    ? -1
                                    : 1;

        // Default - Simple
        if (targetVelocity.X == 0 && targetVelocity.Y == 0)
        {
            var realFrom = !collisioned
                               ? from.Position
                               : nearestCollisionObject.Position;
            var realTarget = !collisioned
                                 ? to
                                 : from.Position;

            var angleSimple = (float)Math.Atan2(realTarget.Y - realFrom.Y, realTarget.X - realFrom.X);
            var velocitySimple = new Vec2
                                 {
                                     X = (realTarget.X - realFrom.X + Math.Cos(angleSimple) * 20) * invertCoefficient,
                                     Y = (realTarget.Y - realFrom.Y + Math.Sin(angleSimple) * 20) * invertCoefficient
                                 };

            var directionSimple = new Vec2(to.X - from.Position.X, to.Y - from.Position.Y);

            return (directionSimple, velocitySimple);
        }

        // Smart
        var vectorFrom = new Vector2((float)from.Position.X, (float)from.Position.Y);
        var vectorTo = new Vector2((float)to.X, (float)to.Y);
        var vectorToVelocity = new Vector2((float)targetVelocity.X, (float)targetVelocity.Y);

        var totarget = vectorTo - vectorFrom;

        var a = Vector2.Dot(vectorToVelocity, vectorToVelocity) - (float)(World.Constants.Weapons[(int)from.WeaponType].ProjectileSpeed * World.Constants.Weapons[(int)from.WeaponType].ProjectileSpeed);
        var b = 2 * Vector2.Dot(vectorToVelocity, totarget);
        var c = Vector2.Dot(totarget, totarget);

        var p = -b / (2 * a);
        var q = (float)Math.Sqrt((b * b) - 4 * a * c) / (2 * a);

        var t1 = p - q;
        var t2 = p + q;
        float t;

        if (t1 > t2 && t2 > 0)
        {
            t = t2;
        }
        else
        {
            t = t1;
        }

        var aimSpot = vectorTo + vectorToVelocity * t;
        var direction = new Vec2(aimSpot.X - from.Position.X, aimSpot.Y - from.Position.Y);

        var angle = (float)Math.Atan2(direction.Y - from.Position.Y, direction.X - from.Position.X);
        var velocity = new Vec2
                       {
                           X = (direction.X - from.Position.X + Math.Cos(angle) * 20) * invertCoefficient,
                           Y = (direction.Y - from.Position.Y + Math.Sin(angle) * 20) * invertCoefficient
                       };

        return (direction, velocity);
    }

    public Vec2 GetRandomVec()
    {
        return new Vec2
               {
                   X = new Random().Next(-20, 20),
                   Y = new Random().Next(-20, 20)
               };
    }

    public Vec2 GetZoneBorderPoint(CustomItem item)
    {
        var vX = item.Position.X - World.ZoneCenter.X;
        var vY = item.Position.Y - World.ZoneCenter.Y;
        var magV = Math.Sqrt(vX * vX + vY * vY);
        var aX = World.ZoneCenter.X + vX / magV * (World.ZoneRadius - InZonePointCoefficient * World.ZoneRadius);
        var aY = World.ZoneCenter.Y + vY / magV * (World.ZoneRadius - InZonePointCoefficient * World.ZoneRadius);
        return new Vec2 { X = aX, Y = aY };
    }

    public bool IsDistanceAllowToHit(CustomUnit from, CustomUnit to, double coefficient = 1.0)
    {
        if (from == null || to == null)
        {
            return false;
        }

        return GetDistanceBetween(from.Position, to.Position) <= WeaponRanges[(int)from.WeaponType] * coefficient;
    }

    public bool IsClearVisible(CustomUnit from, CustomUnit to)
    {
        if (from == null || to == null)
        {
            return false;
        }

        var distance = GetDistanceBetween(from.Position, to.Position);
        var potentialCover = World.Objects.Cast<CustomItem>()
                                  // .Union(World.AllUnits) // todo
                                  .Where(o => !o.IsBulletProof &&
                                              GetDistanceBetween(from.Position, o.Position) <= distance * 1.15 &&
                                              GetDistanceBetween(to.Position, o.Position) <= distance * 1.15);

        var dpx = to.Position.X - from.Position.X;
        var dpy = to.Position.Y - from.Position.Y;
        var a = dpx * dpx + dpy * dpy;
        foreach (var o in potentialCover)
        {
            var b = 2 * (dpx * (from.Position.X - o.Position.X) + dpy * (from.Position.Y - o.Position.Y));
            var c = o.Position.X * o.Position.X + o.Position.Y * o.Position.Y;
            c += from.Position.X * from.Position.X + from.Position.Y * from.Position.Y;
            c -= 2 * (o.Position.X * from.Position.X + o.Position.Y * from.Position.Y);
            c -= o.Radius * o.Radius;
            var bb4ac = b * b - 4 * a * c;
            if (Math.Abs(a) < float.Epsilon || bb4ac < 0)
            {
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}