#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public class Object : CustomItem
{
    public Obstacle Obstacle { get; }
    public bool IsBulletProof { get; }
    public bool IsTransparent { get; }
    public double Radius { get; }

    public Object(Obstacle obstacle, Constants constants) : base(obstacle.Id, obstacle.Position, constants)
    {
        Obstacle = obstacle;
        IsBulletProof = obstacle.CanShootThrough;
        IsTransparent = obstacle.CanSeeThrough;
        Radius = obstacle.Radius;
    }
}