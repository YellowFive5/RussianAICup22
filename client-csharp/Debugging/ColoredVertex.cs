#region Usings

using System.IO;
using AiCup22.Model;

#endregion

namespace AiCup22.Debugging
{
    /// <summary>
    /// Point + color
    /// </summary>
    public struct ColoredVertex
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vec2 Position { get; set; }

        /// <summary>
        /// Color
        /// </summary>
        public Color Color { get; set; }

        public ColoredVertex(Vec2 position, Color color)
        {
            Position = position;
            Color = color;
        }

        /// <summary> Read ColoredVertex from reader </summary>
        public static ColoredVertex ReadFrom(BinaryReader reader)
        {
            var result = new ColoredVertex();
            result.Position = Vec2.ReadFrom(reader);
            result.Color = Color.ReadFrom(reader);
            return result;
        }

        /// <summary> Write ColoredVertex to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            Position.WriteTo(writer);
            Color.WriteTo(writer);
        }

        /// <summary> Get string representation of ColoredVertex </summary>
        public override string ToString()
        {
            string stringResult = "ColoredVertex { ";
            stringResult += "Position: ";
            stringResult += Position.ToString();
            stringResult += ", ";
            stringResult += "Color: ";
            stringResult += Color.ToString();
            stringResult += " }";
            return stringResult;
        }
    }
}