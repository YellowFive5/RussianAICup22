#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using AiCup22.CustomModel;
using AiCup22.Model;
using Object = AiCup22.CustomModel.Object;

#endregion

namespace AiCup22;

public static class Measurer
{
    public static readonly double[] WeaponRanges = { 24, 16, 36 };
    public const double InZonePointCoefficient = 0.1;

    public static double GetDistanceBetween(Vec2 a, Vec2 b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    public static Vec2 GetTargetDirectionTo(Vec2 from, Vec2 to, bool inversed = false)
    {
        if (inversed)
        {
            return new Vec2
                   {
                       X = (to.X - from.X) * -1,
                       Y = (to.Y - from.Y) * -1
                   };
        }

        return new Vec2
               {
                   X = to.X - from.X,
                   Y = to.Y - from.Y
               };
    }

    public static Vec2 GetAdvancedTargetDirectionTo(CustomUnit from, CustomUnit to, World world)
    {
        return GetTargetDirectionTo(from.Position, to.Position);
        var distance = GetDistanceBetween(from.Position, to.Position);
        var coefficient = world.Constants.Weapons[(int)from.WeaponType].ProjectileSpeed;

        return new Vec2
               {
                   X = to.Position.X - from.Position.X + to.Unit.Velocity.X + distance / coefficient * 0.6,
                   Y = to.Position.Y - from.Position.Y + to.Unit.Velocity.Y + distance / coefficient * 0.6,
               };
    }

    public static Vec2 GetTargetVelocityTo(Vec2 from, Vec2 to, bool inversed = false)
    {
        var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        if (inversed)
        {
            return new Vec2
                   {
                       X = (to.X - from.X + Math.Cos(angle) * 20) * -1,
                       Y = (to.Y - from.Y + Math.Sin(angle) * 20) * -1
                   };
        }

        return new Vec2
               {
                   X = to.X - from.X + Math.Cos(angle) * 20,
                   Y = to.Y - from.Y + Math.Sin(angle) * 20
               };
    }

    public static Vec2 GetRandomVec()
    {
        return new Vec2
               {
                   X = new Random().Next(-20, 20),
                   Y = new Random().Next(-20, 20)
               };
    }

    public static Vec2 GetZoneBorderPoint(CustomItem item, Vec2 zoneCenter, double zoneRadius)
    {
        var vX = item.Position.X - zoneCenter.X;
        var vY = item.Position.Y - zoneCenter.Y;
        var magV = Math.Sqrt(vX * vX + vY * vY);
        var aX = zoneCenter.X + vX / magV * (zoneRadius - InZonePointCoefficient * zoneRadius);
        var aY = zoneCenter.Y + vY / magV * (zoneRadius - InZonePointCoefficient * zoneRadius);
        return new Vec2 { X = aX, Y = aY };
    }

    public static bool IsDistanceAllowToHit(CustomUnit from, CustomUnit to)
    {
        return GetDistanceBetween(from.Position, to.Position) <= WeaponRanges[(int)from.WeaponType];
    }

    public static bool IsClearVisible(CustomUnit from, CustomUnit to, IEnumerable<Object> objects)
    {
        var distance = GetDistanceBetween(from.Position, to.Position);
        var potentialCover = objects.Where(o => !o.IsBulletProof &&
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