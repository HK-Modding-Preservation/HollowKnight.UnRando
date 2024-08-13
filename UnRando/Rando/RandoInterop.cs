using ItemChanger;
using PurenailCore.SystemUtil;
using RandomizerCore;
using RandomizerMod.RC;

namespace UnRando.Rando;

internal class RandoInterop
{
    internal static void Setup()
    {
        Finder.DefineCustomItem(new UnRandoCheck());

        ConnectionMenu.Setup();
        LogicPatcher.Setup();
        RequestModifier.Setup();
        RandoController.OnExportCompleted += OnExportCompleted;
    }

    internal static bool IsEnabled => UnRando.GS.RandoSettings.Enabled;

    private static void OnExportCompleted(RandoController rc)
    {
        if (!IsEnabled) return;

        ItemChangerMod.Modules.GetOrAdd<UnRandoModule>();

        // Add placement tags to every unrando check.
        foreach (var entry in ItemChanger.Internal.Ref.Settings.Placements)
            foreach (var item in entry.Value.Items)
                if (item is UnRandoCheck check)
                    check.AddTag<UnRandoPlacementTag>().PlacementName = entry.Key;
    }
}
