#region Usings

using System.IO;

#endregion

namespace AiCup22.Model
{
    /// <summary>
    /// Weapon projectile
    /// </summary>
    public struct Projectile
    {
        /// <summary>
        /// Unique id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Index of the weapon this projectile was shot from (starts with 0)
        /// </summary>
        public int WeaponTypeIndex { get; set; }

        /// <summary>
        /// Id of unit who made the shot
        /// </summary>
        public int ShooterId { get; set; }

        /// <summary>
        /// Id of player (team), whose unit made the shot
        /// </summary>
        public int ShooterPlayerId { get; set; }

        /// <summary>
        /// Current position
        /// </summary>
        public Vec2 Position { get; set; }

        /// <summary>
        /// Projectile's velocity
        /// </summary>
        public Vec2 Velocity { get; set; }

        /// <summary>
        /// Left time of projectile's life
        /// </summary>
        public double LifeTime { get; set; }

        public Projectile(int id, int weaponTypeIndex, int shooterId, int shooterPlayerId, Vec2 position, Vec2 velocity, double lifeTime)
        {
            Id = id;
            WeaponTypeIndex = weaponTypeIndex;
            ShooterId = shooterId;
            ShooterPlayerId = shooterPlayerId;
            Position = position;
            Velocity = velocity;
            LifeTime = lifeTime;
        }

        /// <summary> Read Projectile from reader </summary>
        public static Projectile ReadFrom(BinaryReader reader)
        {
            var result = new Projectile();
            result.Id = reader.ReadInt32();
            result.WeaponTypeIndex = reader.ReadInt32();
            result.ShooterId = reader.ReadInt32();
            result.ShooterPlayerId = reader.ReadInt32();
            result.Position = Vec2.ReadFrom(reader);
            result.Velocity = Vec2.ReadFrom(reader);
            result.LifeTime = reader.ReadDouble();
            return result;
        }

        /// <summary> Write Projectile to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(WeaponTypeIndex);
            writer.Write(ShooterId);
            writer.Write(ShooterPlayerId);
            Position.WriteTo(writer);
            Velocity.WriteTo(writer);
            writer.Write(LifeTime);
        }

        /// <summary> Get string representation of Projectile </summary>
        public override string ToString()
        {
            string stringResult = "Projectile { ";
            stringResult += "Id: ";
            stringResult += Id.ToString();
            stringResult += ", ";
            stringResult += "WeaponTypeIndex: ";
            stringResult += WeaponTypeIndex.ToString();
            stringResult += ", ";
            stringResult += "ShooterId: ";
            stringResult += ShooterId.ToString();
            stringResult += ", ";
            stringResult += "ShooterPlayerId: ";
            stringResult += ShooterPlayerId.ToString();
            stringResult += ", ";
            stringResult += "Position: ";
            stringResult += Position.ToString();
            stringResult += ", ";
            stringResult += "Velocity: ";
            stringResult += Velocity.ToString();
            stringResult += ", ";
            stringResult += "LifeTime: ";
            stringResult += LifeTime.ToString();
            stringResult += " }";
            return stringResult;
        }
    }
}