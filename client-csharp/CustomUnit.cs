#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22;

public abstract class CustomUnit
{
    public Unit Unit { get; }
    public int Id { get; }
    public Vec2 Position { get; }
    public double Health { get; }
    public double Shield { get; }
    public int Potions { get; }
    public WeaponItem.WeaponType WeaponType { get; }
    public int Ammo { get; }
    public Action? Action { get; }
    public bool IsLooting => Action?.ActionType == ActionType.Looting;
    public bool IsHeeling => Action?.ActionType == ActionType.UseShieldPotion;

    protected CustomUnit(Unit unit)
    {
        Unit = unit;
        Id = unit.Id;
        Position = unit.Position;
        WeaponType = unit.Weapon switch
        {
            0 => WeaponItem.WeaponType.Pistol,
            1 => WeaponItem.WeaponType.Rifle,
            2 => WeaponItem.WeaponType.Sniper,
            _ => WeaponItem.WeaponType.None
        };
        Health = unit.Health;
        Shield = unit.Shield;
        Potions = unit.ShieldPotions;
        Ammo = WeaponType == WeaponItem.WeaponType.None
                   ? 0
                   : unit.Ammo[(int)WeaponType];
        Action = unit.Action;
    }
}