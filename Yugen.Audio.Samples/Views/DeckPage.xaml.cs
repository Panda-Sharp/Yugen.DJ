using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views
{
    public sealed partial class DeckPage : Page
    {
        public DeckPage()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<DeckViewModel>();
        }

        private DeckViewModel ViewModel => (DeckViewModel)DataContext;
    }
}