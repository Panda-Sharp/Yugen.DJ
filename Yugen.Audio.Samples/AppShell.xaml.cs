using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples
{
    public sealed partial class AppShell : Page
    {
        public AppShell()
        {
            InitializeComponent();

            DataContext = App.Current.Services.GetService<AppShellViewModel>();
        }

        private AppShellViewModel ViewModel => (AppShellViewModel)DataContext;
    }
}
