#region Usings

using System.IO;
using System.Text;
using AiCup22.Model;

#endregion

namespace AiCup22.Debugging
{
    /// <summary>
    /// Renderer's state
    /// </summary>
    public struct DebugState
    {
        /// <summary>
        /// Pressed keys
        /// </summary>
        public string[] PressedKeys { get; set; }

        /// <summary>
        /// Cursor's position in game coordinates
        /// </summary>
        public Vec2 CursorWorldPosition { get; set; }

        /// <summary>
        /// Id of unit which is followed by the camera, or None
        /// </summary>
        public int? LockedUnit { get; set; }

        /// <summary>
        /// Current camera state
        /// </summary>
        public Camera Camera { get; set; }

        public DebugState(string[] pressedKeys, Vec2 cursorWorldPosition, int? lockedUnit, Camera camera)
        {
            PressedKeys = pressedKeys;
            CursorWorldPosition = cursorWorldPosition;
            LockedUnit = lockedUnit;
            Camera = camera;
        }

        /// <summary> Read DebugState from reader </summary>
        public static DebugState ReadFrom(BinaryReader reader)
        {
            var result = new DebugState();
            result.PressedKeys = new string[reader.ReadInt32()];
            for (int pressedKeysIndex = 0; pressedKeysIndex < result.PressedKeys.Length; pressedKeysIndex++)
            {
                result.PressedKeys[pressedKeysIndex] = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
            }

            result.CursorWorldPosition = Vec2.ReadFrom(reader);
            if (reader.ReadBoolean())
            {
                result.LockedUnit = reader.ReadInt32();
            }
            else
            {
                result.LockedUnit = null;
            }

            result.Camera = Camera.ReadFrom(reader);
            return result;
        }

        /// <summary> Write DebugState to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(PressedKeys.Length);
            foreach (var pressedKeysElement in PressedKeys)
            {
                var pressedKeysElementData = Encoding.UTF8.GetBytes(pressedKeysElement);
                writer.Write(pressedKeysElementData.Length);
                writer.Write(pressedKeysElementData);
            }

            CursorWorldPosition.WriteTo(writer);
            if (!LockedUnit.HasValue)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(LockedUnit.Value);
            }

            Camera.WriteTo(writer);
        }

        /// <summary> Get string representation of DebugState </summary>
        public override string ToString()
        {
            string stringResult = "DebugState { ";
            stringResult += "PressedKeys: ";
            stringResult += "[ ";
            int pressedKeysIndex = 0;
            foreach (var pressedKeysElement in PressedKeys)
            {
                if (pressedKeysIndex != 0)
                {
                    stringResult += ", ";
                }

                stringResult += "\"" + pressedKeysElement + "\"";
                pressedKeysIndex++;
            }

            stringResult += " ]";
            stringResult += ", ";
            stringResult += "CursorWorldPosition: ";
            stringResult += CursorWorldPosition.ToString();
            stringResult += ", ";
            stringResult += "LockedUnit: ";
            if (!LockedUnit.HasValue)
            {
                stringResult += "null";
            }
            else
            {
                stringResult += LockedUnit.Value.ToString();
            }

            stringResult += ", ";
            stringResult += "Camera: ";
            stringResult += Camera.ToString();
            stringResult += " }";
            return stringResult;
        }
    }
}