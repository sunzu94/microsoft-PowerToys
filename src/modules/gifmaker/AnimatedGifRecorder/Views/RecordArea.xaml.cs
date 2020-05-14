using AnimatedGifRecorder.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        private ResizableRectangle rect;
        private void Area_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Area.CaptureMouse();
                var point1 = e.GetPosition(Area);

                if (rect != null)
                {
                    Area.Children.Remove(rect);
                }

                rect = new ResizableRectangle(Area, point1);
                Area.Children.Add(rect);

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
                    if (rect != null)
                    {
                        rect.SetCoordinates(point2);
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

                if (rect.IsZeroSize())
                {
                    Area.Children.Remove(rect);
                    rect = null;
                }

                e.Handled = true;
            }
            catch (Exception)
            {
                Debug.WriteLine("canvas mouse up error");
            }
        }
    }
}
