#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22;

public class ShieldItem : CustomItem
{
    public int Amount { get; }

    public ShieldItem(Loot loot) : base(loot)
    {
        var shield = loot.Item as Item.ShieldPotions;
        Amount = shield.Amount;
    }
}