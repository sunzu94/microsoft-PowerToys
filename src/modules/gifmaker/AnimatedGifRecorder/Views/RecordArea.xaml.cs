using AnimatedGifRecorder.Controls;
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
                Area.CaptureMouse(e.Pointer);
                var ptrPoint = e.GetPosition(Area);
                var point1 = ptrPoint.Position;

                Area.Children.OfType<ResizableRectangle>()
                    .ToList()
                    .ForEach(r => r.IsSelected = false);

                rect = new ResizableRectangle(Area, point1);
                Area.Children.Add(rect);

                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void Area_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var ptrPoint = e.GetPosition(Area);
                if (ptrPoint.Properties.IsLeftButtonPressed)
                {
                    if (rect != null)
                    {
                        var point2 = ptrPoint.Position;
                        rect.SetCoordinates(point2);
                    }
                }
            }
            catch (Exception)
            {
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
                }

                rect = null;

                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }
    }
}
