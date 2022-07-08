#region Usings

using System;
using System.IO;

#endregion

namespace AiCup22.Debugging
{
    /// <summary>
    /// Debug commands that can be sent while debugging with the app
    /// </summary>
    public abstract class DebugCommand
    {
        /// <summary> Write DebugCommand to writer </summary>
        public abstract void WriteTo(BinaryWriter writer);

        /// <summary> Read DebugCommand from reader </summary>
        public static DebugCommand ReadFrom(BinaryReader reader)
        {
            switch (reader.ReadInt32())
            {
                case Add.TAG:
                    return Add.ReadFrom(reader);
                case Clear.TAG:
                    return Clear.ReadFrom(reader);
                case SetAutoFlush.TAG:
                    return SetAutoFlush.ReadFrom(reader);
                case Flush.TAG:
                    return Flush.ReadFrom(reader);
                default:
                    throw new Exception("Unexpected tag value");
            }
        }

        /// <summary>
        /// Add debug data to current tick
        /// </summary>
        public class Add : DebugCommand
        {
            public const int TAG = 0;

            /// <summary>
            /// Data to add
            /// </summary>
            public DebugData DebugData { get; set; }

            public Add()
            {
            }

            public Add(DebugData debugData)
            {
                DebugData = debugData;
            }

            /// <summary> Read Add from reader </summary>
            public static new Add ReadFrom(BinaryReader reader)
            {
                var result = new Add();
                result.DebugData = DebugData.ReadFrom(reader);
                return result;
            }

            /// <summary> Write Add to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                DebugData.WriteTo(writer);
            }

            /// <summary> Get string representation of Add </summary>
            public override string ToString()
            {
                string stringResult = "Add { ";
                stringResult += "DebugData: ";
                stringResult += DebugData.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Clear current tick's debug data
        /// </summary>
        public class Clear : DebugCommand
        {
            public const int TAG = 1;


            /// <summary> Read Clear from reader </summary>
            public static new Clear ReadFrom(BinaryReader reader)
            {
                var result = new Clear();
                return result;
            }

            /// <summary> Write Clear to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }

            /// <summary> Get string representation of Clear </summary>
            public override string ToString()
            {
                string stringResult = "Clear { ";
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Enable/disable auto performing of commands
        /// </summary>
        public class SetAutoFlush : DebugCommand
        {
            public const int TAG = 2;

            /// <summary>
            /// Enable/disable autoflush
            /// </summary>
            public bool Enable { get; set; }

            public SetAutoFlush()
            {
            }

            public SetAutoFlush(bool enable)
            {
                Enable = enable;
            }

            /// <summary> Read SetAutoFlush from reader </summary>
            public static new SetAutoFlush ReadFrom(BinaryReader reader)
            {
                var result = new SetAutoFlush();
                result.Enable = reader.ReadBoolean();
                return result;
            }

            /// <summary> Write SetAutoFlush to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                writer.Write(Enable);
            }

            /// <summary> Get string representation of SetAutoFlush </summary>
            public override string ToString()
            {
                string stringResult = "SetAutoFlush { ";
                stringResult += "Enable: ";
                stringResult += Enable.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Perform all previously sent commands
        /// </summary>
        public class Flush : DebugCommand
        {
            public const int TAG = 3;


            /// <summary> Read Flush from reader </summary>
            public static new Flush ReadFrom(BinaryReader reader)
            {
                var result = new Flush();
                return result;
            }

            /// <summary> Write Flush to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }

            /// <summary> Get string representation of Flush </summary>
            public override string ToString()
            {
                string stringResult = "Flush { ";
                stringResult += " }";
                return stringResult;
            }
        }
    }
}