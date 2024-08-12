using ItemChanger;
using ItemChanger.Util;
using System.Collections.Generic;
using System.Linq;

namespace UnRando.Rando;

internal class UnRandoCheck : AbstractItem
{
    public UnRandoCheck() => name = nameof(UnRandoCheck);

    public override AbstractItem Clone() => new UnRandoCheck();

    private AbstractPlacement? GetRealPlacement(bool forReal)
    {
        var mod = UnRandoModule.Get()!;

        int checksRequired = forReal ? ++mod.ChecksObtained : (mod.ChecksObtained + 1);
        UnRandoLocation loc = new(checksRequired);
        return ItemChangerMod.GetPlacement(loc.name);
    }

    public override bool GiveEarly(string containerType) => GetRealPlacement(false)?.Items.Any(i => i.GiveEarly(containerType)) ?? false;

    private static AbstractItem? _LumaflyEscape = null;
    private static AbstractItem LumaflyEscape()
    {
        _LumaflyEscape ??= Finder.GetItem("Lumafly_Escape")!;
        return _LumaflyEscape;
    }

    public override void GiveImmediate(GiveInfo info)
    {
        var p = GetRealPlacement(true);
        var callback = info.Callback;
        ItemUtility.GiveSequentially(p?.Items ?? [LumaflyEscape()], p, info, () => callback?.Invoke(this));

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
