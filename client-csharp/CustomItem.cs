#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22;

public abstract class CustomItem
{
    public Loot Loot { get; }
    public Vec2 Position { get; }

    protected CustomItem(Loot loot)
    {
        Loot = loot;
        Position = loot.Position;
    }
}