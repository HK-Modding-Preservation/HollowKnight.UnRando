using MenuChanger.Attributes;
using System.Collections.Generic;

namespace UnRando.Rando;

public enum ProgressionSetting
{
    Random,
    Early,
    Average,
    Late,
    VeryLate,
}

internal enum RandoProgressionType
{
    Dash,
    Claw,
    CDash,
    Wings,
}

public class RandomizationSettings
{
    public bool Enabled = false;

    public ProgressionSetting DashProgression = ProgressionSetting.Random;
    public ProgressionSetting ClawProgression = ProgressionSetting.Random;
    public ProgressionSetting CDashProgression = ProgressionSetting.Random;
    public ProgressionSetting WingsProgression = ProgressionSetting.Random;

    internal Dictionary<RandoProgressionType, ProgressionSetting> ProgressionDict()
    {
        return new()
        {
            { RandoProgressionType.Dash, DashProgression },
            { RandoProgressionType.Claw, ClawProgression },
            { RandoProgressionType.CDash, CDashProgression },
            { RandoProgressionType.Wings, WingsProgression },
        };
    }
}
