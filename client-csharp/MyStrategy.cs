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
        private Measurer Measurer { get; set; }
        private World World { get; }
        private MyUnit Me => World.Me;
        private DebugInterface DebugInterface { get; set; }
        public Dictionary<int, UnitOrder> Command { get; set; }

        private readonly bool debugPrint = false;
        private int playerTurn;

        public MyStrategy(Constants constants)
        {
            World = new World(constants);
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            var players = game.Units.Count(u => u.PlayerId == game.MyId);
            playerTurn = playerTurn >= players - 1
                             ? 0
                             : playerTurn + 1;

            DebugInterface = debugInterface;
            Command = new Dictionary<int, UnitOrder>();

            Measurer = new Measurer(World, DebugInterface);

            World.Scan(game, playerTurn);

            ChooseAction();

            return new Order(Command);
        }

        private void ChooseAction()
        {
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Potions.ToString(), new Vec2(), 5, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestSniperAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.PolyLine(new[] { new Vec2(50,100), new Vec2(0,50) }, 5, CustomDebug.GreenColor));

            ReturnInZone();

            Heel();

            ProcessItems();

            AttackEnemy();

            GoToTarget();
        }

        #region Behaviour

        private void ProcessItems()
        {
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

            // NormalOrder
            CollectPotions();

            CollectAmmo();

            ChangeWeapon();
        }

        private void ReturnInZone()
        {
            if (Command.Any())
            {
                return;
            }

            if (World.NearToOutOfZone)
            {
                GoTo(Measurer.GetZoneBorderPoint(Me));
            }
        }

        private void Heel()
        {
            if (Command.Any())
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
                TakePotion();
                return;
            }

            if (!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me)
                ||
                !Measurer.IsClearVisible(World.NearestEnemy, Me))
            {
                // Heel
                TakePotion();
            }
        }

        private void CollectPotions()
        {
            if (Command.Any())
            {
                return;
            }

            if (Me.IsPotionsEmpty &&
                World.IsNearestShieldLootItemVisible)
            {
                GoPickup(World.NearestShieldLootItem);
                return;
            }

            if (Me.NeedToCollectPotions &&
                World.IsNearestShieldLootItemVisible &&
                (!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me) ||
                 !Measurer.IsClearVisible(World.NearestEnemy, Me)))
            {
                GoPickup(World.NearestShieldLootItem);
            }
        }

        private void CollectAmmo()
        {
            if (Command.Any())
            {
                return;
            }

            if (Me.IsAmmoEmpty && World.IsNearestActiveAmmoVisible())
            {
                GoPickup(World.GetNearestActiveAmmoLoot());
                return;
            }

            if (Me.NeedToCollectAmmo &&
                World.IsNearestActiveAmmoVisible() &&
                (!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me) ||
                 !Measurer.IsClearVisible(World.NearestEnemy, Me))
               )
            {
                GoPickup(World.GetNearestActiveAmmoLoot());
            }
        }

        private void AttackEnemy()
        {
            if (Command.Any())
            {
                return;
            }

            // See anyone and have ammo
            if (!World.IsNearestEnemyVisible || Me.IsAmmoEmpty)
            {
                return;
            }

            // No distance to hit - came to
            if (!Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy))
            {
                GoTo(World.NearestEnemy);
                return;
            }

            // Has distance and clear vision
            if (Measurer.IsClearVisible(Me, World.NearestEnemy) &&
                Me.IsAimed)
            {
                // Shoot
                ComeToAim(World.NearestEnemy, true, true);
                return;
            }

            // Aim
            ComeToAim(World.NearestEnemy);
        }

        private void ChangeWeapon()
        {
            if (Command.Any())
            {
                return;
            }

            if (Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me, 0.9) &&
                Measurer.IsClearVisible(World.NearestEnemy, Me))
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

        private void GoToTarget()
        {
            if (Command.Any())
            {
                return;
            }

            GoTo(Measurer.GetZoneBorderPoint(Me));
        }

        #endregion

        #region Actions

        private void GoBackFrom(CustomUnit unit, bool withShoot = false)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, unit.Position, unit.Velocity);
            var actionAim = new ActionOrder.Aim(withShoot);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(Measurer.GetRandomVec(), movement.direction, actionAim) }, };
        }

        private void GoPickup(CustomItem item)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, item.Position);
            var actionPickup = new ActionOrder.Pickup(item.Id);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, actionPickup) }, };
        }

        private void GoTo(CustomUnit unit)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, unit.Position, unit.Velocity);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, null) }, };
        }

        private void GoTo(Vec2 point)
        {
            var movement = Measurer.GetSmartDirectionVelocity(Me, point);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, Measurer.GetInvertedVec(Me.Direction), null) }, };
        }

        private void ComeToAim(CustomUnit unit, bool withShot = false, bool inverted = false)
        {
            var smartAim = Measurer.GetSmartDirectionVelocity(Me, unit.Position, unit.Velocity, inverted);
            var actionAim = new ActionOrder.Aim(withShot);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(smartAim.velocity, smartAim.direction, actionAim) }, };
        }

        private void TakePotion()
        {
            var actionUseShieldPotion = new ActionOrder.UseShieldPotion();
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(Measurer.GetRandomVec(), Me.Direction, actionUseShieldPotion) }, };
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