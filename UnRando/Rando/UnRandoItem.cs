using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.Util;
using System.Collections.Generic;
using System.Linq;

namespace UnRando.Rando;

internal class UnRandoPlacementTag : Tag
{
    public string? PlacementName;
}

internal class UnRandoCheck : AbstractItem
{
    public UnRandoCheck() => name = nameof(UnRandoCheck);

    public override AbstractItem Clone() => new UnRandoCheck();

    private AbstractPlacement? GetRealPlacement(bool forReal)
    {
        var mod = UnRandoModule.Get()!;

        int checksRequired = forReal ? ++mod.ChecksObtained : (mod.ChecksObtained + 1);
        UnRandoLocation loc = new(checksRequired);
        return ItemChanger.Internal.Ref.Settings.Placements[loc.name];
    }

    public override bool GiveEarly(string containerType) => GetRealPlacement(false)?.Items.Any(i => i.GiveEarly(containerType)) ?? false;

    private static AbstractItem LumaflyEscape() => Finder.GetItem("Lumafly_Escape")!;

    public override void GiveImmediate(GiveInfo info)
    {
        var p = GetRealPlacement(true);
        var callback = info.Callback;
        List<AbstractItem> items = new(p?.Items ?? [LumaflyEscape()]);

        ItemUtility.GiveSequentially(items, p, info, () => callback?.Invoke(this));

        // Place refillables on location.
        var checkPlacement = ItemChanger.Internal.Ref.Settings.Placements[GetTag<UnRandoPlacementTag>()!.PlacementName!];
        foreach (var item in items)
        {
            var tag = item.GetTag<PersistentItemTag>();
            if (tag != null && tag.Persistence == Persistence.SemiPersistent)
            {
                // Place a pre-obtained copy of this item at this location.
                var clone = item.Clone();
                clone.SetObtained();
                checkPlacement.Items.Insert(0, item);
            }
        }

        // Null out the callback to prevent early control.
        UIDef = null;
        info.Callback = null;
    }

    public override void ResolveItem(GiveEventArgs args)
    {
        args.Item = this;

        var p = GetRealPlacement(false);
        List<UIDef> uiDefs = [];
        if (p != null)
        {
            foreach (var item in p.Items)
            {
                GiveEventArgs delegateArgs = new(item, item, args.Placement, args.Info, args.OriginalState);
                item.ResolveItem(delegateArgs);
                uiDefs.Add(delegateArgs.Item!.UIDef!);
            }
        }
        else uiDefs.Add(LumaflyEscape().GetResolvedUIDef()!);

        UIDef = new MultiUIDef(uiDefs);
    }
}
