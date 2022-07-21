#region Usings

using System.Collections.Generic;
using System.Linq;
using AiCup22.CustomModel;
using AiCup22.Debugging;
using AiCup22.Model;

#endregion

namespace AiCup22
{
    public class MyStrategy
    {
        public Constants Constants { get; }
        private Measurer Measurer { get; set; }
        private World World { get; set; }
        private MyUnit Me { get; set; }
        private MyUnit Leader { get; set; }
        private DebugInterface DebugInterface { get; set; }
        public List<MyUnit> MyUnits { get; set; } = new();
        public Dictionary<int, UnitOrder> Commands { get; set; }

        public MyStrategy(Constants constants)
        {
            Constants = constants;
            World = new World(constants);
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            // DebugInterface = debugInterface; // debug on
            DebugInterface = null; // debug off

            Commands = new Dictionary<int, UnitOrder>();
            MyUnits = new List<MyUnit>();
            World = new World(Constants);
            Measurer = new Measurer(World, DebugInterface);

            foreach (var unit in game.Units)
            {
                if (unit.PlayerId == game.MyId)
                {
                    MyUnits.Add(new MyUnit(unit, Constants));
                }
            }

            Leader = MyUnits.First();

            foreach (var me in MyUnits)
            {
                Me = me;
                World.Scan(game, me);
                ChooseAction();
            }

            return new Order(Commands);
        }

        private void ChooseAction()
        {
            // DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Potions.ToString(), new Vec2(), 5, CustomDebug.VioletColor));
            // DebugInterface?.Add(new DebugData.Ring(World.NearestSniperAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));
            // DebugInterface?.Add(new DebugData.PolyLine(new[] { new Vec2(50,100), new Vec2(0,50) }, 5, CustomDebug.GreenColor));
            // DebugInterface?.Add(new DebugData.Ring(World.DangerZone, 2, 2, CustomDebug.RedColor));

            ReturnInZone();

            Heel();

            ChangeWeapon();

            ProcessItems();

            AttackEnemy();

            GoToTarget();
        }


        #region Behaviour

        private void ReturnInZone()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (World.NearToOutOfZone)
            {
                GoTo(Measurer.GetZoneBorderPoint(Me));
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ReturnInZone/1", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (World.InDangerZone)
            {
                GoBackFrom(World.DangerZone);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ReturnInZone/2", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Me.IsSpawning && World.IsFarFromTeammate)
            {
                GoTo(World.NearestTeammate);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ReturnInZone/3", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Me.IsSpawning && World.IsNearestEnemyVisible)
            {
                GoBackFrom(World.NearestEnemy);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ReturnInZone/4", new Vec2(), 2, CustomDebug.VioletColor));
            }
        }

        private void Heel()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (Me.IsPotionsEmpty || Me.IsShieldFull)
            {
                return;
            }

            if (Me.IsShieldEmpty)
            {
                // Heel
                TakePotion(World.IsNearestShotEnemyVisible);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "Heel/1", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (!Measurer.IsDistanceAllowToHit(World.NearestShootEnemy, Me)
                ||
                !Measurer.IsClearVisible(World.NearestShootEnemy.Position, Me.Position))
            {
                // Heel
                TakePotion(World.IsNearestShotEnemyVisible);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "Heel/2", new Vec2(), 2, CustomDebug.VioletColor));
            }
        }

        private void ProcessItems()
        {
            if (Me.IsPotionsEmpty && Me.IsAmmoEmpty)
            {
                if (World.IsNearestShieldLootItemVisible && World.IsNearestActiveAmmoVisible())
                {
                    if (Measurer.GetDistanceBetween(World.NearestShieldLootItem.Position, Me.Position) < Measurer.GetDistanceBetween(World.GetNearestActiveAmmoLoot().Position, Me.Position))
                    {
                        CollectPotions();
                        return;
                    }

                    CollectAmmo();
                    return;
                }
            }

            if (Me.IsPotionsEmpty)
            {
                CollectPotions();
                return;
            }

            if (Me.IsAmmoEmpty)
            {
                CollectAmmo();
                return;
            }

            var itemToLoot = World.GetNearestItemToLoot();
            if (itemToLoot == null)
            {
                return;
            }

            switch (itemToLoot)
            {
                case ShieldLootItem:
                    CollectPotions();
                    return;
                case AmmoLootItem:
                    CollectAmmo();
                    return;
            }
        }

