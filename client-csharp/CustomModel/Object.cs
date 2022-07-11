#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public class Object : CustomItem
{
    public Obstacle Obstacle { get; }
    public bool IsTransparent { get; }

    public Object(Obstacle obstacle, Constants constants) : base(obstacle.Id, obstacle.Position, obstacle.Radius, obstacle.CanShootThrough, constants)
    {
        Obstacle = obstacle;
    }
}