#region Usings

using System;
using AiCup22.Model;

#endregion

namespace AiCup22;

public static class Measurer
{
    public static double GetDistanceBetween(Vec2 a, Vec2 b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    public static Vec2 GetTargetDirectionTo(Vec2 from, Vec2 to)
    {
        return new Vec2
               {
                   X = to.X - from.X,
                   Y = to.Y - from.Y
               };
    }

    public static Vec2 GetTargetVelocityTo(Vec2 from, Vec2 to)
    {
        var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        return new Vec2
               {
                   X = to.X - from.X + Math.Cos(angle) * 20,
                   Y = to.Y - from.Y + Math.Sin(angle) * 20
               };
    }
}