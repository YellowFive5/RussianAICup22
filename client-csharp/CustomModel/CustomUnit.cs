#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public abstract class CustomUnit : CustomItem
{
    public Unit Unit { get; }
    public double Health { get; }
    public bool IsHealthDamaged => Health <= Constants.UnitHealth * 0.85;
    public bool IsHealthInjured => Health <= Constants.UnitHealth * 0.5;
    public double Shield { get; }
    public bool IsShieldDamaged => Shield <= Constants.MaxShield * 0.85;
    public bool IsShieldInjured => Shield <= Constants.MaxShield * 0.5;
    public bool IsShieldEmpty => Shield <= 0;

    public int Potions { get; }
    public bool IsPotionsFull => Potions >= Constants.MaxShieldPotionsInInventory;
    public bool IsPotionsUnderHalf => Potions <= Constants.MaxShieldPotionsInInventory * 0.5;
    public bool IsPotionsEmpty => Potions <= 0;
    public WeaponLootItem.WeaponType WeaponType { get; }
    public int Ammo { get; }
    public bool IsAmmoFull => Ammo <= Constants.Weapons[(int)WeaponType].MaxInventoryAmmo * 0.85;
    public bool IsOutOfAmmo => Ammo <= Constants.Weapons[(int)WeaponType].MaxInventoryAmmo * 0.50;
    public bool IsAmmoEmpty => Ammo <= 0;

    public Action? Action { get; }
    public bool IsLooting => Action?.ActionType == ActionType.Looting;
    public bool IsHeeling => Action?.ActionType == ActionType.UseShieldPotion;
    public bool IsAimed => Unit.Aim >= 1;
    public bool IsHalfAimed => Unit.Aim > 0.5;

    protected CustomUnit(Unit unit, Constants constants) : base(unit.Id, unit.Position, constants)
    {
        Unit = unit;
        WeaponType = unit.Weapon switch
        {
            0 => WeaponLootItem.WeaponType.Pistol,
            1 => WeaponLootItem.WeaponType.Rifle,
            2 => WeaponLootItem.WeaponType.Sniper,
            _ => WeaponLootItem.WeaponType.None
        };
        Health = unit.Health;
        Shield = unit.Shield;
        Potions = unit.ShieldPotions;
        Ammo = WeaponType == WeaponLootItem.WeaponType.None
                   ? 3
                   : unit.Ammo[(int)WeaponType];
        Action = unit.Action;
    }
}