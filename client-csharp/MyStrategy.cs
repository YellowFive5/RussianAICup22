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

        private readonly bool debugPrint = true;

        public MyStrategy(Constants constants)
        {
            World = new World(constants);
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            DebugInterface = debugInterface;
            Command = new Dictionary<int, UnitOrder>();

            Measurer = new Measurer(World, DebugInterface);

            World.Scan(game);

            ChooseAction();

            return new Order(Command);
        }

        private void ChooseAction()
        {
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Ammo.ToString(), new Vec2(), 5, CustomDebug.RedColor));
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Potions.ToString(), new Vec2(), 5, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestRifleAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestSniperAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.PolyLine(new[] { new Vec2(50,100), new Vec2(0,50) }, 5, CustomDebug.GreenColor));

            ReturnInZone();

            GoHeel();

            TakePotions();

            TakeAmmo();

            AttackEnemy();

            ChangeWeapon();

            GoToTarget();
        }

        #region Behaviour

        private void ReturnInZone()
        {
            if (Command.Any())
            {
                return;
            }

            if (World.NearToOutOfZone)
            {
                Go(Measurer.GetZoneBorderPoint(Me, World.ZoneCenter, World.ZoneRadius));
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

            // Under hit
            if (World.IsNearestEnemyVisible && Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me))
            {
                // Can I hit
                if (Measurer.IsClearVisible(Me, World.NearestEnemy, World.Objects) &&
                    Me.IsAimed &&
                    Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy))
                {
                    // Shoot
                    RunAwayFrom(World.NearestEnemy, true);
                    return;
                }

                // Can heel not under fire
                if (!Measurer.IsClearVisible(World.NearestEnemy, Me, World.Objects) ||
                    !Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me))
                {
                    // Heel
                    TakePotionIfHave();
                    return;
                }

                // Aim
                RunAwayFrom(World.NearestEnemy);
                return;
            }

            // Heel
            TakePotionIfHave();
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

            // Be afraid of sniper when no sniper
            if (World.IsNearestEnemyVisible &&
                World.NearestEnemy.WeaponType is WeaponLootItem.WeaponType.Sniper &&
                Me.WeaponType is not WeaponLootItem.WeaponType.Sniper)
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

            if (Measurer.IsClearVisible(Me, World.NearestEnemy, World.Objects) &&
                Me.IsAimed)
            {
                // Shoot
                ComeToAim(World.NearestEnemy, true);
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

            Go(Measurer.GetZoneBorderPoint(Me, World.ZoneCenter, World.ZoneRadius));
        }

        #endregion

        #region Actions

        private void RunAwayFrom(CustomUnit unit, bool withShoot = false)
        {
            var movement = Measurer.GetSmartMovement(Me, unit.Position, true);
            var actionAim = new ActionOrder.Aim(withShoot);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, actionAim) }, };
        }

        private void GoPickup(CustomItem item)
        {
            var movement = Measurer.GetSmartMovement(Me, item.Position);
            var actionPickup = new ActionOrder.Pickup(item.Id);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, actionPickup) }, };
        }

        private void Go(CustomItem item)
        {
            var movement = Measurer.GetSmartMovement(Me, item.Position);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, null) }, };
        }

        private void Go(Vec2 point)
        {
            var movement = Measurer.GetSmartMovement(Me, point);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, null) }, };
        }

        private void ComeToAim(CustomUnit unit, bool withShot = false)
        {
            var targetVelocity = Measurer.GetRandomVec();
            var movement = Measurer.GetSmartMovement(Me, unit.Position);

            // if (debugPrint) //todo debug smartaim
            // {
            //     DebugInterface.Add(new DebugData.PolyLine(new[]
            //                                               {
            //                                                   Me.Position,
            //                                                   new Vec2(targetDirection.X + Me.Position.X,
            //                                                            targetDirection.Y + Me.Position.Y)
            //                                               },
            //                                               0.3,
            //                                               CustomDebug.VioletColor));
            // }

            var actionAim = new ActionOrder.Aim(withShot);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, actionAim) }, };
        }

        private void TakePotionIfHave()
        {
            if (Me.IsPotionsEmpty)
            {
                return;
            }

            var actionUseShieldPotion = new ActionOrder.UseShieldPotion();
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(Measurer.GetRandomVec(), Measurer.GetRandomVec(), actionUseShieldPotion) }, };
        }

        #endregion

        public void DebugUpdate(int displayedTick, DebugInterface debugInterface)
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