        private void CollectPotions()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (Me.IsPotionsEmpty &&
                World.IsNearestShieldLootItemVisible)
            {
                GoPickup(World.NearestShieldLootItem);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectPotions/1", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Me.NeedToCollectPotions &&
                World.IsNearestShieldLootItemVisible &&
                (!Measurer.IsDistanceAllowToHit(World.NearestShootEnemy, Me) ||
                 !Measurer.IsClearVisible(World.NearestShootEnemy.Position, Me.Position)))
            {
                GoPickup(World.NearestShieldLootItem);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectPotions/2", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (!Me.IsPotionsFull &&
                World.IsNearestShieldLootItemVisible &&
                !World.IsNearestShotEnemyVisible)
            {
                GoPickup(World.NearestShieldLootItem);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectPotions/3", new Vec2(), 2, CustomDebug.VioletColor));
            }
        }

        private void CollectAmmo()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (Me.IsAmmoEmpty && World.IsNearestActiveAmmoVisible())
            {
                GoPickup(World.GetNearestActiveAmmoLoot());
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectAmmo/1", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Me.NeedToCollectAmmo &&
                World.IsNearestActiveAmmoVisible() &&
                (!Measurer.IsDistanceAllowToHit(World.NearestShootEnemy, Me) ||
                 !Measurer.IsClearVisible(World.NearestShootEnemy.Position, Me.Position)))
            {
                GoPickup(World.GetNearestActiveAmmoLoot());
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectAmmo/2", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (!Me.IsAmmoFull &&
                World.IsNearestActiveAmmoVisible() &&
                !World.IsNearestShotEnemyVisible)
            {
                GoPickup(World.GetNearestActiveAmmoLoot());
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectAmmo/3", new Vec2(), 2, CustomDebug.VioletColor));
            }
        }

        private void ChangeWeapon()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (Me.IsAmmoEmpty)
            {
                if (Me.WeaponType == WeaponLootItem.WeaponType.Sniper && !World.IsNearestSniperAmmoLootVisible)
                {
                    if (World.IsNearestRifleVisible && World.IsNearestRifleAmmoLootVisible)
                    {
                        GoPickup(World.NearestRifle);
                        DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/1", new Vec2(), 2, CustomDebug.VioletColor));
                        return;
                    }

                    if (World.IsNearestPistolVisible && World.IsNearestPistolAmmoLootVisible)
                    {
                        GoPickup(World.NearestPistol);
                        DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/2", new Vec2(), 2, CustomDebug.VioletColor));
                        return;
                    }
                }

                if (Me.WeaponType == WeaponLootItem.WeaponType.Rifle && !World.IsNearestRifleAmmoLootVisible)
                {
                    if (World.IsNearestPistolVisible && World.IsNearestPistolAmmoLootVisible)
                    {
                        GoPickup(World.NearestPistol);
                        DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/3", new Vec2(), 2, CustomDebug.VioletColor));
                        return;
                    }
                }
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.None)
            {
                if (World.IsNearestSniperVisible && World.IsNearestSniperAmmoLootVisible)
                {
                    GoPickup(World.NearestSniper);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/4", new Vec2(), 2, CustomDebug.VioletColor));
                    return;
                }

                if (World.IsNearestRifleVisible && World.IsNearestRifleAmmoLootVisible)
                {
                    GoPickup(World.NearestRifle);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/5", new Vec2(), 2, CustomDebug.VioletColor));
                    return;
                }

                if (World.IsNearestPistolVisible && World.IsNearestPistolAmmoLootVisible)
                {
                    GoPickup(World.NearestPistol);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/6", new Vec2(), 2, CustomDebug.VioletColor));
                    return;
                }
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Pistol)
            {
                if (World.IsNearestSniperVisible && World.IsNearestSniperAmmoLootVisible)
                {
                    GoPickup(World.NearestSniper);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/7", new Vec2(), 2, CustomDebug.VioletColor));
                    return;
                }

                if (World.IsNearestRifleVisible && World.IsNearestRifleAmmoLootVisible)
                {
                    GoPickup(World.NearestRifle);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/8", new Vec2(), 2, CustomDebug.VioletColor));
                    return;
                }
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Rifle)
            {
                if (World.IsNearestSniperVisible && World.IsNearestSniperAmmoLootVisible)
                {
                    GoPickup(World.NearestSniper);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/9", new Vec2(), 2, CustomDebug.VioletColor));
                }
            }
        }

        private void AttackEnemy()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            // See anyone and have ammo
            if (!World.IsNearestShotEnemyVisible || Me.IsAmmoEmpty)
            {
                return;
            }

            // Has distance and clear vision
            if (Measurer.IsDistanceAllowToHit(Me, World.NearestShootEnemy) &&
                Measurer.IsClearVisible(Me.Position, World.NearestShootEnemy.Position) &&
                Me.IsAimed)
            {
                // Shoot
                ComeToAim(World.NearestShootEnemy, true);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "AttackEnemy/1", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Measurer.IsDistanceAllowToHit(Me, World.NearestShootEnemy))
            {
                // Aim
                ComeToAim(World.NearestShootEnemy);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "AttackEnemy/2", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            GoTo(World.NearestShootEnemy);
            DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "AttackEnemy/3", new Vec2(), 2, CustomDebug.VioletColor));
        }

        private void GoToTarget()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            GoTo(Measurer.GetZoneBorderPoint(Me));
            DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "GoToTarget/1", new Vec2(), 2, CustomDebug.VioletColor));
        }

        #endregion

        #region Actions

        private void GoBackFrom(CustomUnit unit)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, unit.Position, invertedVelocity: true);
            Commands.Add(Me.Id, new UnitOrder(movement.velocity, Measurer.GetInvertedVector(Me.Direction), null));
        }

        private void GoBackFrom(Vec2 point)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, point, invertedVelocity: true);
            Commands.Add(Me.Id, new UnitOrder(movement.velocity, Measurer.GetInvertedVector(Me.Direction), null));
        }

        private void GoPickup(CustomItem item)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, item.Position);
            var actionPickup = new ActionOrder.Pickup(item.Id);
            Commands.Add(Me.Id, new UnitOrder(movement.velocity, movement.direction, actionPickup));
        }

        private void GoTo(CustomUnit unit)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, unit.Position);
            Commands.Add(Me.Id, new UnitOrder(movement.velocity, movement.direction, null));
        }

        private void GoTo(Vec2 point)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, point);
            Commands.Add(Me.Id, new UnitOrder(movement.velocity, Measurer.GetInvertedVector(Me.Direction), null));
        }

        private void ComeToAim(CustomUnit unit, bool withShot = false)
        {
            var smartAim = Measurer.GetSmartDirectionVelocity(Me, unit.Position, unit.Velocity);
            var velocity = Measurer.GetBulletsDodgeVelocity(Me, unit);

            var actionAim = new ActionOrder.Aim(withShot);
            Commands.Add(Me.Id, new UnitOrder(velocity, smartAim.direction, actionAim));
        }

        private void TakePotion(bool isNearestEnemyVisible)
        {
            var actionUseShieldPotion = new ActionOrder.UseShieldPotion();
            Commands.Add(Me.Id,
                         isNearestEnemyVisible
                             ? new UnitOrder(Measurer.GetBulletsDodgeVelocity(Me, World.NearestShootEnemy), Measurer.GetVectorTo(Me.Position, World.NearestShootEnemy.Position), actionUseShieldPotion)
                             : new UnitOrder(Measurer.GetWiggleVelocity(Me.Direction), Measurer.GetInvertedVector(Me.Direction), actionUseShieldPotion));
        }

        #endregion

        public void DebugUpdate(int displayedTick, DebugInterface debugInterface)
        {
            debugInterface.Clear();
        }

        public void Finish()
        {
        }
    }
}