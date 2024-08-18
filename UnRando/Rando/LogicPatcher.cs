using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System.Linq;

namespace UnRando.Rando;

internal class LogicPatcher
{
    private const string UNRANDO_CHECK_COUNT = "UNRANDO_CHECK_COUNT";

    internal static void Setup() => RCData.RuntimeLogicOverride.Subscribe(1000f, PatchLogic);

    private static void PatchLogic(GenerationSettings gs, LogicManagerBuilder lmb)
    {
        if (!RandoInterop.IsEnabled) return;

        if (gs.SplitGroupSettings.RandomizeOnStart)
            throw new System.ArgumentException("UnRando does not support SplitGroup.RandomizeOnStart");

        var checkCount = lmb.GetOrAddTerm(UNRANDO_CHECK_COUNT, TermType.Int);
        lmb.AddItem(new CappedItem(nameof(UnRandoCheck), [new(checkCount, 1)], new(checkCount, int.MaxValue)));

        // We have to guess the location count, since logic must be finalized before placements.
        int locsEstimate = (int)(lmb.LogicLookup.Keys.Where(k => !k.Contains("[") && !lmb.Waypoints.Contains(k)).Count() * 1.05f) + 100;
        for (int i = 0; i < locsEstimate; i++)
        {
            UnRandoLocation loc = new(i + 1);
            lmb.AddLogicDef(new(loc.name, $"{UNRANDO_CHECK_COUNT} > {i - 1}"));
        }
    }
}
