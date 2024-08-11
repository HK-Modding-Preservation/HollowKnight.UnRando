using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace UnRando.Rando;

internal class SettingsProxy : RandoSettingsProxy<RandomizationSettings, string>
{
    public override string ModKey => nameof(UnRando);

    public override VersioningPolicy<string> VersioningPolicy => new StrictModVersioningPolicy(UnRando.Instance);

    public override bool TryProvideSettings(out RandomizationSettings? settings)
    {
        settings = UnRando.GS.RandoSettings;
        return settings.Enabled;
    }

    public override void ReceiveSettings(RandomizationSettings? settings) => ConnectionMenu.Instance.ApplySettings(settings ?? new());
}
