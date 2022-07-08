#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public class AmmoLootItem : CustomLootItem
{
    public WeaponLootItem.WeaponType Type { get; }
    public int Amount { get; }

    public AmmoLootItem(Loot loot, Constants constants) : base(loot, constants)
    {
        var ammo = loot.Item as Item.Ammo;
        Type = ammo.WeaponTypeIndex switch
        {
            0 => WeaponLootItem.WeaponType.Pistol,
            1 => WeaponLootItem.WeaponType.Rifle,
            2 => WeaponLootItem.WeaponType.Sniper,
            _ => WeaponLootItem.WeaponType.None
        };
        Amount = ammo.Amount;
    }
}