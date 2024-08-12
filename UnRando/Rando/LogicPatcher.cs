using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace UnRando.Rando;

internal class LogicPatcher
{
    private const string UNRANDO_CHECK_COUNT = "UNRANDO_CHECK_COUNT";

    internal static void Setup() => RCData.RuntimeLogicOverride.Subscribe(1000f, PatchLogic);

    private static void CheckSplitGroupSettings(SplitGroupSettings settings)
    {
        if (settings.RandomizeOnStart) throw new System.ArgumentException("UnRando does not support SplitGroup.RandomizeOnStart");

        foreach (var field in SplitGroupSettings.IntFields.Values)
        {
            int value = (int)field.GetValue(settings);
            if (value != -1) throw new System.ArgumentException($"UnRando does not support Split Groups ({field.Name} ({value}) != -1)");
        }
    }

    private static void PatchLogic(GenerationSettings gs, LogicManagerBuilder lmb)
    {
        if (!RandoInterop.IsEnabled) return;

        CheckSplitGroupSettings(gs.SplitGroupSettings);

        var checkCount = lmb.GetOrAddTerm(UNRANDO_CHECK_COUNT, TermType.Int);
        lmb.AddItem(new CappedItem(nameof(UnRandoCheck), [new(checkCount, 1)], new(checkCount, int.MaxValue)));

        int locs = 0;
        foreach (var key in lmb.LogicLookup.Keys)
        {
            if (key.Contains("[") || lmb.Waypoints.Contains(key)) continue;
            ++locs;
        }

        // We have to guess the location count, since logic must be finalized before placements.
        int locsNeeded = (int)(locs * 1.05f) + 100;
        for (int i = 0; i < locsNeeded; i++)
        {
            UnRandoLocation loc = new(i + 1);
            lmb.AddLogicDef(new(loc.name, $"{UNRANDO_CHECK_COUNT} > {i}"));
        }
    }
}
