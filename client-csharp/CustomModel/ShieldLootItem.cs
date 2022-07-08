#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public class ShieldLootItem : CustomLootItem
{
    public int Amount { get; }

    public ShieldLootItem(Loot loot, Constants constants) : base(loot, constants)
    {
        var shield = loot.Item as Item.ShieldPotions;
        Amount = shield.Amount;
    }
}