using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using Modding;
using PurenailCore.SystemUtil;
using RandomizerMod.Menu;
using RandomizerMod.Settings;
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
    private readonly MenuItem<bool> enableSetting;
    private readonly NumericEntryField<int> splitGroupSetting;

    private RandomizationSettings Settings => UnRando.GS.RandoSettings;

    private ConnectionMenu(MenuPage connectionsPage)
    {
        MenuPage unRandoPage = new("UnRando Main Page", connectionsPage);
        entryButton = new(connectionsPage, Localize("UnRando"));
        entryButton.AddHideAndShowEvent(unRandoPage);

        factory = new(unRandoPage, Settings);
        enableSetting = (factory.ElementLookup[nameof(RandomizationSettings.Enabled)] as MenuItem<bool>)!;
        enableSetting.OnClick += UpdateUI;
        splitGroupSetting = (factory.ElementLookup[nameof(RandomizationSettings.SplitGroup)] as NumericEntryField<int>)!;

        VerticalItemPanel panel = new(unRandoPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_LARGE, true, [enableSetting, splitGroupSetting]);
        UpdateUI();
    }

    internal void UpdateUI()
    {
        if (enableSetting.Value)
        {
            splitGroupSetting.Label.Text.color = Colors.DEFAULT_COLOR;
            splitGroupSetting.InputField.interactable = true;
            splitGroupSetting.InputField.textComponent.color = Colors.DEFAULT_COLOR;

            entryButton.Text.color = Colors.TRUE_COLOR;
        }
        else
        {
            splitGroupSetting.Label.Text.color = Colors.FALSE_COLOR;
            splitGroupSetting.InputField.interactable = false;
            splitGroupSetting.InputField.textComponent.color = Colors.FALSE_COLOR;

            entryButton.Text.color = Colors.DEFAULT_COLOR;
        }
    }

    internal void ApplySettings(RandomizationSettings settings)
    {
        factory.SetMenuValues(settings);
        UpdateUI();
    }
}
