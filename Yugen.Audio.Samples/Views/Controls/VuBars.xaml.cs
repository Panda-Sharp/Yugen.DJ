using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels.Controls;

namespace Yugen.Audio.Samples.Views.Controls
{
    public sealed partial class VuBars : UserControl
    {
        public VuBars()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<VuBarsVieModel>();
        }

        private VuBarsVieModel ViewModel => (VuBarsVieModel)DataContext;
    }
}