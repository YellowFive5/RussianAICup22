﻿#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using AiCup22.CustomModel;
using AiCup22.Model;
using Object = AiCup22.CustomModel.Object;
using Sound = AiCup22.CustomModel.Sound;

#endregion

namespace AiCup22;

public class World
{
    public Constants Constants { get; }
    public Game Game { get; set; }

    #region Zone

    public Vec2 ZoneCenter => Game.Zone.CurrentCenter;
    public double ZoneRadius => Game.Zone.CurrentRadius;
    public Vec2 ZoneNextCenter => Game.Zone.NextCenter;
    public double ZoneNextRadius => Game.Zone.NextRadius;

    #endregion

    #region Units

    public MyUnit Me { get; set; }
    public bool OutOfZone => Measurer.GetDistanceBetween(ZoneCenter, Me.Position) >= Game.Zone.CurrentRadius;
    public bool NearToOutOfZone => Measurer.GetDistanceBetween(ZoneCenter, Me.Position) >= Game.Zone.CurrentRadius * NearToOutOfZoneCoefficient;
    public double NearToOutOfZoneCoefficient => 0.980;

    public List<CustomUnit> AllUnits => MyUnits.Cast<CustomUnit>()
                                               .Union(EnemyUnits).ToList();

    public List<MyUnit> MyUnits { get; set; } = new();
    public List<MyUnit> MyTeammates => MyUnits.Where(u => u.Id != Me.Id).ToList();
    public bool HasAnyTeammates => MyTeammates.Any();

    public MyUnit Commander => HasAnyTeammates
                                   ? MyUnits.First()
                                   : Me;

    public bool IsFarFromCommander => IsImDeputy && Measurer.GetDistanceBetween(Me.Position, Commander.Position) >= Constants.ViewDistance * 0.5;
    public bool IsImDeputy => HasAnyTeammates && Me != MyUnits.First();

    public List<EnemyUnit> EnemyUnits { get; set; } = new();

    public EnemyUnit NearestEnemy => EnemyUnits.Where(e => !e.IsSpawning)
                                               .OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();

    public bool IsNearestEnemyVisible => NearestEnemy != null;
    public EnemyUnit NearestWeakestEnemy => EnemyUnits.OrderBy(e => e.HealthShieldPoints).FirstOrDefault();

    public EnemyUnit ReachestPointsEnemy => EnemyUnits
                                            .OrderByDescending(u => Game.Players.Single(p => p.Id == u.Unit.PlayerId).Score)
                                            .FirstOrDefault();

    public double NearestEnemyDistance => Measurer.GetDistanceBetween(Me.Position, NearestEnemy.Position);
    public EnemyUnit NearestPistolEnemy => EnemyUnits.Where(e => e.WeaponType == WeaponLootItem.WeaponType.Pistol).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestPistolEnemyVisible => NearestPistolEnemy != null;
    public double NearestPistolEnemyDistance => Measurer.GetDistanceBetween(Me.Position, NearestPistolEnemy.Position);

    public EnemyUnit NearestRifleEnemy => EnemyUnits.Where(e => e.WeaponType == WeaponLootItem.WeaponType.Rifle).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestRifleEnemyVisible => NearestRifleEnemy != null;
    public double NearestRifleEnemyDistance => Measurer.GetDistanceBetween(Me.Position, NearestRifleEnemy.Position);

    public EnemyUnit NearestSniperEnemy => EnemyUnits.Where(e => e.WeaponType == WeaponLootItem.WeaponType.Sniper).OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position)).FirstOrDefault();
    public bool IsNearestSniperEnemyVisible => NearestSniperEnemy != null;
    public double NearestSniperEnemyDistance => Measurer.GetDistanceBetween(Me.Position, NearestSniperEnemy.Position);

    #endregion

    #region Items

    public List<ShieldLootItem> ShieldItems { get; set; } = new();

    public ShieldLootItem NearestShieldLootItem => ShieldItems.OrderBy(e => Measurer.GetDistanceBetween(Me.Position, e.Position))
                                                              .FirstOrDefault();

    public bool IsNearestShieldLootItemVisible => NearestShieldLootItem != null;

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

    public AmmoLootItem GetNearestActiveAmmoLoot()
    {
        switch (Me.WeaponType)
        {
            case WeaponLootItem.WeaponType.Pistol:
                return NearestPistolAmmoLoot;
            case WeaponLootItem.WeaponType.Rifle:
                return NearestRifleAmmoLoot;
            case WeaponLootItem.WeaponType.Sniper:
                return NearestSniperAmmoLoot;
            case WeaponLootItem.WeaponType.None:
                return null;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool IsNearestActiveAmmoVisible()
    {
        switch (Me.WeaponType)
        {
            case WeaponLootItem.WeaponType.Pistol:
                return IsNearestPistolAmmoLootVisible;
            case WeaponLootItem.WeaponType.Rifle:
                return IsNearestRifleAmmoLootVisible;
            case WeaponLootItem.WeaponType.Sniper:
                return IsNearestSniperAmmoLootVisible;
            case WeaponLootItem.WeaponType.None:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public CustomItem GetNearestItemToLoot()
    {
        var nearestCollectibles = new List<CustomItem>();
        if (IsNearestShieldLootItemVisible)
        {
            nearestCollectibles.Add(NearestShieldLootItem);
        }

        if (IsNearestActiveAmmoVisible())
        {
            nearestCollectibles.Add(GetNearestActiveAmmoLoot());
        }

        switch (Me.WeaponType)
        {
            case WeaponLootItem.WeaponType.Pistol:
            {
                if (IsNearestSniperVisible)
                {
                    nearestCollectibles.Add(NearestSniper);
                }

                if (IsNearestRifleVisible)
                {
                    nearestCollectibles.Add(NearestRifle);
                }

                break;
            }
            case WeaponLootItem.WeaponType.Rifle when IsNearestSniperVisible:
                nearestCollectibles.Add(NearestSniper);
                break;
        }

        return nearestCollectibles.OrderBy(nc => Measurer.GetDistanceBetween(nc.Position, Me.Position))
                                  .FirstOrDefault();
    }

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

    public void Scan(Game game, MyUnit me)
    {
        Game = game;
        Me = me;

        MyUnits = new List<MyUnit>();
        EnemyUnits = new List<EnemyUnit>();
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
        foreach (var loot in game.Loot.Where(l => Measurer.GetDistanceBetween(game.Zone.CurrentCenter, l.Position) < game.Zone.CurrentRadius * NearToOutOfZoneCoefficient))
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