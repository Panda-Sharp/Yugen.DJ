using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using Yugen.Audio.Samples.Views;

namespace Yugen.Audio.Samples.Constants
{
    public static class Menu
    {
        public static List<NavigationViewItemBase> MenuList = new List<NavigationViewItemBase>
        {
            NewNavigationViewItem(nameof(HomePage)),
            NewNavigationViewItem(nameof(AudioGraphPage)),
            NewNavigationViewItem(nameof(CsCorePage)),
            NewNavigationViewItem(nameof(SharpDXPage)),
            NewNavigationViewItem(nameof(AudioFrameInputNodePage)),
            NewNavigationViewItem(nameof(WaveformPage)),
            NewNavigationViewItem(nameof(VinylPage)),
            NewNavigationViewItem(nameof(DeckPage))
        };

        public static NavigationViewItem NewNavigationViewItem(string key) =>
            new NavigationViewItem
            {
                Content = key,
                Tag = key,
                Icon = new Windows.UI.Xaml.Controls.FontIcon { Glyph = "\uE80F" },
                IsExpanded = true,
                SelectsOnInvoked = true
            };
    }
}
