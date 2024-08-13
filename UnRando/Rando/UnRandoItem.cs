using ItemChanger;
using ItemChanger.Placements;
using ItemChanger.Tags;
using ItemChanger.Util;
using RecentItemsDisplay;
using System.Collections.Generic;
using System.Linq;

namespace UnRando.Rando;

internal class UnRandoPlacementTag : Tag
{
    public string? PlacementName;
}

internal class UnRandoCheck : AbstractItem
{
    public UnRandoCheck()
    {
        name = nameof(UnRandoCheck);

        // Don't display un-rando checks.
        var interop = AddTag<InteropTag>();
        interop.Message = "RecentItems";
        interop.Properties.Add("IgnoreItem", true);
    }

    public override AbstractItem Clone() => new UnRandoCheck();

    private AbstractPlacement? GetRealPlacement(bool forReal)
    {
        var mod = UnRandoModule.Get()!;

        int checksRequired = forReal ? ++mod.ChecksObtained : (mod.ChecksObtained + 1);
        UnRandoLocation loc = new(checksRequired);
        return ItemChanger.Internal.Ref.Settings.Placements[loc.name];
    }

    public override bool GiveEarly(string containerType) => GetRealPlacement(false)?.Items.Any(i => i.GiveEarly(containerType)) ?? false;

    private static AbstractItem Nothing() => Finder.GetItem("Lumafly_Escape")!;

    public override void GiveImmediate(GiveInfo info)
    {
        var p = GetRealPlacement(true);
        var callback = info.Callback;

        var checkPlacement = ItemChanger.Internal.Ref.Settings.Placements[GetTag<UnRandoPlacementTag>()!.PlacementName!];
        var scene = (checkPlacement as IPrimaryLocationPlacement)?.Location.sceneName;

        // Place refillables on location.
        List<AbstractItem> items = new(p?.Items ?? [Nothing()]);
        List<AbstractItem> toInsert = [];
        foreach (var item in items)
        {
            if (scene != null)
            {
                var recentItems = item.AddTag<InteropTag>();
                recentItems.Message = "RecentItems";
                recentItems.Properties.Add("DisplaySource", AreaName.LocalizedCleanAreaName(scene));
            }

            var tag = item.GetTag<PersistentItemTag>();
            if (tag != null && tag.Persistence == Persistence.SemiPersistent)
            {
                // Place a pre-obtained copy of this item at this location.
                var clone = item.Clone();
                clone.SetObtained();
                toInsert.Add(clone);
            }
        }

        ItemUtility.GiveSequentially(items, p, info, () =>
        {
            callback?.Invoke(this);
            checkPlacement.Items.InsertRange(0, toInsert);
        });

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
        else uiDefs.Add(Nothing().GetResolvedUIDef()!);

        UIDef = new MultiUIDef(uiDefs);
    }
}
