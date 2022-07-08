#region Usings

using System.Collections.Generic;
using System.IO;

#endregion

namespace AiCup22.Model
{
    /// <summary>
    /// Player's (team's) orders
    /// </summary>
    public struct Order
    {
        /// <summary>
        /// Orders for each of your units
        /// </summary>
        public IDictionary<int, UnitOrder> UnitOrders { get; set; }

        public Order(IDictionary<int, UnitOrder> unitOrders)
        {
            UnitOrders = unitOrders;
        }

        /// <summary> Read Order from reader </summary>
        public static Order ReadFrom(BinaryReader reader)
        {
            var result = new Order();
            int unitOrdersSize = reader.ReadInt32();
            result.UnitOrders = new Dictionary<int, UnitOrder>(unitOrdersSize);
            for (int unitOrdersIndex = 0; unitOrdersIndex < unitOrdersSize; unitOrdersIndex++)
            {
                int unitOrdersKey;
                UnitOrder unitOrdersValue;
                unitOrdersKey = reader.ReadInt32();
                unitOrdersValue = UnitOrder.ReadFrom(reader);
                result.UnitOrders.Add(unitOrdersKey, unitOrdersValue);
            }

            return result;
        }

        /// <summary> Write Order to writer </summary>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(UnitOrders.Count);
            foreach (var unitOrdersEntry in UnitOrders)
            {
                var unitOrdersKey = unitOrdersEntry.Key;
                var unitOrdersValue = unitOrdersEntry.Value;
                writer.Write(unitOrdersKey);
                unitOrdersValue.WriteTo(writer);
            }
        }

        /// <summary> Get string representation of Order </summary>
        public override string ToString()
        {
            string stringResult = "Order { ";
            stringResult += "UnitOrders: ";
            stringResult += "{ ";
            int unitOrdersIndex = 0;
            foreach (var unitOrdersEntry in UnitOrders)
            {
                if (unitOrdersIndex != 0)
                {
                    stringResult += ", ";
                }

                var unitOrdersKey = unitOrdersEntry.Key;
                stringResult += unitOrdersKey.ToString();
                stringResult += ": ";
                var unitOrdersValue = unitOrdersEntry.Value;
                stringResult += unitOrdersValue.ToString();
                unitOrdersIndex++;
            }

            stringResult += " }";
            stringResult += " }";
            return stringResult;
        }
    }
}