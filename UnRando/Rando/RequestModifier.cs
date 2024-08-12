using ItemChanger;
using RandomizerCore;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using System.Collections.Generic;

namespace UnRando.Rando;

internal class RequestModifier
{
    internal static void Setup() => RequestBuilder.OnUpdate.Subscribe(1000f, UnRando);

    private static Dictionary<string, (int, int)> GEO_COSTS = new()
    {
        ["Iselda"] = (100, 200),
        ["Salubra"] = (100, 300),
        ["Salubra_(Requires_Charms)"] = (250, 450),
        ["Sly"] = (150, 350),
        ["Sly_(Key)"] = (350, 600),
    };

    private static Dictionary<string, int> SHOP_COUNTS = new()
    {
        ["Iselda"] = 1,
        ["Salubra"] = 1,
        ["Salubra_(Requires_Charms)"] = 2,
        ["Sly"] = 2,
        ["Sly_(Key)"] = 2,
    };

    private static CostDef[]? GetVanillaCosts(RequestBuilder rb, string loc, System.Random r)
    {
        if (GEO_COSTS.TryGetValue(loc, out var costs))
        {
            var (low, high) = costs;
            return [new CostDef("GEO", r.Next(low, high + 1))];
        }

        return [];
    }

    private static void UnRando(RequestBuilder rb)
    {
        if (!RandoInterop.IsEnabled) return;

        List<int> totalHolder = [0];
        System.Random r = new(rb.gs.Seed + 9187);
        foreach (var igb in rb.EnumerateItemGroups())
        {
            igb.LocationPadder = (factory, count) =>
            {
                List<IRandoLocation> locs = [];
                for (int i = 0; i < count; i++)
                {
                    int randomSlot = r.Next(totalHolder[0]) + 1;
                    UnRandoLocation loc = new(randomSlot);
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
                if (SHOP_COUNTS.TryGetValue(loc, out var overrideCount)) count = System.Math.Min(count, overrideCount);

                subTotal += count;
                for (int i = 0; i < count; i++) rb.AddToPreplaced(new VanillaDef(nameof(UnRandoCheck), loc, GetVanillaCosts(rb, loc, r)));
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
    }
}
