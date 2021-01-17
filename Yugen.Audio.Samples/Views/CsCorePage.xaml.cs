using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views
{
    public sealed partial class CsCorePage : Page
    {
        public CsCorePage()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<CsCoreViewModel>();
        }

        private CsCoreViewModel ViewModel => (CsCoreViewModel)DataContext;
    }
}