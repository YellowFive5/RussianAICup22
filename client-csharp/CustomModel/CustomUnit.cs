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
    public bool IsShieldDamaged => Shield <= Constants.UnitHealth * 0.85;
    public bool IsShieldInjured => Shield <= Constants.UnitHealth * 0.5;
    public bool IsShieldTotallyBroken => Shield <= 0;

    public int Potions { get; }
    public bool IsPotionsFull => Potions >= Constants.MaxShieldPotionsInInventory;
    public bool IsPotionsUnderHalf => Potions <= Constants.MaxShieldPotionsInInventory * 0.5;
    public WeaponLootItem.WeaponType WeaponType { get; }
    public int Ammo { get; }
    public Action? Action { get; }
    public bool IsLooting => Action?.ActionType == ActionType.Looting;
    public bool IsHeeling => Action?.ActionType == ActionType.UseShieldPotion;

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
                   ? 0
                   : unit.Ammo[(int)WeaponType];
        Action = unit.Action;
    }
}