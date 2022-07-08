#region Usings

using System.IO;

#endregion

namespace AiCup22.Model
{
    /// <summary>
    /// Current game's state
    /// </summary>
    public struct Game
    {
        /// <summary>
        /// Your player's id
        /// </summary>
        public int MyId { get; set; }

        /// <summary>
        /// List of players (teams)
        /// </summary>
        public Player[] Players { get; set; }

        /// <summary>
        /// Current tick
        /// </summary>
        public int CurrentTick { get; set; }

        /// <summary>
        /// List of units visible by your team
        /// </summary>
        public Unit[] Units { get; set; }

        /// <summary>
        /// List of loot visible by your team
        /// </summary>
        public Loot[] Loot { get; set; }

        /// <summary>
        /// List of projectiles visible by your team
        /// </summary>
        public Projectile[] Projectiles { get; set; }

        /// <summary>
        /// Current state of game zone
        /// </summary>
        public Zone Zone { get; set; }

        /// <summary>
        /// List of sounds heard by your team during last tick
        /// </summary>
        public Sound[] Sounds { get; set; }

        public Game(int myId, Player[] players, int currentTick, Unit[] units, Loot[] loot, Projectile[] projectiles, Zone zone, Sound[] sounds)
        {
            MyId = myId;
            Players = players;
            CurrentTick = currentTick;
            Units = units;
            Loot = loot;
            Projectiles = projectiles;
            Zone = zone;
            Sounds = sounds;
        }

        /// <summary> Read Game from reader </summary>
        public static Game ReadFrom(BinaryReader reader)
        {
            var result = new Game();
            result.MyId = reader.ReadInt32();
            result.Players = new Player[reader.ReadInt32()];
            for (int playersIndex = 0; playersIndex < result.Players.Length; playersIndex++)
            {
                result.Players[playersIndex] = Player.ReadFrom(reader);
            }

            result.CurrentTick = reader.ReadInt32();
            result.Units = new Unit[reader.ReadInt32()];
            for (int unitsIndex = 0; unitsIndex < result.Units.Length; unitsIndex++)
            {
                result.Units[unitsIndex] = Unit.ReadFrom(reader);
            }

            result.Loot = new Loot[reader.ReadInt32()];
            for (int lootIndex = 0; lootIndex < result.Loot.Length; lootIndex++)
            {
                result.Loot[lootIndex] = Model.Loot.ReadFrom(reader);
            }

            result.Projectiles = new Projectile[reader.ReadInt32()];
            for (int projectilesIndex = 0; projectilesIndex < result.Projectiles.Length; projectilesIndex++)
            {
                result.Projectiles[projectilesIndex] = Projectile.ReadFrom(reader);
            }

            result.Zone = Zone.ReadFrom(reader);
            result.Sounds = new Sound[reader.ReadInt32()];
            for (int soundsIndex = 0; soundsIndex < result.Sounds.Length; soundsIndex++)
            {
                result.Sounds[soundsIndex] = Sound.ReadFrom(reader);
            }

            return result;
        }

        /// <summary> Write Game to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(MyId);
            writer.Write(Players.Length);
            foreach (var playersElement in Players)
            {
                playersElement.WriteTo(writer);
            }

            writer.Write(CurrentTick);
            writer.Write(Units.Length);
            foreach (var unitsElement in Units)
            {
                unitsElement.WriteTo(writer);
            }

            writer.Write(Loot.Length);
            foreach (var lootElement in Loot)
            {
                lootElement.WriteTo(writer);
            }

            writer.Write(Projectiles.Length);
            foreach (var projectilesElement in Projectiles)
            {
                projectilesElement.WriteTo(writer);
            }

            Zone.WriteTo(writer);
            writer.Write(Sounds.Length);
            foreach (var soundsElement in Sounds)
            {
                soundsElement.WriteTo(writer);
            }
        }

        /// <summary> Get string representation of Game </summary>
        public override string ToString()
        {
            string stringResult = "Game { ";
            stringResult += "MyId: ";
            stringResult += MyId.ToString();
            stringResult += ", ";
            stringResult += "Players: ";
            stringResult += "[ ";
            int playersIndex = 0;
            foreach (var playersElement in Players)
            {
                if (playersIndex != 0)
                {
                    stringResult += ", ";
                }

                stringResult += playersElement.ToString();
                playersIndex++;
            }

            stringResult += " ]";
            stringResult += ", ";
            stringResult += "CurrentTick: ";
            stringResult += CurrentTick.ToString();
            stringResult += ", ";
            stringResult += "Units: ";
            stringResult += "[ ";
            int unitsIndex = 0;
            foreach (var unitsElement in Units)
            {
                if (unitsIndex != 0)
                {
                    stringResult += ", ";
                }

                stringResult += unitsElement.ToString();
                unitsIndex++;
            }

            stringResult += " ]";
            stringResult += ", ";
            stringResult += "Loot: ";
            stringResult += "[ ";
            int lootIndex = 0;
            foreach (var lootElement in Loot)
            {
                if (lootIndex != 0)
                {
                    stringResult += ", ";
                }

                stringResult += lootElement.ToString();
                lootIndex++;
            }

            stringResult += " ]";
            stringResult += ", ";
            stringResult += "Projectiles: ";
            stringResult += "[ ";
            int projectilesIndex = 0;
            foreach (var projectilesElement in Projectiles)
            {
                if (projectilesIndex != 0)
                {
                    stringResult += ", ";
                }

                stringResult += projectilesElement.ToString();
                projectilesIndex++;
            }

            stringResult += " ]";
            stringResult += ", ";
            stringResult += "Zone: ";
            stringResult += Zone.ToString();
            stringResult += ", ";
            stringResult += "Sounds: ";
            stringResult += "[ ";
            int soundsIndex = 0;
            foreach (var soundsElement in Sounds)
            {
                if (soundsIndex != 0)
                {
                    stringResult += ", ";
                }

                stringResult += soundsElement.ToString();
                soundsIndex++;
            }

            stringResult += " ]";
            stringResult += " }";
            return stringResult;
        }
    }
}