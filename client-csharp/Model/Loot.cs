#region Usings

using System.IO;

#endregion

namespace AiCup22.Model
{
    /// <summary>
    /// Loot lying on the ground
    /// </summary>
    public struct Loot
    {
        /// <summary>
        /// Unique id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        public Vec2 Position { get; set; }

        /// <summary>
        /// Item
        /// </summary>
        public Item Item { get; set; }

        public Loot(int id, Vec2 position, Item item)
        {
            Id = id;
            Position = position;
            Item = item;
        }

        /// <summary> Read Loot from reader </summary>
        public static Loot ReadFrom(BinaryReader reader)
        {
            var result = new Loot();
            result.Id = reader.ReadInt32();
            result.Position = Vec2.ReadFrom(reader);
            result.Item = Item.ReadFrom(reader);
            return result;
        }

        /// <summary> Write Loot to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
            Position.WriteTo(writer);
            Item.WriteTo(writer);
        }

        /// <summary> Get string representation of Loot </summary>
        public override string ToString()
        {
            string stringResult = "Loot { ";
            stringResult += "Id: ";
            stringResult += Id.ToString();
            stringResult += ", ";
            stringResult += "Position: ";
            stringResult += Position.ToString();
            stringResult += ", ";
            stringResult += "Item: ";
            stringResult += Item.ToString();
            stringResult += " }";
            return stringResult;
        }
    }
}