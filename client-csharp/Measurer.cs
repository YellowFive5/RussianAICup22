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
}