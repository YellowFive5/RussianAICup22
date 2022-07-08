#region Usings

using System;
using System.IO;
using AiCup22.Debugging;
using AiCup22.Model;

#endregion

namespace AiCup22.Codegame
{
    /// <summary>
    /// Message sent from client
    /// </summary>
    public abstract class ClientMessage
    {
        /// <summary> Write ClientMessage to writer </summary>
        public abstract void WriteTo(BinaryWriter writer);

        /// <summary> Read ClientMessage from reader </summary>
        public static ClientMessage ReadFrom(BinaryReader reader)
        {
            switch (reader.ReadInt32())
            {
                case DebugMessage.TAG:
                    return DebugMessage.ReadFrom(reader);
                case OrderMessage.TAG:
                    return OrderMessage.ReadFrom(reader);
                case DebugUpdateDone.TAG:
                    return DebugUpdateDone.ReadFrom(reader);
                case RequestDebugState.TAG:
                    return RequestDebugState.ReadFrom(reader);
                default:
                    throw new Exception("Unexpected tag value");
            }
        }

        /// <summary>
        /// Ask app to perform new debug command
        /// </summary>
        public class DebugMessage : ClientMessage
        {
            public const int TAG = 0;

            /// <summary>
            /// Command to perform
            /// </summary>
            public DebugCommand Command { get; set; }

            public DebugMessage()
            {
            }

            public DebugMessage(DebugCommand command)
            {
                Command = command;
            }

            /// <summary> Read DebugMessage from reader </summary>
            public static new DebugMessage ReadFrom(BinaryReader reader)
            {
                var result = new DebugMessage();
                result.Command = DebugCommand.ReadFrom(reader);
                return result;
            }

            /// <summary> Write DebugMessage to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Command.WriteTo(writer);
            }

            /// <summary> Get string representation of DebugMessage </summary>
            public override string ToString()
            {
                string stringResult = "DebugMessage { ";
                stringResult += "Command: ";
                stringResult += Command.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Reply for ServerMessage::GetOrder
        /// </summary>
        public class OrderMessage : ClientMessage
        {
            public const int TAG = 1;

            /// <summary>
            /// Player's order
            /// </summary>
            public Order Order { get; set; }

            public OrderMessage()
            {
            }

            public OrderMessage(Order order)
            {
                Order = order;
            }

            /// <summary> Read OrderMessage from reader </summary>
            public static new OrderMessage ReadFrom(BinaryReader reader)
            {
                var result = new OrderMessage();
                result.Order = Order.ReadFrom(reader);
                return result;
            }

            /// <summary> Write OrderMessage to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
                Order.WriteTo(writer);
            }

            /// <summary> Get string representation of OrderMessage </summary>
            public override string ToString()
            {
                string stringResult = "OrderMessage { ";
                stringResult += "Order: ";
                stringResult += Order.ToString();
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Signifies finish of the debug update
        /// </summary>
        public class DebugUpdateDone : ClientMessage
        {
            public const int TAG = 2;


            /// <summary> Read DebugUpdateDone from reader </summary>
            public static new DebugUpdateDone ReadFrom(BinaryReader reader)
            {
                var result = new DebugUpdateDone();
                return result;
            }

            /// <summary> Write DebugUpdateDone to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }

            /// <summary> Get string representation of DebugUpdateDone </summary>
            public override string ToString()
            {
                string stringResult = "DebugUpdateDone { ";
                stringResult += " }";
                return stringResult;
            }
        }

        /// <summary>
        /// Request debug state from the app
        /// </summary>
        public class RequestDebugState : ClientMessage
        {
            public const int TAG = 3;


            /// <summary> Read RequestDebugState from reader </summary>
            public static new RequestDebugState ReadFrom(BinaryReader reader)
            {
                var result = new RequestDebugState();
                return result;
            }

            /// <summary> Write RequestDebugState to writer </summary>
            public override void WriteTo(BinaryWriter writer)
            {
                writer.Write(TAG);
            }

            /// <summary> Get string representation of RequestDebugState </summary>
            public override string ToString()
            {
                string stringResult = "RequestDebugState { ";
                stringResult += " }";
                return stringResult;
            }
        }
    }
}