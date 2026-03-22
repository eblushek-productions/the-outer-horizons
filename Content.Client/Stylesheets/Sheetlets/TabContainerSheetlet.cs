using Content.Client.Stylesheets.SheetletConfigs;
using Content.Client.Stylesheets.Stylesheets;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Content.Client.Stylesheets.StylesheetHelpers;

namespace Content.Client.Stylesheets.Sheetlets;

[CommonSheetlet]
public sealed class TabContainerSheetlet<T> : Sheetlet<T> where T: PalettedStylesheet, ITabContainerConfig
{
    public override StyleRule[] GetRules(T sheet, object config)
    {
        ITabContainerConfig tabCfg = sheet;

        var tabContainerPanel = sheet.GetTextureOr(tabCfg.TabContainerPanelPath, NanotrasenStylesheet.TextureRoot)
            .IntoPatch(StyleBox.Margin.All, 2);

        var tabContainerBoxActive = new StyleBoxFlat(sheet.SecondaryPalette.Element);
        tabContainerBoxActive.SetContentMarginOverride(StyleBox.Margin.Horizontal, 5);
        var tabContainerBoxInactive = new StyleBoxFlat(sheet.SecondaryPalette.Background);
        tabContainerBoxInactive.SetContentMarginOverride(StyleBox.Margin.Horizontal, 5);


        var tabContainerCyanActive = new StyleBoxFlat(Color.FromHex("#2F4C52"));
        tabContainerCyanActive.SetContentMarginOverride(StyleBox.Margin.Horizontal, 5);
        var tabContainerCyanInactive = new StyleBoxFlat(Color.FromHex("#122B30"));
        tabContainerCyanInactive.SetContentMarginOverride(StyleBox.Margin.Horizontal, 5);

        return
        [
            E<TabContainer>()
                .Prop(TabContainer.StylePropertyPanelStyleBox, tabContainerPanel)
                .Prop(TabContainer.StylePropertyTabStyleBox, tabContainerBoxActive)
                .Prop(TabContainer.StylePropertyTabStyleBoxInactive, tabContainerBoxInactive),

            //
            E<TabContainer>()
                .Class("CyanTab")
                .Prop(TabContainer.StylePropertyPanelStyleBox, tabContainerPanel)
                .Prop(TabContainer.StylePropertyTabStyleBox, tabContainerCyanActive)
                .Prop(TabContainer.StylePropertyTabStyleBoxInactive, tabContainerCyanInactive),
        ];
    }
}
