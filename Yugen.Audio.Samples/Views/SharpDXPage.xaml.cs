using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views
{
    public sealed partial class SharpDXPage : Page
    {
        public SharpDXPage()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<SharpDXViewModel>();
        }

        private SharpDXViewModel ViewModel => (SharpDXViewModel)DataContext;
    }
}