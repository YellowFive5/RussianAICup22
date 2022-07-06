#region Usings

#endregion

namespace UsefullTips._19
{
    public class MyStrategy19
    {
        public MyStrategy19()
        {
            Around = new World();
        }

        private Game Game { get; set; }
        private Debug Debug { get; set; }
        private MyUnit Me { get; set; }
        private World Around { get; }
        private Role Player1Role { get; } = Role.Rocketman;
        private Role Player2Role { get; } = Role.Rifleman;
        private int Player1Id { get; set; }
        private int Player2Id { get; set; }

        public UnitAction GetAction(Unit unit, Game game, Debug debug)
        {
            Game = game;
            Debug = debug;

            if (Player1Id == 0)
            {
                Player1Id = unit.Id;
            }
            else if (Player2Id == 0)
            {
                Player2Id = unit.Id;
            }

            if (unit.Id == Player1Id)
            {
                Me = new MyUnit(unit) { Role = Player1Role };
            }
            else if (unit.Id == Player2Id)
            {
                Me = new MyUnit(unit) { Role = Player2Role };
            }

            WorldScanner.Scan(Game, Me, Around);

            ChooseAction();

            DebugWrite();

            return Me.NextAction.Action;
        }

        private void ChooseAction()
        {
            Me.NextAction = new CustomAction();

            if (Me.NeedHeel && Around.NearestHealthExist)
            {
                Action.GoHeel(Game, Me, Around);
                return;
            }

            if (!Me.RoleWeaponTaken)
            {
                Action.TakeRoleWeapon(Game, Me, Around);
                return;
            }

            if (Me.RLEquiped)
            {
                //Action.ShootEmWithRL(Game, Me, Around, Debug);
                Action.ShootEmWithRL(Game, Me, Around);
            }
            else
            {
                //Action.ShootEm(Game, Me, Around, Debug);
                Action.ShootEm(Game, Me, Around);
            }
        }

