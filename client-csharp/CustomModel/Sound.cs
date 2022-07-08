#region Usings

using AiCup22.Model;

#endregion

namespace AiCup22.CustomModel;

public class Sound : CustomItem
{
    public Model.Sound _Sound { get; }

    public Sound(Model.Sound sound, Constants constants) : base(0, sound.Position, constants)
    {
        _Sound = sound;
    }
}