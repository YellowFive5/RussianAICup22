#region Usings

using System;
using AiCup22.CustomModel;
using AiCup22.Model;

#endregion

namespace AiCup22;

public static class Measurer
{
    public const double PistolRange = 24;
    public const double RifleRange = 16;
    public const double SniperRange = 36;

    public const double Coefficient = 0.2;

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

    public static bool IsDistanceAllowToHit(MyUnit me, EnemyUnit enemy)
    {
        switch (me.WeaponType)
        {
            case WeaponLootItem.WeaponType.Pistol:
                return GetDistanceBetween(me.Position, enemy.Position) <= PistolRange;
            case WeaponLootItem.WeaponType.Rifle:
                return GetDistanceBetween(me.Position, enemy.Position) <= RifleRange;
            case WeaponLootItem.WeaponType.Sniper:
                return GetDistanceBetween(me.Position, enemy.Position) <= SniperRange;
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
                return GetDistanceBetween(me.Position, enemy.Position) <= PistolRange * (1 + Coefficient);
            case WeaponLootItem.WeaponType.Rifle:
                return GetDistanceBetween(me.Position, enemy.Position) <= RifleRange * (1 + Coefficient);
            case WeaponLootItem.WeaponType.Sniper:
                return GetDistanceBetween(me.Position, enemy.Position) <= SniperRange * (1 + Coefficient);
            case WeaponLootItem.WeaponType.None:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}