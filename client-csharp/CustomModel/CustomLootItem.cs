#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public abstract class CustomLootItem : CustomItem
{
    public Loot Loot { get; }

    protected CustomLootItem(Loot loot, Constants constants) : base(loot.Id, loot.Position, constants)
    {
        Loot = loot;
    }
}