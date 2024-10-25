using MenuChanger.Attributes;

namespace UnRando.Rando;

public class RandomizationSettings
{
    public bool Enabled = false;

    [MenuRange(-1, 99)]
    public int SplitGroup = -1;
}
