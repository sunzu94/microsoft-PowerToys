using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimatedGifRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Recorder recorder;
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += CloseOnEsc;

            ToolbarElement.CaptureButton.Click += CaptureButton_Click;
            ToolbarElement.RecordPauseButton.Click += RecordPauseButton_Click;
            ToolbarElement.StopButton.Click += StopButton_Click;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            recorder.Stop();
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Cross;
            var p1 = RecordAreaElement.GetFirstPoint();
            var p2 = RecordAreaElement.GetSecondPoint();

            var X = (int)Math.Min(p1.X, p2.X);
            var Width = (int)Math.Max(p1.X, p2.X) - X;
            var Y = (int)Math.Min(p1.Y, p2.Y);
            var Height = (int)Math.Min(p1.Y, p2.Y) - Y;

            recorder = new Recorder(new RecorderConf
            {
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                FrameRate = 10
            });
        }

        private void RecordPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToolbarElement.RecordPauseText.Text == "Record")
            {
                RecordAreaElement.IsEnabled = false;
                recorder.Start();
            }

            Cursor = Cursors.Arrow;
        }

        private void CloseOnEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
