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

            ReturnInWhiteZone();

            Heel();

            ProcessItems();

            AttackEnemy();

            GoToTarget();
        }


        #region Behaviour

        private void ReturnInWhiteZone()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (World.NearToOutOfZone)
            {
                GoTo(Measurer.GetZoneBorderPoint(Me));
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ReturnInZone/GoTo(Measurer.GetZoneBorderPoint(Me))", new Vec2(), 2, CustomDebug.VioletColor));
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
                TakePotion(World.IsNearestEnemyVisible);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "Heel/Me.IsShieldEmpty/TakePotion())", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me)
                ||
                !Measurer.IsClearVisible(World.NearestEnemy, Me))
            {
                // Heel
                TakePotion(World.IsNearestEnemyVisible);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "Heel/IsDistanceAllowToHit/TakePotion())", new Vec2(), 2, CustomDebug.VioletColor));
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
                case WeaponLootItem:
                    ChangeWeapon();
                    break;
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
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectPotions/Me.IsPotionsEmpty/GoPickup(World.NearestShieldLootItem);)", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Me.NeedToCollectPotions &&
                World.IsNearestShieldLootItemVisible &&
                (!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me) ||
                 !Measurer.IsClearVisible(World.NearestEnemy, Me)))
            {
                GoPickup(World.NearestShieldLootItem);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectPotions/Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me)/GoPickup(World.NearestShieldLootItem);)", new Vec2(), 2, CustomDebug.VioletColor));
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
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectAmmo/Me.IsAmmoEmpty/GoPickup(World.GetNearestActiveAmmoLoot()))", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Me.NeedToCollectAmmo &&
                World.IsNearestActiveAmmoVisible() &&
                (!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me) ||
                 !Measurer.IsClearVisible(World.NearestEnemy, Me))
               )
            {
                GoPickup(World.GetNearestActiveAmmoLoot());
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "CollectAmmo/!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me)/GoPickup(World.GetNearestActiveAmmoLoot()))", new Vec2(), 2, CustomDebug.VioletColor));
            }
        }

        private void ChangeWeapon()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me) &&
                Measurer.IsClearVisible(World.NearestEnemy, Me))
            {
                return;
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Pistol)
            {
                if (World.IsNearestSniperVisible)
                {
                    GoPickup(World.NearestSniper);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/World.IsNearestSniperVisible/GoPickup(World.NearestSniper)", new Vec2(), 2, CustomDebug.VioletColor));
                    return;
                }

                if (World.IsNearestRifleVisible)
                {
                    GoPickup(World.NearestRifle);
                    DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/World.IsNearestRifleVisible/GoPickup(World.NearestRifle);", new Vec2(), 2, CustomDebug.VioletColor));

                    return;
                }
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Rifle && World.IsNearestSniperVisible)
            {
                GoPickup(World.NearestSniper);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "ChangeWeapon/Me.WeaponType/GoPickup(World.NearestSniper);", new Vec2(), 2, CustomDebug.VioletColor));
            }
        }

        private void AttackEnemy()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            // See anyone and have ammo
            if (!World.IsNearestEnemyVisible || Me.IsAmmoEmpty)
            {
                return;
            }

            // Has distance and clear vision
            if (Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy) &&
                Measurer.IsClearVisible(Me, World.NearestEnemy) &&
                Me.IsAimed)
            {
                // Shoot
                ComeToAim(World.NearestEnemy, true);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "AttackEnemy/Measurer.IsClearVisible(Me, World.NearestEnemy)/ComeToAim(World.NearestEnemy, true)", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            if (Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy))
            {
                // Aim
                ComeToAim(World.NearestEnemy);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "AttackEnemy/ComeToAim(World.NearestEnemy)", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            GoTo(World.NearestEnemy);
            DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "AttackEnemy/GoTo(World.NearestEnemy);", new Vec2(), 2, CustomDebug.VioletColor));
        }

        private void GoToTarget()
        {
            if (Commands.Any(c => c.Key == Me.Id))
            {
                return;
            }

            if (World.IsImDeputy && World.IsFarFromCommander)
            {
                GoTo(World.Commander);
                DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "GoToTarget/GoTo(World.MyTeammates.First()))", new Vec2(), 2, CustomDebug.VioletColor));
                return;
            }

            GoTo(Measurer.GetZoneBorderPoint(Me));
            DebugInterface?.Add(new DebugData.PlacedText(World.Me.Position, "GoToTarget/GoTo(Measurer.GetZoneBorderPoint(Me));", new Vec2(), 2, CustomDebug.VioletColor));
        }

        #endregion

        #region Actions

        private void GoBackFrom(CustomUnit unit, bool withShoot = false)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, unit.Position, unit.Velocity);
            var actionAim = new ActionOrder.Aim(withShoot);

            Commands.Add(Me.Id, new UnitOrder(Measurer.GetRandomVector(), movement.direction, actionAim));
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
                             ? new UnitOrder(Measurer.GetBulletsDodgeVelocity(Me, World.NearestEnemy), Measurer.GetVectorTo(Me.Position, World.NearestEnemy.Position), actionUseShieldPotion)
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