using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Yugen.DJ.Views;

namespace Yugen.DJ.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            SettingsCommand = new AsyncRelayCommand(SettingsCommandBehavior);
        }

        public ICommand SettingsCommand { get; }

        private async Task SettingsCommandBehavior()
        {
            var settingsDialog = new SettingsDialog();
            await settingsDialog.ShowAsync();
        }
    }
}