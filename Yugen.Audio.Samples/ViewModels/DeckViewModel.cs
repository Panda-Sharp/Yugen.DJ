using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private WaveformViewModel _waveformViewModel;

        public DeckViewModel(WaveformViewModel waveformViewModel)
        {
            _waveformViewModel = waveformViewModel;
         
            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
        }

        public ICommand OpenCommand { get; }

        private async Task OpenCommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                     new List<string> { ".mp3" },
                     PickerLocationId.MusicLibrary
                 );

            if (audioFile != null)
            {
                var ras = await audioFile.OpenReadAsync();
                var stream = ras.AsStreamForRead();

                await _waveformViewModel.GenerateAudioData(stream);
            }
        }
    }
}