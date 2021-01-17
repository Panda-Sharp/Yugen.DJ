using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views
{
    public sealed partial class AudioGraphPage : Page
    {
        public AudioGraphPage()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<AudioGraphViewModel>();
        }

        private AudioGraphViewModel ViewModel => (AudioGraphViewModel)DataContext;
    }
}