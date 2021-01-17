using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Models;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ.Views.Controls
{
    public sealed partial class Volume : UserControl
    {
        public Volume()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<VolumeViewModel>();
        }

        public Side Side
        {
            set => ViewModel.Side = value;
        }

        private VolumeViewModel ViewModel => (VolumeViewModel)DataContext;
    }
}