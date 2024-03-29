#region Usings

using System.IO;

#endregion

namespace AiCup22.Model
{
    /// <summary>
    /// An obstacle on the map
    /// </summary>
    public struct Obstacle
    {
        /// <summary>
        /// Unique id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Center position
        /// </summary>
        public Vec2 Position { get; set; }

        /// <summary>
        /// Obstacle's radius
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Whether units can see through this obstacle, or it blocks the view
        /// </summary>
        public bool CanSeeThrough { get; set; }

        /// <summary>
        /// Whether projectiles can go through this obstacle
        /// </summary>
        public bool CanShootThrough { get; set; }

        public Obstacle(int id, Vec2 position, double radius, bool canSeeThrough, bool canShootThrough)
        {
            Id = id;
            Position = position;
            Radius = radius;
            CanSeeThrough = canSeeThrough;
            CanShootThrough = canShootThrough;
        }

        /// <summary> Read Obstacle from reader </summary>
        public static Obstacle ReadFrom(BinaryReader reader)
        {
            var result = new Obstacle();
            result.Id = reader.ReadInt32();
            result.Position = Vec2.ReadFrom(reader);
            result.Radius = reader.ReadDouble();
            result.CanSeeThrough = reader.ReadBoolean();
            result.CanShootThrough = reader.ReadBoolean();
            return result;
        }

        /// <summary> Write Obstacle to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            Position.WriteTo(writer);
            writer.Write(Radius);
            writer.Write(CanSeeThrough);
            writer.Write(CanShootThrough);
        }

        /// <summary> Get string representation of Obstacle </summary>
        public override string ToString()
        {
            string stringResult = "Obstacle { ";
            stringResult += "Id: ";
            stringResult += Id.ToString();
            stringResult += ", ";
            stringResult += "Position: ";
            stringResult += Position.ToString();
            stringResult += ", ";
            stringResult += "Radius: ";
            stringResult += Radius.ToString();
            stringResult += ", ";
            stringResult += "CanSeeThrough: ";
            stringResult += CanSeeThrough.ToString();
            stringResult += ", ";
            stringResult += "CanShootThrough: ";
            stringResult += CanShootThrough.ToString();
            stringResult += " }";
            return stringResult;
        }
    }
}