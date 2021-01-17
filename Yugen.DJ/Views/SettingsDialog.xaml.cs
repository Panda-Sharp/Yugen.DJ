using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ.Views
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public SettingsDialog()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<SettingsViewModel>();
        }

        private SettingsViewModel ViewModel => (SettingsViewModel)DataContext;
    }
}