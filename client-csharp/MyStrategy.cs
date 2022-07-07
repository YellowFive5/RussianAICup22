#region Usings

using System.Collections.Generic;
using AiCup22.Model;

#endregion

namespace AiCup22
{
    public class MyStrategy
    {
        public MyStrategy(Constants constants)
        {
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            return new Order(new Dictionary<int, UnitOrder>());
        }

        public void DebugUpdate(DebugInterface debugInterface)
        {
        }

        public void Finish()
        {
        }
    }
}