#region Usings

using System.IO;

#endregion

namespace AiCup22.Model
{
    /// <summary>
    /// Sound heard by one of your units
    /// </summary>
    public struct Sound
    {
        /// <summary>
        /// Sound type index (starting with 0)
        /// </summary>
        public int TypeIndex { get; set; }

        /// <summary>
        /// Id of unit that heard this sound
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// Position where sound was heard (different from sound source position)
        /// </summary>
        public Vec2 Position { get; set; }

        public Sound(int typeIndex, int unitId, Vec2 position)
        {
            TypeIndex = typeIndex;
            UnitId = unitId;
            Position = position;
        }

        /// <summary> Read Sound from reader </summary>
        public static Sound ReadFrom(BinaryReader reader)
        {
            var result = new Sound();
            result.TypeIndex = reader.ReadInt32();
            result.UnitId = reader.ReadInt32();
            result.Position = Vec2.ReadFrom(reader);
            return result;
        }

        /// <summary> Write Sound to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(TypeIndex);
            writer.Write(UnitId);
            Position.WriteTo(writer);
        }

        /// <summary> Get string representation of Sound </summary>
        public override string ToString()
        {
            string stringResult = "Sound { ";
            stringResult += "TypeIndex: ";
            stringResult += TypeIndex.ToString();
            stringResult += ", ";
            stringResult += "UnitId: ";
            stringResult += UnitId.ToString();
            stringResult += ", ";
            stringResult += "Position: ";
            stringResult += Position.ToString();
            stringResult += " }";
            return stringResult;
        }
    }
}