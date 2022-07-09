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
    public const double PistolRange = 24;
    public const double RifleRange = 16;
    public const double SniperRange = 36;

    public const double HitRangeCoefficient = 0.2;
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

    public static bool IsDistanceAllowToHit(MyUnit me, EnemyUnit enemy, double coefficient = 1)
    {
        switch (me.WeaponType)
        {
            case WeaponLootItem.WeaponType.Pistol:
                return GetDistanceBetween(me.Position, enemy.Position) <= PistolRange * coefficient;
            case WeaponLootItem.WeaponType.Rifle:
                return GetDistanceBetween(me.Position, enemy.Position) <= RifleRange * coefficient;
            case WeaponLootItem.WeaponType.Sniper:
                return GetDistanceBetween(me.Position, enemy.Position) <= SniperRange * coefficient;
            case WeaponLootItem.WeaponType.None:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static bool IsHittableFromEnemy(MyUnit me, EnemyUnit enemy)
    {
        switch (enemy.WeaponType)
        {
            case WeaponLootItem.WeaponType.Pistol:
                return GetDistanceBetween(me.Position, enemy.Position) <= PistolRange * (1 + HitRangeCoefficient);
            case WeaponLootItem.WeaponType.Rifle:
                return GetDistanceBetween(me.Position, enemy.Position) <= RifleRange * (1 + HitRangeCoefficient);
            case WeaponLootItem.WeaponType.Sniper:
                return GetDistanceBetween(me.Position, enemy.Position) <= SniperRange * (1 + HitRangeCoefficient);
            case WeaponLootItem.WeaponType.None:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static bool IsClearVisible(MyUnit me, EnemyUnit enemy, List<Object> objects)
    {
        var distance = GetDistanceBetween(me.Position, enemy.Position);
        var potentialCover = objects.Where(o => !o.IsBulletProof &&
                                                GetDistanceBetween(me.Position, o.Position) <= distance * 1.15 &&
                                                GetDistanceBetween(enemy.Position, o.Position) <= distance * 1.15);

        var dpx = enemy.Position.X - me.Position.X;
        var dpy = enemy.Position.Y - me.Position.Y;
        var a = dpx * dpx + dpy * dpy;
        foreach (var o in potentialCover)
        {
            var b = 2 * (dpx * (me.Position.X - o.Position.X) + dpy * (me.Position.Y - o.Position.Y));
            var c = o.Position.X * o.Position.X + o.Position.Y * o.Position.Y;
            c += me.Position.X * me.Position.X + me.Position.Y * me.Position.Y;
            c -= 2 * (o.Position.X * me.Position.X + o.Position.Y * me.Position.Y);
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