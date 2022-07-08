#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22;

public abstract class CustomUnit
{
    public Unit Unit { get; }
    public int Id { get; }

    protected CustomUnit(Unit unit)
    {
        Unit = unit;
        Id = unit.Id;
    }
}