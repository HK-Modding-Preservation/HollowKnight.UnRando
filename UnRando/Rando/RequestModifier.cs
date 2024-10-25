using ItemChanger;
using ItemChanger.Locations;
using RandomizerCore;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnRando.Rando;

internal record ShopData
{
    public readonly string costType;
    public readonly int lowCost;
    public readonly int highCost;
    public readonly int itemCount;

    public ShopData(int itemCount, (int, int) costs)
    {
        this.itemCount = itemCount;
        costType = "GEO";
        (lowCost, highCost) = costs;
    }

    public ShopData(int itemCount, string costType)
    {
        this.itemCount = itemCount;
        this.costType = costType;
        lowCost = 1;
        highCost = 1;
    }
}

internal class RequestModifier
{
    internal static void Setup() => RequestBuilder.OnUpdate.Subscribe(1000f, ApplyUnRando);

    private static readonly Dictionary<string, ShopData> SHOP_DATA = new()
    {
        ["Egg_Shop"] = new(4, "RANCIDEGGS"),
        ["Grubfather"] = new(4, "GRUBS"),
        ["Iselda"] = new(1, (100, 200)),
        ["Leg_Eater"] = new(1, (150, 250)),
        ["Salubra"] = new(1, (100, 300)),
        ["Salubra_(Requires_Charms)"] = new(3, (350, 600)),
        ["Seer"] = new(4, "ESSENCE"),
        ["Sly"] = new(1, (100, 200)),
        ["Sly_(Key)"] = new(2, (350, 600)),
    };

    private static CostDef[]? GetVanillaCosts(string loc, System.Random r)
    {
        if (SHOP_DATA.TryGetValue(loc, out var data) && data.costType == "GEO") return [new CostDef(data.costType, r.Next(data.lowCost, data.highCost + 1))];
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
            var splitGroup = GetSplitGroup(igb.label);
            if (splitGroup != UnRando.GS.RandoSettings.SplitGroup) continue;

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
                else if (Finder.GetLocation(loc) is CustomShopLocation) count = 4;

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
    }

    private static int GetSplitGroup(string label)
    {
        if (!label.StartsWith(RBConsts.SplitGroupPrefix)) return -1;
        else return int.Parse(label.Substring(RBConsts.SplitGroupPrefix.Length));
    }
}
