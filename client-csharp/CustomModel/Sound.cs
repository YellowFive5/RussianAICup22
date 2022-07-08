namespace AiCup22.CustomModel;

public class Sound : CustomItem
{
    public Model.Sound _Sound { get; }

    public Sound(Model.Sound sound) : base(0, sound.Position)
    {
        _Sound = sound;
    }
}