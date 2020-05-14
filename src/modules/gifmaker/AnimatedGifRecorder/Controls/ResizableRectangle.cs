using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnimatedGifRecorder.Controls
{
    /// <summary>
    /// For drawing screen area to select for recording
    /// Resizable Rectangle from: https://github.com/kareemsulthan07/ResizableRectangleSample
    /// </summary>
    class ResizableRectangle : Canvas
    {
        private Point point1, point2;

        public Point Point1
        {
            get => point1;
        }

        public Point Point2
        {
            get => point2;
        }

        private readonly Rectangle FillRectangle;
        
        public ResizableRectangle(Point startPoint)
        {
            point1 = startPoint;

            FillRectangle = new Rectangle
            {
                Stroke = new SolidColorBrush(Colors.White),
                Fill = new SolidColorBrush(Colors.White),
                Opacity = .5,
                Visibility = Visibility.Visible,
                Name = "MyRectangle"
            };
            Children.Add(FillRectangle);
        }

        public void ResizeRectangle(Point point1, Point point2)
        {
            SetTop(FillRectangle, Math.Min(point1.Y, point2.Y));
            SetRight(FillRectangle, Math.Max(point1.X, point2.X));
            SetBottom(FillRectangle, Math.Max(point1.Y, point2.Y));
            SetLeft(FillRectangle, Math.Min(point1.X, point2.X));

            FillRectangle.Width = Math.Abs(point1.X - point2.X);
            FillRectangle.Height = Math.Abs(point1.Y - point2.Y);
        }

        public void SetCoordinates(Point point2)
        {
            this.point2 = point2;
            ResizeRectangle(point1, point2);
        }

        public bool IsZeroSize() => 
            Math.Abs(point1.X - point2.X) < 5 &&
            Math.Abs(point1.Y - point2.Y) < 5;
    }
}