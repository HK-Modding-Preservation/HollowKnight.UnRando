using ItemChanger;

namespace UnRando.Rando;

internal class UnRandoModule : ItemChanger.Modules.Module
{
    public int ChecksObtained = 0;

    public static UnRandoModule Get() => ItemChangerMod.Modules.Get<UnRandoModule>();

    public override void Initialize() { }

    public override void Unload() { }
}
