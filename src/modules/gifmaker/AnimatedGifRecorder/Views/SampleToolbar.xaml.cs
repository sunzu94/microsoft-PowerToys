using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnimatedGifRecorder.Views
{
    /// <summary>
    /// Interaction logic for SampleToolbar.xaml
    /// </summary>
    public partial class SampleToolbar : UserControl
    {
        Recorder recorder;

        public SampleToolbar()
        {
            InitializeComponent();
            DataContext = this;
            ImageUri = imageRecord;
        }

        private readonly string imageRecord = "pack://application:,,,/Resources/media-record.png";
        private readonly string imagePause = "pack://application:,,,/Resources/media-pause.png";

        public static readonly DependencyProperty ImageUriProperty = DependencyProperty.Register("ImageUri", typeof(string), typeof(SampleToolbar));

        public string ImageUri
        {
            get => (string)GetValue(ImageUriProperty);
            set => SetValue(ImageUriProperty, value);
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Cross;
            RecordPauseButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            if (RecordPauseText.Text == "Pause")
            {
                ImageUri = imageRecord;
                RecordPauseText.Text = "Record";
            }
            recorder = new Recorder(new RecorderConf() { Width = 800, Height = 600, X = 50, Y=50, FrameRate = 10});
        }

        private void RecordPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (RecordPauseText.Text == "Record")
            {
                Cursor = Cursors.Arrow;
                ImageUri = imagePause;
                RecordPauseText.Text = "Pause";
                StopButton.IsEnabled = true;
                recorder.Start();
            }
            else
            {
                ImageUri = imageRecord;
                RecordPauseText.Text = "Record";
                recorder.Pause();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            RecordPauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;
            if (RecordPauseText.Text == "Pause")
            {
                ImageUri = imageRecord;
                RecordPauseText.Text = "Record";
            }
            recorder.Stop();
        }
    }
}
