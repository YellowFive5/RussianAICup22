#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22;

public class WeaponItem : CustomItem
{
    public enum WeaponType
    {
        Pistol,
        Rifle,
        Sniper,
        None
    }

    public WeaponType Type { get; }

    public WeaponItem(Loot loot) : base(loot)
    {
        var weapon = loot.Item as Item.Weapon;
        Type = weapon.TypeIndex switch
        {
            0 => WeaponType.Pistol,
            1 => WeaponType.Rifle,
            2 => WeaponType.Sniper,
            _ => WeaponType.None
        };
    }
}