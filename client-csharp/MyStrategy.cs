#region Usings

using System.Collections.Generic;
using AiCup22.CustomModel;
using AiCup22.Debugging;
using AiCup22.Model;

#endregion

namespace AiCup22
{
    public class MyStrategy
    {
        private World World { get; }

        public MyStrategy(Constants constants)
        {
            World = new World(constants);
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            World.Scan(game);
            var targetVelocity = new Vec2();
            var targetDirection = new Vec2();
            var actionAim = new ActionOrder.Aim(false);
            var actionPickup = new ActionOrder.Pickup();
            var unitOrder = new UnitOrder();

            if (World.IsNearestNearestShieldLootItemVisible && !World.Me.IsPotionsFull)
            {
                targetVelocity = Measurer.GetTargetDirectionTo(World.Me.Position, World.NearestShieldLootItem.Position);

                targetDirection = Measurer.GetTargetVelocityTo(World.Me.Position, World.NearestShieldLootItem.Position);

                actionAim = new ActionOrder.Aim(false);
                actionPickup = new ActionOrder.Pickup(World.NearestShieldLootItem.Id);
            }

            unitOrder = new UnitOrder(targetVelocity, targetDirection, actionPickup);
            var myCommand = new Dictionary<int, UnitOrder> { { World.Me.Id, unitOrder } };
            return new Order(myCommand);
        }

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
            // debugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.AmmoItems.Count.ToString(), new Vec2(), 5, CustomDebug.RedColor));
            // debugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.NearestEnemy.Position.ToString(), new Vec2(), 5, CustomDebug.BlueColor));
        }

        public void Finish()
        {
        }
    }
}