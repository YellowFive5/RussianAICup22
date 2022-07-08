#region Usings

using System.Collections.Generic;
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

        public MyStrategy(Constants constants)
        {
            World = new World(constants);
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            DebugInterface = debugInterface;

            World.Scan(game);

            return ChooseAction();
        }

        private Order ChooseAction()
        {
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Ammo.ToString(), new Vec2(), 5, CustomDebug.BlueColor));
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Potions.ToString(), new Vec2(), 5, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestRifleAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestSniperAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));

            if (Me.IsShieldInjured && !Me.IsPotionsEmpty)
            {
                return TakePotion();
            }

            if (Me.IsPotionsUnderHalf && World.IsNearestShieldLootItemVisible)
            {
                return GoPickup(World.NearestShieldLootItem);
            }

            if (Me.IsAmmoUnderHalf && World.IsNearestActiveAmmoVisible())
            {
                return GoPickup(World.GetNearestActiveAmmoLoot());
            }

            if (World.IsNearestEnemyVisible && !Me.IsAmmoEmpty)
            {
                if (!Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy))
                {
                    return Go(World.NearestEnemy);
                }

                if (!Me.IsAimed)
                {
                    return GoAim(World.NearestEnemy);
                }

                return GoAim(World.NearestEnemy, true);
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Pistol)
            {
                if (World.IsNearestSniperVisible)
                {
                    return GoPickup(World.NearestSniper);
                }

                if (World.IsNearestRifleVisible)
                {
                    return GoPickup(World.NearestRifle);
                }
            }

            if (Me.WeaponType == WeaponLootItem.WeaponType.Rifle && World.IsNearestSniperVisible)
            {
                return GoPickup(World.NearestSniper);
            }

            return Go(World.ZoneNextCenter);
        }

        #region Actions

        private Order GoPickup(CustomItem item)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item.Position);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            var actionPickup = new ActionOrder.Pickup(item.Id);
            var myCommand = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, actionPickup) }, };

            return new Order(myCommand);
        }

        private Order Go(CustomItem item)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item.Position);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            var myCommand = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, null) }, };

            return new Order(myCommand);
        }

        private Order Go(Vec2 item)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item);
            var myCommand = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, null) }, };

            return new Order(myCommand);
        }

        private Order GoAim(CustomItem item, bool withShot = false)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item.Position);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            var actionAim = new ActionOrder.Aim(withShot);
            var myCommand = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, actionAim) }, };

            return new Order(myCommand);
        }

        private Order TakePotion()
        {
            var actionUseShieldPotion = new ActionOrder.UseShieldPotion();
            var myCommand = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(new Vec2(), new Vec2(), actionUseShieldPotion) }, };

            return new Order(myCommand);
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