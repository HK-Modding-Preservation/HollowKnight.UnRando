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
        ["Iselda"] = (25, 75),
        ["Salubra"] = (75, 150),
        ["Sly"] = (50, 100),
        ["Sly_(Key)"] = (150, 250),
    };

    private static CostDef[]? GetVanillaCosts(RequestBuilder rb, string loc, System.Random r)
    {
        if (GEO_COSTS.TryGetValue(loc, out var costs))
        {
            var (low, high) = costs;
            return [new CostDef("GEO", r.Next(low, high + 1))];
        }

        return [];
        // if (rb.Vanilla.TryGetValue(loc, out var defs) && defs.Count == 1 && defs[0].Costs != null && defs[0].Costs!.Length > 0) return defs[0].Costs!;
        // else return null;
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
