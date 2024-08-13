using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using Modding;
using PurenailCore.SystemUtil;
using RandomizerMod.Menu;
using RandoSettingsManager;
using System.Collections.Generic;
using static RandomizerMod.Localization;

namespace UnRando.Rando;

internal class ConnectionMenu
{
    internal static ConnectionMenu? Instance { get; private set; }

    internal static void Setup()
    {
        RandomizerMenuAPI.AddMenuPage(OnRandomizerMenuConstruction, TryGetMenuButton);
        MenuChangerMod.OnExitMainMenu += () => Instance = null;

        if (ModHooks.GetMod("RandoSettingsManager") is Mod) HookRandoSettingsManager();
    }

    private static void HookRandoSettingsManager() => RandoSettingsManagerMod.Instance.RegisterConnection(new SettingsProxy());

    private static void OnRandomizerMenuConstruction(MenuPage page) => Instance = new(page);

    private static bool TryGetMenuButton(MenuPage page, out SmallButton button)
    {
        button = Instance!.entryButton;
        return true;
    }

    private readonly SmallButton entryButton;
    private readonly MenuElementFactory<RandomizationSettings> factory;
    private readonly MenuItem<bool> enableSettings;
    private readonly List<MenuItem<ProgressionSetting>> progressionSettings = [];

    private RandomizationSettings Settings => UnRando.GS.RandoSettings;

    private ConnectionMenu(MenuPage connectionsPage)
    {
        MenuPage unRandoPage = new("UnRando Main Page", connectionsPage);
        entryButton = new(connectionsPage, Localize("UnRando"));
        entryButton.AddHideAndShowEvent(unRandoPage);

        factory = new(unRandoPage, Settings);
        enableSettings = (factory.ElementLookup[nameof(RandomizationSettings.Enabled)] as MenuItem<bool>)!;
        enableSettings.OnClick += UpdateUI;

        factory.ElementLookup.Values.ForEach(v =>
        {
            if (v is MenuItem<ProgressionSetting> typed) progressionSettings.Add(typed);
        });

        List<IMenuElement> children = [enableSettings];
        progressionSettings.ForEach(children.Add);
        VerticalItemPanel panel = new(unRandoPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_MEDIUM, true, [.. children]);

        UpdateUI();
    }

    internal void UpdateUI()
    {
        if (enableSettings.Value)
        {
            progressionSettings.ForEach(m => m.Unlock());
            entryButton.Text.color = Colors.TRUE_COLOR;
        }
        else
        {
            progressionSettings.ForEach(m => m.Lock());
            entryButton.Text.color = Colors.DEFAULT_COLOR;
        }
    }

    internal void ApplySettings(RandomizationSettings settings)
    {
        factory.SetMenuValues(settings);
        UpdateUI();
    }
}
