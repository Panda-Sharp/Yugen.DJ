using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using Yugen.Audio.Samples.Constants;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Services;

namespace Yugen.Audio.Samples.ViewModels
{
    /// <summary>
    /// AppShellViewModel
    /// </summary>
    public class AppShellViewModel : ViewModelBase
    {
        public AppShellViewModel()
        {
            NavigationViewOnItemInvokedCommand = new RelayCommand<NavigationViewItemInvokedEventArgs>(NavigationViewOnItemInvokedCommandBehavior);
        }

        public IEnumerable<NavigationViewItemBase> NavItems => Menu.MenuList;

        public ICommand NavigationViewOnItemInvokedCommand { get; }

        private void NavigationViewOnItemInvokedCommandBehavior(NavigationViewItemInvokedEventArgs args)
        {
            var tag = args.InvokedItemContainer.Tag?.ToString();

            NavigationService.NavigateToPage(tag);
        }
    }
}