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
            if (!Me.IsPotionsFull && World.IsNearestShieldLootItemVisible)
            {
                return GoPickup(World.NearestShieldLootItem);
            }

            return new Order(new Dictionary<int, UnitOrder>());
        }

        private Order GoPickup(CustomItem item)
        {
            var targetVelocity = Measurer.GetTargetVelocityTo(Me.Position, item.Position);
            var targetDirection = Measurer.GetTargetDirectionTo(Me.Position, item.Position);
            var actionPickup = new ActionOrder.Pickup(item.Id);
            var myCommand = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(targetVelocity, targetDirection, actionPickup) }, };

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
            // debugInterface.Add(new DebugData.PlacedText(Me.Position, Me.Ammo.ToString(), new Vec2(), 5, CustomDebug.RedColor));
            // debugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.NearestEnemy.Position.ToString(), new Vec2(), 5, CustomDebug.BlueColor));
            // debugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Unit.Aim.ToString(), new Vec2(), 5, CustomDebug.BlueColor));
        }

        public void Finish()
        {
        }
    }
}