using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views
{
    public sealed partial class WaveformPage : Page
    {
        public WaveformPage()
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
    }
}