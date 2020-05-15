using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AnimatedGifRecorder.Views
{
    /// <summary>
    /// Interaction logic for SampleToolbar.xaml
    /// </summary>
    public partial class SampleToolbar : UserControl
    {

        public SampleToolbar()
        {
            InitializeComponent();
            DataContext = this;
            ImageUri = imageRecord;
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        private readonly string imageRecord = "pack://application:,,,/Resources/media-record.png";
        private readonly string imagePause = "pack://application:,,,/Resources/media-pause.png";

        public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register("ImageUri", typeof(string), typeof(SampleToolbar));

        public string ImageUri
        {
            get => (string)GetValue(ImageUriProperty);
            set => SetValue(ImageUriProperty, value);
        }

        private readonly DispatcherTimer dt = new DispatcherTimer();
        private readonly Stopwatch sw = new Stopwatch();
        private string currentTime = string.Empty;

        private void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = string.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                StopwatchText.Text = currentTime;
            }
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            RecordPauseButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            if (RecordPauseText.Text == "Pause")
            {
                ImageUri = imageRecord;
                RecordPauseText.Text = "Record";
            }
        }

        private void RecordPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecordPauseText.Text == "Record")
            {
                sw.Start();
                dt.Start();
                ImageUri = imagePause;
                RecordPauseText.Text = "Pause";
                StopButton.IsEnabled = true;
            }
            else
            {
                if (sw.IsRunning)
                {
                    sw.Stop();
                }
                ImageUri = imageRecord;
                RecordPauseText.Text = "Record";
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }
            sw.Reset();
            StopwatchText.Text = "00:00:00";
            RecordPauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;
            if (RecordPauseText.Text == "Pause")
            {
                ImageUri = imageRecord;
                RecordPauseText.Text = "Record";
            }
        }
    }
}