        private void DebugWrite()
        {
            //Debug.Draw(new CustomData.Log($"" +
            // $"Bullets count: {Around.Bullets.Count} | " +
            //  $"Nearest bullet: {(Around.NearestBullet != null ? $"{(int) Around.NearestBullet.Bullet.Position.X}/{(int) Around.NearestBullet.Bullet.Position.Y}/{(int) Around.NearestBullet.Distance}" : "-")} | " +
            //  $"Nearest bullet type: {(Around.NearestBullet != null ? $"{Around.NearestBullet.WeaponType}" : "-")} | " +
            //  $"Nearest bullet damage: {(Around.NearestBullet != null ? $"{Around.NearestBullet.Damage}" : "-")} | " +
            // $"Nearest enemy {(Around.NearestEnemy != null ? $"{(int) Around.NearestEnemy.Position.X}/{(int) Around.NearestEnemy.Position.Y}/{(int) Around.NearestEnemy.Distance}" : "-")} | " +
            //  $"Nearest enemy mine {(Around.NearestMine != null ? $"{(int) Around.NearestMine.Mine.Position.X}/{(int) Around.NearestMine.Mine.Position.Y}/{(int) Around.NearestMine.Distance}" : "-")} | " +
            //  $"Nearest weapon {(Around.NearestWeapon != null ? Around.NearestWeapon.WeaponType.ToString() : "-")} | " +
            //  $"Nearest weapon position {(Around.NearestWeapon != null ? $"{(int) Around.NearestWeapon.Item.Position.X}/{(int) Around.NearestWeapon.Item.Position.Y}/{(int) Around.NearestWeapon.Distance}" : "-")} | " +
            //  $"Nearest pistol {(Around.NearestPistol != null ? $"{(int) Around.NearestPistol.Item.Position.X}/{(int) Around.NearestPistol.Item.Position.Y}/{(int) Around.NearestPistol.Distance}" : "-")} | " +
            //  $"Nearest rifle {(Around.NearestRifle != null ? $"{(int) Around.NearestRifle.Item.Position.X}/{(int) Around.NearestRifle.Item.Position.Y}/{(int) Around.NearestRifle.Distance}" : "-")} | " +
            //  $"Nearest RL {(Around.NearestRLauncher != null ? $"{(int) Around.NearestRLauncher.Item.Position.X}/{(int) Around.NearestRLauncher.Item.Position.Y}/{(int) Around.NearestRLauncher.Distance}" : "-")} | " +
            //  $"Nearest health {(Around.NearestHealth != null ? $"{(int) Around.NearestHealth.Item.Position.X}/{(int) Around.NearestHealth.Item.Position.Y}/{(int) Around.NearestHealth.Distance}" : "-")} | " +
            //  $"Nearest mine loot {(Around.NearestMineL != null ? $"{(int) Around.NearestMineL.Item.Position.X}/{(int) Around.NearestMineL.Item.Position.Y}/{(int) Around.NearestMineL.Distance}" : "-")} | " +
            //  $"Me has weapon: {Me.HasWeapon} | " +
            //  $"My weapon type: {(Me.HasWeapon ? $"{Me.Weapon.Value.Typ}" : "-")} | " +
            // $"My health: {Me.Health} | " +
            // $"Nearest enemy health: {Around.NearestEnemy.Health} | " +
            //  $"Nearest enemy has weapon: {Around.NearestEnemy.HasWeapon} | " +
            //  $"Nearest enemy weapon type: {(Around.NearestEnemy.HasWeapon ? $"{Around.NearestEnemy.Weapon.Value.Typ}" : "-")} | " +
            //  $"My magazine ammo: {(Me.HasWeapon ? $"{Me.Weapon.Value.Magazine}" : "-")} | " +
            // $"My tile Top: {Around.NextTileT} | " +
            //  $"My tile Bottom: {Around.NextTileB} | " +
            //  $"My tile Left: {Around.NextTileL} | " +
            //  $"My tile Right: {Around.NextTileR} | " +
            // $"My under platform: {Me.UnderPlatform} | " +
            // $"Nearest enemy under platform: {Around.NearestEnemy.UnderPlatform} | " +
            // $"Nearest enemy tile Top: {Around.NearestEnemy.NextTileT} | " +
            //  $"Nearest enemy tile Bottom: {Around.NearestEnemy.NextTileB} | " +
            //  $"Nearest enemy tile Left: {Around.NearestEnemy.NextTileL} | " +
            //  $"Nearest enemy tile Right: {Around.NearestEnemy.NextTileR} | " +
            //  $"Me.OnGround: {Me.OnGround} | " +
            //  $"Me.OnGround: {Me.OnGround} | " +
            //  $"Me.OnLadder: {Me.OnLadder} | " +
            //  $"Me.Stand: {Me.Stand} | " +
            //  $"Me.SeeRight: {Me.SeeRight} | " +
            //  $"Me.SeeLeft: {Me.SeeLeft} | " +
            //  $"Me.Mines: {Me.Mines} | " +
            //  $"Me.CanPlantMine: {Me.CanPlantMine} | " +
            //  $"{Game.Level.Tiles[39][29]}" +
            // $"MeOnGround: {Around.NearestEnemy.Man.OnGround}" +
            // $"Jump: {Me.Jump}" +
            // $"JumpDown: {Me.JumpDown}" +
            // $"BestWeapon: {Around.BestWeapon}" +
            // $"BestWeapon taken: {Me.BestWeaponTaken}" +
            // $"MyWeaponSpread: {Me.WeaponSpread}" +
            // $"EnemyWeaponSpread: {Around.NearestEnemy.WeaponSpread}" +
            //$"Distance to teammate: {Measure.GetDistance(Me.Position,Around.Teammate.Position)}" +
            //$"TeammateX: {Around.Teammate.Position.X}" +
            //$"TeammateY: {Around.Teammate.Position.Y}" +
            //                              $"Me role: {Me.Role}" +
            //                              $"Teammate role: {Around.Teammate.Role}" +
            //                              ""));

            //Debug.Draw(new CustomData.PlacedText("+",
            //                                     new Vec2Float((float)Around.Teammate.Position.X, (float)Around.Teammate.Position.Y),
            //                                     TextAlignment.Center,
            //                                     20,
            //                                     Constants.GreenColor));
            //Debug.Draw(new CustomData.PlacedText("+",
            //                                     new Vec2Float((float)Around.Teammate.Position.X, (float)Around.Teammate.Position.Y+1),
            //                                     TextAlignment.Center,
            //                                     20,
            //                                     Constants.GreenColor));

            //var visibleAndAimed =
            //    Measure.RLAimed(Me, Around.NearestEnemy.Position, Game, Debug) &
            //    Measure.IsStraightVisible(Me, Around.NearestEnemy.Position, Game, Debug);

            //Debug.Draw(new CustomData.Line(new Vec2Float((float) Me.Position.X,
            //                                             (float) Me.Position.Y + (float) Me.Size.Y / 2),
            //                               new Vec2Float((float) Around.NearestEnemy.Position.X,
            //                                             (float) Around.NearestEnemy.Position.Y + (float) Around.NearestEnemy.Size.Y / 2),
            //                               0.1f,
            //                               visibleAndAimed
            //                                   ? Constants.GreenColor
            //                                   : Constants.RedColor));
        }
    }
}