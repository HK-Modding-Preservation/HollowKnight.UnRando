using ItemChanger;
using ItemChanger.UIDefs;
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

    public override void GiveImmediate(GiveInfo info)
    {
        var p = GetRealPlacement(true);
        GiveInfo delegateInfo = new()
        {
            Container = info.Container,
            FlingType = info.FlingType,
            MessageType = info.MessageType,
            Transform = info.Transform,
        };

        if (p != null) foreach (var item in p.Items) item.Give(p, delegateInfo);
        else Finder.GetItem("Lumafly_Escape")!.Give(p, delegateInfo);

        UIDef = null;
    }

    public override void ResolveItem(GiveEventArgs args)
    {
        args.Item = this;

        List<string> previewStrings = [];
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
        else
        {
            uiDefs.Add(new MsgUIDef()
            {
                name = new BoxedString("Something!"),
                shopDesc = new BoxedString("The rando must go on"),
                sprite = new BoxedSprite(Finder.GetItem("Lumafly_Escape")!.GetPreviewSprite()!)
            });
        }

        UIDef = new MultiUIDef(uiDefs);
    }
}
