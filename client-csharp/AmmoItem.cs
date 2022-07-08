#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22;

public class AmmoItem : CustomItem
{
    public WeaponItem.WeaponType Type { get; }
    public int Amount { get; }

    public AmmoItem(Loot loot) : base(loot)
    {
        var ammo = loot.Item as Item.Ammo;
        Type = ammo.WeaponTypeIndex switch
        {
            0 => WeaponItem.WeaponType.Pistol,
            1 => WeaponItem.WeaponType.Rifle,
            2 => WeaponItem.WeaponType.Sniper,
            _ => WeaponItem.WeaponType.None
        };
        Amount = ammo.Amount;
    }
}