using ItemChanger;
using RandomizerCore;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using System.Collections.Generic;
using System.Linq;

namespace UnRando.Rando;

internal record ProgressionSpread
{
    public readonly float min;
    public readonly float max;

    public readonly float splitMin;
    public readonly float splitMax;

    public ProgressionSpread(float min, float max)
    {
        this.min = min;
        this.max = max;
        splitMin = min;
        splitMax = max;
    }

    public ProgressionSpread(float min, float max, float splitMin, float splitMax)
    {
        this.min = min;
        this.max = max;
        this.splitMin = splitMin;
        this.splitMax = splitMax;
    }
}

internal record ProgressionData
{
    public readonly HashSet<string> itemNames;
    public readonly HashSet<string> splitItemNames;
    public readonly ProgressionSpread early;
    public readonly ProgressionSpread average;
    public readonly ProgressionSpread late;
    public readonly ProgressionSpread veryLate;

    public ProgressionData(HashSet<string> itemNames, HashSet<string> splitItemNames, ProgressionSpread early, ProgressionSpread average, ProgressionSpread late, ProgressionSpread veryLate)
    {
        this.itemNames = new(itemNames);
        this.splitItemNames = new(splitItemNames);
        this.early = early;
        this.average = average;
        this.late = late;
        this.veryLate = veryLate;
    }

    internal void Apply(ProgressionSetting setting, RequestBuilder rb, ItemGroupBuilder igb, int totalLocs, System.Random r)
    {
        ProgressionSpread? spread = setting switch { ProgressionSetting.Random => null, ProgressionSetting.Early => early, ProgressionSetting.Average => average, ProgressionSetting.Late => late, ProgressionSetting.VeryLate => veryLate, _ => null };
        if (spread == null) return;

        var (min, max) = (splitItemNames.Any(n => igb.Items.GetCount(n) > 0)) ? (spread.splitMin, spread.splitMax) : (spread.min, spread.max);

        foreach (var item in itemNames) MaybePlace(rb, igb, item, min, max, totalLocs, r);
        foreach (var item in splitItemNames) MaybePlace(rb, igb, item, min, max, totalLocs, r);
    }

    private void MaybePlace(RequestBuilder rb, ItemGroupBuilder igb, string item, float min, float max, int totalLocs, System.Random r)
    {
        int count = igb.Items.GetCount(item);
        if (count == 0) return;

        igb.Items.RemoveAll(item);
        for (int i = 0; i < count; i++)
        {
            int pos = (int)((min + r.NextDouble() * (max - min)) * totalLocs);
            if (pos <= 0) pos = 1;
            if (pos > totalLocs) pos = totalLocs;

            UnRandoLocation loc = new(pos);
            rb.AddToPreplaced(item, loc.name);
        }
    }
}

internal record ShopData
{
    public readonly int lowCost;
    public readonly int highCost;
    public readonly int itemCount;

    public ShopData(int itemCount, (int, int) costs)
    {
        this.itemCount = itemCount;
        (lowCost, highCost) = costs;
    }
}

internal class RequestModifier
{
    internal static void Setup() => RequestBuilder.OnUpdate.Subscribe(1000f, ApplyUnRando);

    private static Dictionary<RandoProgressionType, ProgressionData> PROGRESSION_DATA = new()
    {
        [RandoProgressionType.Dash] = new(["Mothwing_Cloak"], ["Left_Mothwing_Cloak", "Right_Mothwing_Cloak"], new(0, 0.15f, 0, 0.35f), new(0.1f, 0.2f, 0.1f, 0.3f), new(0.35f, 0.5f, 0.3f, 0.55f), new(0.65f, 0.85f, 0.6f, 0.9f)),
        [RandoProgressionType.Claw] = new(["Mantis_Claw"], ["Left_Mantis_Claw", "Right_Mantis_Claw"], new(0.05f, 0.2f, 0.05f, 0.4f), new(0.15f, 0.3f, 0.1f, 0.5f), new(0.5f, 0.7f, 0.4f, 0.75f), new(0.65f, 0.75f, 0.6f, 0.85f)),
        [RandoProgressionType.CDash] = new(["Crystal_Heart"], ["Left_Crystal_Heart", "Right_Crystal_Heart"], new(0.05f, 0.2f, 0.05f, 0.4f), new(0.3f, 0.45f, 0.2f, 0.55f), new(0.5f, 0.65f, 0.4f, 0.75f), new(0.65f, 0.85f, 0.5f, 0.9f)),
        [RandoProgressionType.Wings] = new(["Monarch_Wings"], [], new(0.1f, 0.2f), new(0.35f, 0.5f), new(0.65f, 0.8f), new(0.8f, 0.9f)),
    };

    private static Dictionary<string, ShopData> SHOP_DATA = new()
    {
        ["Iselda"] = new(1, (100, 200)),
        ["Leg_Eater"] = new(2, (200, 300)),
        ["Salubra"] = new(2, (100, 300)),
        ["Salubra_(Requires_Charms)"] = new(2, (250, 450)),
        ["Sly"] = new(2, (150, 350)),
        ["Sly_(Key)"] = new(2, (350, 600)),
    };

    private static CostDef[]? GetVanillaCosts(string loc, System.Random r)
    {
        if (SHOP_DATA.TryGetValue(loc, out var data)) return [new CostDef("GEO", r.Next(data.lowCost, data.highCost + 1))];
        return [];
    }

    private static void ApplyUnRando(RequestBuilder rb)
    {
        if (!RandoInterop.IsEnabled) return;

        List<int> totalHolder = [0];
        List<Shuffler<int>> shufflerHolder = [];
        System.Random r = new(rb.gs.Seed + 9187);

        foreach (var igb in rb.EnumerateItemGroups())
        {
            igb.LocationPadder = (factory, count) =>
            {
                if (shufflerHolder.Count == 0)
                {
                    Shuffler<int> newShuffler = new();
                    for (int i = 0; i < totalHolder[0]; i++) newShuffler.Add(i);
                    shufflerHolder.Add(newShuffler);
                }
                var shuffler = shufflerHolder[0];

                List<IRandoLocation> locs = [];
                for (int i = 0; i < count; i++)
                {
                    int slot = shuffler.Take(r) + 1;
                    UnRandoLocation loc = new(slot);
                    locs.Add(factory.MakeLocation(loc.name));
                }
                return locs;
            };

            int subTotal = 0;
            List<string> toRemove = [];
            foreach (var loc in igb.Locations.EnumerateDistinct())
            {
                if (loc == "Start") continue;

                var count = igb.Locations.GetCount(loc);
                if (SHOP_DATA.TryGetValue(loc, out var data)) count = System.Math.Min(count, data.itemCount);

                subTotal += count;
                for (int i = 0; i < count; i++) rb.AddToPreplaced(new VanillaDef(nameof(UnRandoCheck), loc, GetVanillaCosts(loc, r)));
                toRemove.Add(loc);
            }
            toRemove.ForEach(igb.Locations.RemoveAll);

            for (int i = 0; i < subTotal; i++)
            {
                UnRandoLocation loc = new(totalHolder[0] + i + 1);
                Finder.UndefineCustomLocation(loc.name);
                Finder.DefineCustomLocation(loc);

                igb.Locations.Add(loc.name);
                rb.EditLocationRequest(loc.name, loc.GetInfo);
            }
            totalHolder[0] += subTotal;
        }

        foreach (var igb in rb.EnumerateItemGroups())
        {
            foreach (var entry in UnRando.GS.RandoSettings.ProgressionDict()) PROGRESSION_DATA[entry.Key].Apply(entry.Value, rb, igb, totalHolder[0], r);
        }
    }
}
