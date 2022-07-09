#region Usings

using System.Collections.Generic;
using System.Linq;
using AiCup22.CustomModel;
using AiCup22.Model;

#endregion

namespace AiCup22
{
    public class MyStrategy
    {
        private World World { get; }
        private MyUnit Me => World.Me;
        private DebugInterface DebugInterface { get; set; }
        public Dictionary<int, UnitOrder> Command { get; set; }
        public MyStrategy(Constants constants)
        {
            World = new World(constants);
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            DebugInterface = debugInterface;
            Command = new Dictionary<int, UnitOrder>();

            World.Scan(game);

            ChooseAction();

            return new Order(Command);
        }

        private void ChooseAction()
        {
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Ammo.ToString(), new Vec2(), 5, CustomDebug.BlueColor));
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Potions.ToString(), new Vec2(), 5, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestRifleAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestSniperAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));

            ReturnInZone();

            GoHeel();

            TakePotions();

            TakeAmmo();

            ChangeWeapon();

            AttackEnemy();

            GoToTarget();
            
            }

        #region Behaviour

        private void GoToTarget()
        {
            if (Command.Any())
            {
                return;
            }

            Go(World.ZoneCenter);
        }

        private void AttackEnemy()
        {
            if (Command.Any())
            {
                return;
            }

            if (!World.IsNearestEnemyVisible || Me.IsAmmoEmpty)
            {
                return;
            }

            // More than one
            if (World.EnemyUnits.Count > 1)
            {
                RunAwayFrom(World.NearestEnemy);
                return;
            }

            // Can't hit
            if (!Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy))
            {
                Go(World.NearestEnemy);
                return;
            }

            // Aim
            if (!Me.IsAimed)
            {
                CameToAim(World.NearestEnemy);
                return;
            }

            // Shoot
            CameToAim(World.NearestEnemy, true);
        }

        private void ChangeWeapon()
        {
            if (Command.Any())
            {
                return;
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Pistol)
            {
                if (World.IsNearestSniperVisible)
                {
                    GoPickup(World.NearestSniper);
                    return;
                }

                if (World.IsNearestRifleVisible)
                {
                    GoPickup(World.NearestRifle);
                    return;
                }
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Rifle && World.IsNearestSniperVisible)
            {
                GoPickup(World.NearestSniper);
            }
        }

        private void TakeAmmo()
        {
            if (Command.Any())
            {
                return;
            }

            if (Me.IsOutOfAmmo && World.IsNearestActiveAmmoVisible())
            {
                GoPickup(World.GetNearestActiveAmmoLoot());
            }
        }

        private void TakePotions()
        {
            if (Command.Any())
            {
                return;
            }

            if (Me.IsPotionsUnderHalf && World.IsNearestShieldLootItemVisible)
            {
                GoPickup(World.NearestShieldLootItem);
            }
        }

        private void GoHeel()
        {
            if (Command.Any())
            {
                return;
            }

            if (!Me.IsShieldInjured)
            {
                return;
            }

            // Can be hit
            if (World.IsNearestEnemyVisible && Measurer.IsHittableFromEnemy(Me, World.NearestEnemy))
            {
                RunAwayFrom(World.NearestEnemy);
                return;
            }

            // Heel
            if (!Me.IsPotionsEmpty)
            {
                TakePotion();
            }
        }

        private void ReturnInZone()
        {
            if (Command.Any())
            {
                return;
            }

            if (World.OutOfZone)
            {
                Go(World.ZoneCenter);
            }
        }

        #endregion

        #region Actions

        private void RunAwayFrom(CustomItem item, bool shotOut = true)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item.Position, true);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            var actionAim = shotOut
                                ? new ActionOrder.Aim(true)
                                : null;

            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, actionAim) }, };
        }

        private void GoPickup(CustomItem item)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item.Position);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            var actionPickup = new ActionOrder.Pickup(item.Id);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, actionPickup) }, };
        }

        private void Go(CustomItem item)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item.Position);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, null) }, };
        }

        private void Go(Vec2 item)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, null) }, };
        }

        private void CameToAim(CustomItem item, bool withShot = false)
        {
            var targetVelocity = Measurer.GetRandomVec();
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            var actionAim = new ActionOrder.Aim(withShot);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, actionAim) }, };
        }

        private void TakePotion()
        {
            var actionUseShieldPotion = new ActionOrder.UseShieldPotion();
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(Measurer.GetRandomVec(), Measurer.GetRandomVec(), actionUseShieldPotion) }, };
        }

        #endregion

        public void DebugUpdate(DebugInterface debugInterface)
        {
            debugInterface.Clear();

            // if (World.IsNearestEnemyVisible)
            // {
            //     debugInterface.Add(new DebugData.Ring(World.NearestEnemy.Position, 1, 1, CustomDebug.RedColor));
            // }

            // foreach (var enemy in World.EnemyUnits)
            // {
            //     debugInterface.Add(new DebugData.Ring(enemy.Position, 1, 1, CustomDebug.GreenColor));
            // }
            // if (World.IsNearestNearestShieldLootItemVisible)
            // {
            //     debugInterface.Add(new DebugData.Ring(World.NearestShieldLootItem.Position, 1, 1, CustomDebug.VioletColor));
            // }
            //
            // debugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Potions.ToString(), new Vec2(), 5, CustomDebug.RedColor));
            // foreach (var enemy in World.Sounds)
            // {
            //     debugInterface.Add(new DebugData.Ring(enemy.Position, 1, 1, CustomDebug.BlueColor));
            // }

            // if (World.IsNearestSoundHeard)
            // {
            //     debugInterface.Add(new DebugData.Ring(World.NearestSound.Position, 1, 1, CustomDebug.RedColor));
            // }

            // foreach (var shield in World.ShieldItems)
            // {
            //     debugInterface.Add(new DebugData.Ring(shield.Position, 1, 1, CustomDebug.VioletColor));
            // }
            //
            // debugInterface.Add(new DebugData.Ring(World.NearestRifle.Position, 1, 1, CustomDebug.RedColor));
            //
            // debugInterface.Add(new DebugData.PlacedText(Me.Position, Me.Ammo.ToString(), new Vec2(), 5, CustomDebug.RedColor));
            // debugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.NearestEnemy.Position.ToString(), new Vec2(), 5, CustomDebug.BlueColor));
            // debugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Unit.Aim.ToString(), new Vec2(), 5, CustomDebug.BlueColor));
        }

        public void Finish()
        {
        }
    }
}