using AnimatedGifRecorder.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace AnimatedGifRecorder.Views
{
    /// <summary>
    /// Interaction logic for RecordArea.xaml
    /// Resizable Rectangle from: https://github.com/kareemsulthan07/ResizableRectangleSample
    /// </summary>
    public partial class RecordArea : UserControl
    {
        public RecordArea()
        {
            InitializeComponent();
            recording = false;
            UpdateUI();

        }

        private ResizableRectangle targetArea;

        public Point GetFirstPoint() => targetArea.Point1;
        public Point GetSecondPoint() => targetArea.Point2;

        public bool Recording
        {
            set 
            {
                recording = value;
                UpdateUI();
            }
        }

        private bool recording;
        
        private void UpdateUI()
        {
            if (!recording)
            {
                Area.Visibility = Visibility.Visible;
                ExclusionPath.Visibility = Visibility.Collapsed;
            } 
            else
            {
                Area.Visibility = Visibility.Collapsed;
                ExclusionPath.Visibility = Visibility.Visible;
            }

        }

        private void Area_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Area.CaptureMouse();
                var point1 = e.GetPosition(Area);

                if (targetArea != null)
                {
                    Area.Children.Remove(targetArea);
                }

                targetArea = new ResizableRectangle(point1);
                Area.Children.Add(targetArea);

                e.Handled = true;
            }
            catch (Exception)
            {
                Debug.WriteLine("canvas mouse down error");
            }
        }

        private void Area_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var point2 = e.GetPosition(Area);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (targetArea != null)
                    {
                        targetArea.SetCoordinates(point2);
                    }
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("canvas mouse move error");
            }
        }

        private void Area_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Area.ReleaseMouseCapture();

                if (targetArea.IsZeroSize())
                {
                    Area.Children.Remove(targetArea);
                    targetArea = null;
                }

                Exclusion.Rect = new Rect(targetArea.Point1, targetArea.Point2);
                e.Handled = true;
            }
            catch (Exception)
            {
                Debug.WriteLine("canvas mouse up error");
            }
        }
    }
}
