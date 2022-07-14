#region Usings

using System;
using System.Linq;
using System.Numerics;
using AiCup22.CustomModel;
using AiCup22.Debugging;
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

    public (Vec2 direction, Vec2 velocity) GetSmartDirectionVelocity(CustomUnit from,
                                                                     Vec2 to,
                                                                     Vec2 targetVelocity = default,
                                                                     bool invertedVelocity = false)
    {
        var nearestCollisionObject = GetCollisionObjectsOnMyWay(from, to);

        var collisioned = nearestCollisionObject != null && Math.Round(GetDistanceBetween(from.Position, to)) >= Math.Round(GetDistanceBetween(to, nearestCollisionObject.Position));

        var invertCoefficient = invertedVelocity
                                    ? -1
                                    : 1;

        // Default - Simple
        if (targetVelocity.X == 0 && targetVelocity.Y == 0)
        {
            var realFrom = from.Position;
            var realTarget = to;

            if (collisioned)
            {
                var r = nearestCollisionObject.Radius + UnitRadius;

                var dx = nearestCollisionObject.Position.X - realFrom.X;
                var dy = nearestCollisionObject.Position.Y - realFrom.Y;
                var dd = Math.Sqrt(dx * dx + dy * dy);
                var a1 = Math.Asin(r / dd);
                var b1 = Math.Atan2(dy, dx);

                var t3 = b1 - a1;
                var ta = new Vec2(r * Math.Sin(t3),
                                  r * -Math.Cos(t3));

                t3 = b1 + a1;
                var tb = new Vec2(r * -Math.Sin(t3),
                                  r * Math.Cos(t3));

                // realTarget = World.Game.CurrentTick / 30 % 2 == 0
                //                  ? new Vec2(ta.X + nearestCollisionObject.Position.X, ta.Y + nearestCollisionObject.Position.Y)
                //                  : new Vec2(tb.X + nearestCollisionObject.Position.X, tb.Y + nearestCollisionObject.Position.Y);

                realTarget = new Vec2(ta.X + nearestCollisionObject.Position.X, ta.Y + nearestCollisionObject.Position.Y);
                // DebugInterface?.Add(new DebugData.Ring(nearestCollisionObject.Position, r, 0.1, CustomDebug.VioletColor));
                // DebugInterface?.Add(new DebugData.PolyLine(new[] { realFrom, realTarget }, 0.3, CustomDebug.GreenColor));
            }

            var angleSimple = FindAngle(realFrom, realTarget);
            var velocitySimple = new Vec2
                                 {
                                     X = (realTarget.X - realFrom.X + Math.Cos(angleSimple) * 20) * invertCoefficient,
                                     Y = (realTarget.Y - realFrom.Y + Math.Sin(angleSimple) * 20) * invertCoefficient
                                 };

            var directionSimple = new Vec2(realTarget.X - realFrom.X, realTarget.Y - realFrom.Y);

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
        var angle = FindAngle(from.Position, direction);

        var velocity = new Vec2
                       {
                           X = (direction.X - from.Position.X + Math.Cos(angle) * 20) * invertCoefficient,
                           Y = (direction.Y - from.Position.Y + Math.Sin(angle) * 20) * invertCoefficient
                       };

        return (direction, velocity);
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

    public CustomItem GetCollisionObjectsOnMyWay(CustomItem from, Vec2 to)
    {
        if (from == null)
        {
            return null;
        }

        var potentialCover = World.Objects.Cast<CustomItem>()
                                  // .Union(World.AllUnits) // todo
                                  .Where(o => GetDistanceBetween(from.Position, o.Position) <= 25)
                                  .OrderBy(o => GetDistanceBetween(from.Position, o.Position));

        var dpx = to.X - from.Position.X;
        var dpy = to.Y - from.Position.Y;
        var a = dpx * dpx + dpy * dpy;
        foreach (var o in potentialCover)
        {
            var rad = o.Radius + UnitRadius;
            var b = 2 * (dpx * (from.Position.X - o.Position.X) + dpy * (from.Position.Y - o.Position.Y));
            var c = o.Position.X * o.Position.X + o.Position.Y * o.Position.Y;
            c += from.Position.X * from.Position.X + from.Position.Y * from.Position.Y;
            c -= 2 * (o.Position.X * from.Position.X + o.Position.Y * from.Position.Y);
            c -= rad * rad;
            var bb4ac = b * b - 4 * a * c;
            if (Math.Abs(a) < float.Epsilon || bb4ac < 0)
            {
            }
            else
            {
                return o;
            }
        }

        return null;
    }

    public static double GetDistanceBetween(Vec2 a, Vec2 b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    public float FindAngle(Vec2 point1, Vec2 point2)
    {
        return (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
    }

    public Vec2 FindRayPoint(Vec2 from, Vec2 through, int rayDistance = 10)
    {
        var angle = FindAngle(from, through);
        return new Vec2(from.X + Math.Cos(angle) * rayDistance,
                        from.Y + Math.Sin(angle) * rayDistance);
    }

    public Vec2 GetWiggleVelocity(Vec2 from, bool fourSides = false)
    {
        // var normaized = new Vec2(Me.Direction.X + Me.Position.X, Me.Direction.Y + Me.Position.Y);
        // var p = Measurer.FindRayPoint(Me.Position, normaized);
        // DebugInterface.Add(new DebugData.PolyLine(new[] { Me.Position, p }, 1, CustomDebug.GreenColor));
        //
        // var smartAim = Measurer.GetWiggleVelocity(Me.Direction);
        // var normaizedV = new Vec2(smartAim.X + Me.Position.X, smartAim.Y + Me.Position.Y);
        //
        // DebugInterface.Add(new DebugData.PolyLine(new[] { Me.Position, normaizedV }, 1, CustomDebug.RedColor));
        //
        // var actionAim = new ActionOrder.Aim(false);
        //
        // Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(smartAim, Me.Direction, actionAim) }, };

        const double degToRad = Math.PI / 180;
        int angle;

        if (fourSides && World.Game.CurrentTick / 30 % 2 == 0)
        {
            angle = World.Game.CurrentTick / 15 % 3 == 0
                        ? 0
                        : 180;
        }
        else
        {
            angle = World.Game.CurrentTick / 15 % 3 == 0
                        ? 90
                        : 270;
        }

        var ca = Math.Cos(angle * degToRad);
        var sa = Math.Sin(angle * degToRad);
        return new Vec2
               {
                   X = (ca * from.X - sa * from.Y) * 30,
                   Y = (sa * from.X + ca * from.Y) * 30,
               };
    }

    public Vec2 GetBulletsDodgeVelocity(MyUnit myUnit, CustomUnit enemy)
    {
        var direction = myUnit.Direction;

        var bulletToDodge = World.Bullets
                                 .Where(b => b.Projectile.ShooterPlayerId != World.Me.Unit.PlayerId)
                                 .Where(b => GetDistanceBetween(b.Position, myUnit.Position) <= GetDistanceBetween(enemy.Position, myUnit.Position))
                                 .OrderBy(b => GetDistanceBetween(myUnit.Position, b.Position))
                                 .FirstOrDefault();

        const double degToRad = Math.PI / 180;
        if (bulletToDodge != null)
        {
            var meToEnemy = FindAngle(myUnit.Position, enemy.Position);
            var meToBullet = FindAngle(myUnit.Position, bulletToDodge.Position);
            var resultAngle = meToBullet - meToEnemy;
            
            // DebugInterface?.Add(new DebugData.Ring(bulletToDodge.Position, 0.5, 0.5, CustomDebug.VioletColor));
            // DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, resultAngle.ToString(), World.Me.Direction, 2, CustomDebug.GreenColor));

            var angle = 0;
            if (resultAngle < 0)
            {
                angle = 90;
                // DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "Left", new Vec2(), 5, CustomDebug.GreenColor));
            }
            else
            {
                angle = 270;
                // DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "Right", new Vec2(), 5, CustomDebug.GreenColor));
            }

            var ca = Math.Cos(angle * degToRad);
            var sa = Math.Sin(angle * degToRad);
            return new Vec2
                   {
                       X = (ca * direction.X - sa * direction.Y) * 30,
                       Y = (sa * direction.X + ca * direction.Y) * 30,
                   };
        }

        return GetWiggleVelocity(direction);
    }


    public Vec2 GetRandomVec()
    {
        return new Vec2
               {
                   X = new Random().Next(-20, 20),
                   Y = new Random().Next(-20, 20)
               };
    }

    public Vec2 GetInvertedVec(Vec2 to)
    {
        return new Vec2
               {
                   X = -to.Y,
                   Y = to.X
               };
    }
}