#region Usings

using System.IO;

#endregion

namespace AiCup22.Model
{
    /// <summary>
    /// Order for specific unit
    /// </summary>
    public struct UnitOrder
    {
        /// <summary>
        /// Target moving velocity
        /// </summary>
        public Vec2 TargetVelocity { get; set; }

        /// <summary>
        /// Target view direction (vector length doesn't matter)
        /// </summary>
        public Vec2 TargetDirection { get; set; }

        /// <summary>
        /// Order to perform an action, or None
        /// </summary>
        public ActionOrder Action { get; set; }

        public UnitOrder(Vec2 targetVelocity, Vec2 targetDirection, ActionOrder action)
        {
            TargetVelocity = targetVelocity;
            TargetDirection = targetDirection;
            Action = action;
        }

        /// <summary> Read UnitOrder from reader </summary>
        public static UnitOrder ReadFrom(BinaryReader reader)
        {
            var result = new UnitOrder();
            result.TargetVelocity = Vec2.ReadFrom(reader);
            result.TargetDirection = Vec2.ReadFrom(reader);
            if (reader.ReadBoolean())
            {
                result.Action = ActionOrder.ReadFrom(reader);
            }
            else
            {
                result.Action = null;
            }

            return result;
        }

        /// <summary> Write UnitOrder to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            TargetVelocity.WriteTo(writer);
            TargetDirection.WriteTo(writer);
            if (Action == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                Action.WriteTo(writer);
            }
        }

        /// <summary> Get string representation of UnitOrder </summary>
        public override string ToString()
        {
            string stringResult = "UnitOrder { ";
            stringResult += "TargetVelocity: ";
            stringResult += TargetVelocity.ToString();
            stringResult += ", ";
            stringResult += "TargetDirection: ";
            stringResult += TargetDirection.ToString();
            stringResult += ", ";
            stringResult += "Action: ";
            if (Action == null)
            {
                stringResult += "null";
            }
            else
            {
                stringResult += Action.ToString();
            }

            stringResult += " }";
            return stringResult;
        }
    }
}