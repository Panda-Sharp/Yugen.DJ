using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using Yugen.Audio.Samples.Views;

namespace Yugen.Audio.Samples.Constants
{
    public static class Menu
    {
        public static List<NavigationViewItemBase> MenuList = new List<NavigationViewItemBase>
        {
            NewNavigationViewItem(nameof(AudioGraphPage), nameof(AudioGraphPage)),
            NewNavigationViewItem(nameof(CsCorePage), nameof(CsCorePage)),
            NewNavigationViewItem(nameof(SharpDXPage), nameof(SharpDXPage)),
            NewNavigationViewItem(nameof(AudioFrameInputNodePage), nameof(AudioFrameInputNodePage)),
            NewNavigationViewItem(nameof(WaveformPage), nameof(WaveformPage))
        };

        public static NavigationViewItem NewNavigationViewItem(string content, string tag) =>
            new NavigationViewItem
            {
                Content = content,
                Tag = tag,
                Icon = new Windows.UI.Xaml.Controls.FontIcon { Glyph = "\uE80F" },
                IsExpanded = true,
                SelectsOnInvoked = true
            };
    }
}
