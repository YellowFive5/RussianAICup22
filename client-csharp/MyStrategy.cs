#region Usings

using System.Collections.Generic;
using AiCup22.Model;

#endregion

namespace AiCup22
{
    public class MyStrategy
    {
        private World World { get; }

        public MyStrategy(Constants constants)
        {
            World = new World(constants);
        }

        public Order GetOrder(Game game, DebugInterface debugInterface)
        {
            World.Scan(game);

            ActionOrder.Aim actionAim;

            Vec2 targetVelocity = new Vec2();
            Vec2 targetDirection = new Vec2();

            actionAim = new ActionOrder.Aim(true); // стрелять
            if (actionAim == null)
            {
                actionAim = new ActionOrder.Aim(false);
            }


            targetVelocity.X = 10; // бежать будете просто в право и немного вверх 
            targetVelocity.Y = 10;

            UnitOrder UnitOrder1 = new UnitOrder(targetVelocity, targetDirection, actionAim); // создаете контейнер с командами кроме стрельбы может быть любое действие откройте в среде объект, выберите и создайте
            Dictionary<int, UnitOrder> MyCommand = new Dictionary<int, UnitOrder>(); // создание словаря для отдачи
            MyCommand.Add(World.Me.Id, UnitOrder1); // добавляем в словарь нашего колдуна и его команды, в этом примере он не в себе
            return new Order(MyCommand); // возвращаете свой словарь
            // надеюсь кому поможет
        }

        public void DebugUpdate(DebugInterface debugInterface)
        {
        }

        public void Finish()
        {
        }
    }
}