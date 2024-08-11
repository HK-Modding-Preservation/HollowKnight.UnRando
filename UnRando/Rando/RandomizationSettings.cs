namespace UnRando.Rando;

public enum ProgressSetting
{
    Early,
    Random,
    Late,
    VeryLate,
}

public class RandomizationSettings
{
    public bool Enabled = false;
    public ProgressSetting DashProgression = ProgressSetting.Random;
    public ProgressSetting ClawProgression = ProgressSetting.Random;
    public ProgressSetting CDashProgression = ProgressSetting.Random;
    public ProgressSetting WingsProgression = ProgressSetting.Random;
    public ProgressSetting SpellProgression = ProgressSetting.Random;
}
