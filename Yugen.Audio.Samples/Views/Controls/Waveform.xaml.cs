using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views.Controls
{
    public sealed partial class Waveform : UserControl
    {
        public Waveform()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<WaveformViewModel>();
            
            ViewModel.WaveformGenerated += OnWaveformCanvasGenerated;
        }

        private WaveformViewModel ViewModel => (WaveformViewModel)DataContext;

        private void OnWaveformCanvasGenerated(object sender, EventArgs e) =>
            WaveformCanvas.Invalidate();

        private void OnWaveformCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args) =>
            ViewModel.WaveformRendererServiceDrawLine(sender, args.DrawingSession);

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.WaveformGenerated -= OnWaveformCanvasGenerated;
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            WaveformCanvas.RemoveFromVisualTree();
            WaveformCanvas = null;
        }
    }
}