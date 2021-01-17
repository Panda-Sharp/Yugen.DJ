using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ.Views.Controls
{
    public sealed partial class Mixer : UserControl
    {
        public Mixer()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<MixerViewModel>();
        }

        private MixerViewModel ViewModel => (MixerViewModel)DataContext;
    }
}