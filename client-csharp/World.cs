#region Usings

using System.Collections.Generic;
using System.Linq;
using AiCup22.CustomModel;
using AiCup22.Model;

#endregion

namespace AiCup22;

public class World
{
    public Constants Constants { get; }

    #region Zone

    public Vec2 ZoneCenter { get; set; }
    public double ZoneRadius { get; set; }
    public Vec2 ZoneNextCenter { get; set; }
    public double ZoneNextRadius { get; set; }

    #endregion

    #region Units

    public MyUnit Me => MyUnits.First(); // todo Round 1 only
    public List<MyUnit> MyUnits { get; } = new();
    public List<EnemyUnit> EnemyUnits { get; } = new();

    public EnemyUnit NearestEnemy => EnemyUnits.OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    #endregion

    #region Items

    public List<WeaponLootItem> WeaponItems { get; } = new();
    public WeaponLootItem NearestPistol => WeaponItems.Where(e => e.Type == WeaponLootItem.WeaponType.Pistol).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public WeaponLootItem NearestRifle => WeaponItems.Where(e => e.Type == WeaponLootItem.WeaponType.Rifle).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public WeaponLootItem NearestSniper => WeaponItems.Where(e => e.Type == WeaponLootItem.WeaponType.Sniper).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    public List<AmmoLootItem> AmmoItems { get; } = new();
    public AmmoLootItem NearestPistolAmmoLoot => AmmoItems.Where(e => e.Type == WeaponLootItem.WeaponType.Pistol).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public AmmoLootItem NearestRifleAmmoLoot => AmmoItems.Where(e => e.Type == WeaponLootItem.WeaponType.Rifle).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public AmmoLootItem NearestSniperAmmoLoot => AmmoItems.Where(e => e.Type == WeaponLootItem.WeaponType.Sniper).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    public List<ShieldLootItem> ShieldItems { get; } = new();
    public ShieldLootItem NearestShieldLootItem => ShieldItems.OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    #endregion

    #region Objects

    public List<Object> Objects { get; } = new();
    public Object NearestObject => Objects.OrderBy(o => Measurer.GetDistanceBetween(Me.Position, o.Position)).FirstOrDefault();
    public Object NearestCoverObject => Objects.Where(o => o.IsBulletProof).OrderBy(o => Measurer.GetDistanceBetween(Me.Position, o.Position)).FirstOrDefault();

    #endregion

    #region Bullets

    public List<Bullet> Bullets { get; } = new();
    public Bullet NearestBullet => Bullets.Where(b => b.Projectile.ShooterPlayerId != Me.Unit.PlayerId).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    #endregion

    public World(Constants constants)
    {
        Constants = constants;
    }

    public void Scan(Game game)
    {
        ZoneCenter = game.Zone.CurrentCenter;
        ZoneRadius = game.Zone.CurrentRadius;
        ZoneNextCenter = game.Zone.NextCenter;
        ZoneNextRadius = game.Zone.NextRadius;

        foreach (var unit in game.Units)
        {
            if (unit.PlayerId == game.MyId)
            {
                MyUnits.Add(new MyUnit(unit));
            }
            else
            {
                EnemyUnits.Add(new EnemyUnit(unit));
            }
        }

        foreach (var loot in game.Loot)
        {
            switch (loot.Item)
            {
                case Item.Weapon:
                    WeaponItems.Add(new WeaponLootItem(loot));
                    break;
                case Item.Ammo:
                    AmmoItems.Add(new AmmoLootItem(loot));
                    break;
                case Item.ShieldPotions:
                    ShieldItems.Add(new ShieldLootItem(loot));
                    break;
            }
        }

        foreach (var projectile in game.Projectiles)
        {
            Bullets.Add(new Bullet(projectile));
        }

        foreach (var obstacle in Constants.Obstacles)
        {
            Objects.Add(new Object(obstacle));
        }
    }
}