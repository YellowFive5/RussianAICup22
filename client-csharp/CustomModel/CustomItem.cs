#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public abstract class CustomItem
{
    public int Id { get; }
    public Vec2 Position { get; }
    public Constants Constants { get; }
    public double Radius { get; }
    public bool IsBulletProof { get; }


    protected CustomItem(int id, Vec2 position, double radius, bool isBulletProof, Constants constants)
    {
        Id = id;
        Position = position;
        Constants = constants;
        IsBulletProof = isBulletProof;
        Radius = radius;
    }
}