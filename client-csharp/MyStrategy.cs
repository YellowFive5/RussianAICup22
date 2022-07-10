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
        private Measurer Measurer { get; set; }
        private World World { get; }
        private MyUnit Me => World.Me;
        private DebugInterface DebugInterface { get; set; }
        public Dictionary<int, UnitOrder> Command { get; set; }

        private readonly bool debugPrint = false;

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
            // DebugInterface.Add(new DebugData.PlacedText(World.Me.Position, World.Me.Potions.ToString(), new Vec2(), 5, CustomDebug.VioletColor));
            // DebugInterface.Add(new DebugData.Ring(World.NearestRifleAmmoLoot.Position, 2, 2, CustomDebug.VioletColor));
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
                Go(Measurer.GetZoneBorderPoint(Me));
            }
        }

        private void Heel()
        {
            if (Command.Any())
            {
                return;
            }

            if (Me.IsShieldInjured)
            {
                // Under potential hit
                if (Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me, 0.75))
                {
                    // Covered
                    if (!Measurer.IsClearVisible(World.NearestEnemy, Me))
                    {
                        // Heel
                        TakePotionIfHave();
                        return;
                    }

                    // Can I hit
                    if (Measurer.IsClearVisible(Me, World.NearestEnemy) &&
                        Me.IsAimed &&
                        Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy))
                    {
                        // Shoot
                        RunAwayFrom(World.NearestEnemy, true);
                        return;
                    }

                    // Aim
                    RunAwayFrom(World.NearestEnemy);
                    return;
                }

                // Heel
                TakePotionIfHave();
                return;
            }

            if (Me.IsShieldDamaged)
            {
                if (!Measurer.IsDistanceAllowToHit(World.NearestEnemy, Me, 0.5))
                {
                    TakePotionIfHave();
                }
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

            // Be afraid of sniper when no sniper // todo test
            if (Me.WeaponType is not WeaponLootItem.WeaponType.Sniper &&
                World.IsNearestSniperEnemyVisible &&
                Measurer.IsDistanceAllowToHit(World.NearestSniperEnemy, Me, 1.1) &&
                Measurer.IsClearVisible(Me, World.NearestEnemy))
            {
                // Can I hit
                if (Me.IsAimed)
                {
                    // Shoot
                    RunAwayFrom(World.NearestSniperEnemy, true);
                    return;
                }

                // Aim
                RunAwayFrom(World.NearestSniperEnemy);
                return;
            }

            // No distance to hit - came to
            if (!Measurer.IsDistanceAllowToHit(Me, World.NearestEnemy))
            {
                Go(World.NearestEnemy);
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

            Go(Measurer.GetZoneBorderPoint(Me));
        }

        #endregion

        #region Actions

        private void RunAwayFrom(CustomUnit unit, bool withShoot = false)
        {
            // var movement = Measurer.GetSmartMovement(Me, unit.Position, true);

            var movement = Measurer.GetAdvancedTargetDirectionTo(Me, unit);

            if (debugPrint) //todo debug smartaim
            {
                // DebugInterface.Add(new DebugData.PolyLine(new[]
                //                                           {
                //                                               Me.Position,
                //                                               smartAim
                //                                           },
                //                                           0.3,
                //                                           CustomDebug.VioletColor));
            }

            var actionAim = new ActionOrder.Aim(withShoot);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, actionAim) }, };
            // Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, smartAim, actionAim) }, };
        }

        private void GoPickup(CustomItem item)
        {
            var movement = Measurer.GetSmartMovement(Me, item.Position);
            // var movement = Measurer.GetAdvancedTargetDirectionTo(Me, item.Position);
            var actionPickup = new ActionOrder.Pickup(item.Id);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, actionPickup) }, };
        }

        private void Go(CustomUnit item)
        {
            var movement = Measurer.GetAdvancedTargetDirectionTo(Me, item);
            // var movement = Measurer.GetSmartMovement(Me, item.Position);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, null) }, };
        }

        private void Go(Vec2 point)
        {
            var movement = Measurer.GetSmartMovement(Me, point);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(movement.velocity, movement.direction, null) }, };
        }

        private void ComeToAim(CustomUnit unit, bool withShot = false, bool inverted = false)
        {
            // var smartAim = Measurer.GetSmartMovement(Me, unit.Position, inverted);

            var smartAim = Measurer.GetAdvancedTargetDirectionTo(Me, unit);

            if (debugPrint) //todo debug smartaim
            {
                DebugInterface.Add(new DebugData.PolyLine(new[]
                                                          {
                                                              Me.Position,
                                                              new Vec2(smartAim.direction.X + Me.Position.X,
                                                                       smartAim.direction.Y + Me.Position.Y)
                                                          },
                                                          0.3,
                                                          CustomDebug.VioletColor));
            }

            var actionAim = new ActionOrder.Aim(withShot);
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(smartAim.velocity, smartAim.direction, actionAim) }, };
        }

        private void TakePotionIfHave()
        {
            if (Me.IsPotionsEmpty)
            {
                return;
            }

            var actionUseShieldPotion = new ActionOrder.UseShieldPotion();
            Command = new Dictionary<int, UnitOrder> { { Me.Id, new UnitOrder(Me.Unit.Velocity, Me.Unit.Direction, actionUseShieldPotion) }, };
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