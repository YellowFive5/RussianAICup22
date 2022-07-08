#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public class Bullet : CustomItem
{
    public Projectile Projectile { get; }
    public WeaponLootItem.WeaponType WeaponType { get; }
    public Vec2 Velocity { get; }

    public Bullet(Projectile projectile) : base(projectile.Id, projectile.Position)
    {
        Projectile = projectile;
        WeaponType = projectile.WeaponTypeIndex switch
        {
            0 => WeaponLootItem.WeaponType.Pistol,
            1 => WeaponLootItem.WeaponType.Rifle,
            2 => WeaponLootItem.WeaponType.Sniper,
            _ => WeaponLootItem.WeaponType.None
        };
        Velocity = projectile.Velocity;
    }
}