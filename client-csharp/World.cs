#region Usings

using System.Collections.Generic;
using System.Linq;
using AiCup22.Model;

#endregion

namespace AiCup22;

public class World
{
    #region Zone

    public Constants Constants { get; }
    public Vec2 ZoneCenter { get; set; }
    public double ZoneRadius { get; set; }
    public Vec2 ZoneNextCenter { get; set; }
    public double ZoneNextRadius { get; set; }

    #endregion

    #region Units

    public MyUnit Me => MyUnits.First(); // todo Round 1 only
    public List<MyUnit> MyUnits { get; } = new();
    public List<EnemyUnit> EnemyUnits { get; } = new();

    public EnemyUnit NearestEnemy => EnemyUnits.OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    #endregion

    #region Items

    public List<WeaponItem> WeaponItems { get; } = new();
    public WeaponItem NearestPistol => WeaponItems.Where(e => e.Type == WeaponItem.WeaponType.Pistol).OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public WeaponItem NearestRifle => WeaponItems.Where(e => e.Type == WeaponItem.WeaponType.Rifle).OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public WeaponItem NearestSniper => WeaponItems.Where(e => e.Type == WeaponItem.WeaponType.Sniper).OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    public List<AmmoItem> AmmoItems { get; } = new();
    public AmmoItem NearestPistolAmmo => AmmoItems.Where(e => e.Type == WeaponItem.WeaponType.Pistol).OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public AmmoItem NearestRifleAmmo => AmmoItems.Where(e => e.Type == WeaponItem.WeaponType.Rifle).OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public AmmoItem NearestSniperAmmo => AmmoItems.Where(e => e.Type == WeaponItem.WeaponType.Sniper).OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    public List<ShieldItem> ShieldItems { get; } = new();
    public ShieldItem NearestShieldItem => ShieldItems.OrderByDescending(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

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
                    WeaponItems.Add(new WeaponItem(loot));
                    break;
                case Item.Ammo:
                    AmmoItems.Add(new AmmoItem(loot));
                    break;
                case Item.ShieldPotions:
                    ShieldItems.Add(new ShieldItem(loot));
                    break;
            }
        }
    }
}