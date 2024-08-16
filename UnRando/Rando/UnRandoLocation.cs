using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace UnRando.Rando;

internal class UnRandoLocation : AutoLocation
{
    public int checksRequired;

    public UnRandoLocation(int checksRequired)
    {
        this.checksRequired = checksRequired;

        name = $"UnRandoLocation-{checksRequired:D3}";
        flingType = ItemChanger.FlingType.DirectDeposit;
    }

    internal void GetInfo(LocationRequestInfo info)
    {
        info.getLocationDef = () =>
        {
            LocationDef def = new()
            {
                AdditionalProgressionPenalty = false,
                FlexibleCount = true,
                Name = name,
                SceneName = "Unknown",
            };
            return def;
        };
    }

    public override AbstractPlacement Wrap()
    {
        var placement = base.Wrap();
        placement.AddTag<CompletionWeightTag>().Weight = 0;
        return placement;
    }

    protected override void OnLoad() { }

    protected override void OnUnload() { }
}
