#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public abstract class CustomItem
{
    public int Id { get; }
    public Vec2 Position { get; }

    protected CustomItem(int id, Vec2 position)
    {
        Id = id;
        Position = position;
    }
}