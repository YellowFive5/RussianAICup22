﻿#region Usings

using System.Collections.Generic;
using System.Linq;
using AiCup22.CustomModel;
using AiCup22.Model;
using Sound = AiCup22.CustomModel.Sound;

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
    public List<MyUnit> MyUnits { get; set; } = new();
    public List<EnemyUnit> EnemyUnits { get; set; } = new();
    public EnemyUnit NearestEnemy => EnemyUnits.OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestEnemyVisible => NearestEnemy != null;

    #endregion

    #region Items

    public List<WeaponLootItem> WeaponItems { get; set; } = new();
    public WeaponLootItem NearestPistol => WeaponItems.Where(e => e.Type == WeaponLootItem.WeaponType.Pistol).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestPistolVisible => NearestPistol != null;
    public WeaponLootItem NearestRifle => WeaponItems.Where(e => e.Type == WeaponLootItem.WeaponType.Rifle).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestRifleVisible => NearestRifle != null;
    public WeaponLootItem NearestSniper => WeaponItems.Where(e => e.Type == WeaponLootItem.WeaponType.Sniper).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestSniperVisible => NearestSniper != null;

    public List<AmmoLootItem> AmmoItems { get; set; } = new();
    public AmmoLootItem NearestPistolAmmoLoot => AmmoItems.Where(e => e.Type == WeaponLootItem.WeaponType.Pistol).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestPistolAmmoLootVisible => NearestPistolAmmoLoot != null;

    public AmmoLootItem NearestRifleAmmoLoot => AmmoItems.Where(e => e.Type == WeaponLootItem.WeaponType.Rifle).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestRifleAmmoLootVisible => NearestRifleAmmoLoot != null;

    public AmmoLootItem NearestSniperAmmoLoot => AmmoItems.Where(e => e.Type == WeaponLootItem.WeaponType.Sniper).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestSniperAmmoLootVisible => NearestSniperAmmoLoot != null;

    public List<ShieldLootItem> ShieldItems { get; set; } = new();
    public ShieldLootItem NearestShieldLootItem => ShieldItems.OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestShieldLootItemVisible => NearestShieldLootItem != null;

    #endregion

    #region Objects

    public List<Object> Objects { get; set; } = new();
    public Object NearestObject => Objects.OrderBy(o => Measurer.GetDistanceBetween(Me.Position, o.Position)).FirstOrDefault();
    public Object NearestCoverObject => Objects.Where(o => o.IsBulletProof).OrderBy(o => Measurer.GetDistanceBetween(Me.Position, o.Position)).FirstOrDefault();

    #endregion

    #region Bullets

    public List<Bullet> Bullets { get; set; } = new();
    public Bullet NearestBullet => Bullets.Where(b => b.Projectile.ShooterPlayerId != Me.Unit.PlayerId).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    #endregion

    #region Sounds

    public List<Sound> Sounds { get; set; } = new();
    public Sound NearestSound => Sounds.OrderBy(s => Measurer.GetDistanceBetween(Me.Position, s.Position)).FirstOrDefault();
    public bool IsNearestSoundHeard => NearestSound != null;

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

        EnemyUnits = new List<EnemyUnit>();
        MyUnits = new List<MyUnit>();
        foreach (var unit in game.Units)
        {
            if (unit.PlayerId == game.MyId)
            {
                MyUnits.Add(new MyUnit(unit, Constants));
            }
            else
            {
                EnemyUnits.Add(new EnemyUnit(unit, Constants));
            }
        }

        WeaponItems = new List<WeaponLootItem>();
        AmmoItems = new List<AmmoLootItem>();
        ShieldItems = new List<ShieldLootItem>();
        foreach (var loot in game.Loot)
        {
            switch (loot.Item)
            {
                case Item.Weapon:
                    WeaponItems.Add(new WeaponLootItem(loot, Constants));
                    break;
                case Item.Ammo:
                    AmmoItems.Add(new AmmoLootItem(loot, Constants));
                    break;
                case Item.ShieldPotions:
                    ShieldItems.Add(new ShieldLootItem(loot, Constants));
                    break;
            }
        }

        Bullets = new List<Bullet>();
        foreach (var projectile in game.Projectiles)
        {
            Bullets.Add(new Bullet(projectile, Constants));
        }

        Objects = new List<Object>();
        foreach (var obstacle in Constants.Obstacles)
        {
            Objects.Add(new Object(obstacle, Constants));
        }

        Sounds = new List<Sound>();
        foreach (var sound in game.Sounds)
        {
            Sounds.Add(new Sound(sound, Constants));
        }
    }
}