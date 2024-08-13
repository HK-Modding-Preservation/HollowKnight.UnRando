using Modding;
using PurenailCore.ModUtil;

namespace UnRando;

public class UnRando : Mod, IGlobalSettings<GlobalSettings>
{
    public static UnRando? Instance { get; private set; }

    public static GlobalSettings GS { get; private set; } = new();

    public UnRando() : base("UnRando")
    {
        Instance = this;
    }

    public override string GetVersion() => VersionUtil.ComputeVersion<UnRando>();

    public override void Initialize() => Rando.RandoInterop.Setup();

    public void OnLoadGlobal(GlobalSettings s) => GS = s ?? new();

    public GlobalSettings OnSaveGlobal() => GS ?? new();

    public static new void Log(string msg) => ((ILogger)Instance!).Log(msg);
}
