#region Usings

using System.Collections.Generic;
using System.Linq;
using AiCup22.Model;

#endregion

namespace AiCup22;

public class World
{
    public Constants Constants { get; }

    public MyUnit Me => MyUnits.First(); // todo Round 1 only
    public List<MyUnit> MyUnits { get; } = new();
    public List<EnemyUnit> EnemyUnits { get; } = new();

    public List<WeaponItem> WeaponItems { get; } = new();
    public List<AmmoItem> AmmoItems { get; } = new();
    public List<ShieldItem> ShieldItems { get; } = new();


    public World(Constants constants)
    {
        Constants = constants;
    }

    public void Scan(Game game)
    {
        foreach (var unit in game.Units)
        {
            if (unit.PlayerId == game.MyId)
            {
                MyUnits.Add(new MyUnit(unit));
            }
            else
            {
                EnemyUnits.Add(new EnemyUnit(unit));
            }
        }

        foreach (var loot in game.Loot)
        {
            switch (loot.Item)
            {
                case Item.Weapon:
                    WeaponItems.Add(new WeaponItem(loot));
                    break;
                case Item.Ammo:
                    AmmoItems.Add(new AmmoItem(loot));
                    break;
                case Item.ShieldPotions:
                    ShieldItems.Add(new ShieldItem(loot));
                    break;
            }
        }
    }
}