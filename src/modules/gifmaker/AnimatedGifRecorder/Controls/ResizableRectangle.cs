using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AnimatedGifRecorder.Controls
{
    /// <summary>
    /// For drawing screen area to select for recording
    /// Resizable Rectangle from: https://github.com/kareemsulthan07/ResizableRectangleSample
    /// </summary>
    class ResizableRectangle : Canvas
    {
        private readonly Canvas parentCanvas;
        private Point point1, point2, pointA, pointB;
        private readonly Line line1, line2, line3, line4;
        private readonly SolidColorBrush lineStrokeBrush, rectFillBrush, rectStrokeBrush;
        private readonly double lineThickness = 2, adjustment;
        private readonly Style lineStyle, rectStyle;
        private readonly Rectangle topRect, bottomRect, leftRect, rightRect;
        private readonly double rectStrokeThickness = 0.5, rectWidth = 8, rectHeight = 8;
        private Point lastPosition;

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                if (IsSelected)
                {
                    topRect.Visibility = Visibility.Visible;
                    bottomRect.Visibility = Visibility.Visible;
                    leftRect.Visibility = Visibility.Visible;
                    rightRect.Visibility = Visibility.Visible;
                }
                else
                {
                    topRect.Visibility = Visibility.Collapsed;
                    bottomRect.Visibility = Visibility.Collapsed;
                    leftRect.Visibility = Visibility.Collapsed;
                    rightRect.Visibility = Visibility.Collapsed;
                }
            }
        }


        public ResizableRectangle(Canvas parentCanvas, Point point1)
        {
            this.parentCanvas = parentCanvas;
            this.point1 = pointA = point2 = pointB = point1;
            lineStrokeBrush = new SolidColorBrush(Colors.Black);
            rectFillBrush = new SolidColorBrush(Colors.White);
            rectStrokeBrush = new SolidColorBrush(Colors.Blue);
            adjustment = lineThickness / 2;


            lineStyle = new Style(typeof(Line));
            lineStyle.Setters.Add(new Setter(Shape.StrokeProperty, lineStrokeBrush));
            lineStyle.Setters.Add(new Setter(Shape.StrokeThicknessProperty, lineThickness));

            rectStyle = new Style(typeof(Rectangle));
            rectStyle.Setters.Add(new Setter(Shape.FillProperty, rectFillBrush));
            rectStyle.Setters.Add(new Setter(Shape.StrokeProperty, rectStrokeBrush));
            rectStyle.Setters.Add(new Setter(Shape.StrokeThicknessProperty, rectStrokeThickness));
            rectStyle.Setters.Add(new Setter(WidthProperty, rectWidth));
            rectStyle.Setters.Add(new Setter(HeightProperty, rectHeight));

            line1 = new Line()
            {
                X1 = point1.X,
                Y1 = point1.Y,
                X2 = point1.X,
                Y2 = point1.Y,
                Name = "line1",
                Style = lineStyle,
            };

            line2 = new Line()
            {
                X1 = point1.X,
                Y1 = point1.Y,
                X2 = point1.X,
                Y2 = point1.Y,
                Name = "line2",
                Style = lineStyle,
            };

            line3 = new Line()
            {
                X1 = point1.X,
                Y1 = point1.Y,
                X2 = point1.X,
                Y2 = point1.Y,
                Name = "line3",
                Style = lineStyle,
            };

            line4 = new Line()
            {
                X1 = point1.X,
                Y1 = point1.Y,
                X2 = point1.X,
                Y2 = point1.Y,
                Name = "line4",
                Style = lineStyle,
            };

            line1.MouseLeftButtonDown += Line_MouseLeftButtonDown;
            line1.MouseMove += Line_MouseMove;
            line1.MouseLeftButtonUp += Line_MouseLeftButtonUp;

            line2.MouseLeftButtonDown += Line_MouseLeftButtonDown;
            line2.MouseMove += Line_MouseMove;
            line2.MouseLeftButtonUp += Line_MouseLeftButtonUp;

            line3.MouseLeftButtonDown += Line_MouseLeftButtonDown;
            line3.MouseMove += Line_MouseMove;
            line3.MouseLeftButtonUp += Line_MouseLeftButtonUp;

            line4.MouseLeftButtonDown += Line_MouseLeftButtonDown;
            line4.MouseMove += Line_MouseMove;
            line4.MouseLeftButtonUp += Line_MouseLeftButtonUp;


            topRect = new Rectangle()
            {
                Name = "topRect",
                Style = rectStyle,
            };

            bottomRect = new Rectangle()
            {
                Name = "bottomRect",
                Style = rectStyle,
            };

            leftRect = new Rectangle()
            {
                Name = "leftRect",
                Style = rectStyle,
            };

            rightRect = new Rectangle()
            {
                Name = "rightRect",
                Style = rectStyle,
            };

            topRect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
            topRect.MouseMove += Rect_MouseMove;
            topRect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;

            bottomRect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
            bottomRect.MouseMove += Rect_MouseMove;
            bottomRect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;

            leftRect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
            leftRect.MouseMove += Rect_MouseMove;
            leftRect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;

            rightRect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
            rightRect.MouseMove += Rect_MouseMove;
            rightRect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;

            Children.Add(line1);
            Children.Add(line2);
            Children.Add(line3);
            Children.Add(line4);

            Children.Add(topRect);
            Children.Add(bottomRect);
            Children.Add(leftRect);
            Children.Add(rightRect);
        }

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var line = (Line)sender;
                line.CaptureMouse();
                lastPosition = e.GetPosition(this);
                IsSelected = true;

                parentCanvas.Children.OfType<ResizableRectangle>()
                    .Except(new List<ResizableRectangle>() { this })
                    .ToList()
                    .ForEach(r => r.IsSelected = false);


                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var ptrPoint = e.GetPosition(this);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var line = (Line)sender;
                    var position = ptrPoint;

                    var x = position.X - lastPosition.X;
                    var y = position.Y - lastPosition.Y;

                    point1 = new Point(point1.X + x, point1.Y + y);
                    pointA = new Point(pointA.X + x, pointA.Y + y);
                    point2 = new Point(point2.X + x, point2.Y + y);
                    pointB = new Point(pointB.X + x, pointB.Y + y);

                    line1.X1 = point1.X;
                    line1.Y1 = point1.Y;
                    line1.X2 = pointA.X;
                    line1.Y2 = pointA.Y;

                    line2.X1 = pointA.X;
                    line2.Y1 = pointA.Y;
                    line2.X2 = point2.X;
                    line2.Y2 = point2.Y;

                    line3.X1 = point2.X;
                    line3.Y1 = point2.Y;
                    line3.X2 = pointB.X;
                    line3.Y2 = pointB.Y;

                    line4.X1 = pointB.X;
                    line4.Y1 = pointB.Y;
                    line4.X2 = point1.X;
                    line4.Y2 = point1.Y;

                    var rectX = point1.X + ((pointA.X - point1.X) / 2) - (lineThickness + adjustment);
                    var rectY = point1.Y + ((pointB.Y - point1.Y) / 2) - (lineThickness + adjustment);

                    SetLeft(topRect, rectX);
                    SetTop(topRect, point1.Y - (lineThickness * 2));

                    SetLeft(bottomRect, rectX);
                    SetTop(bottomRect, point2.Y - (lineThickness * 2));

                    SetLeft(leftRect, point1.X - (lineThickness * 2));
                    SetTop(leftRect, rectY);

                    SetLeft(rightRect, point2.X - (lineThickness * 2));
                    SetTop(rightRect, rectY);

                    lastPosition = position;
                }

                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var line = (Line)sender;
                line.ReleaseMouseCapture();
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }


        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var rect = (Rectangle)sender;
                rect.CaptureMouse();
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void Rect_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var ptrPoint = e.GetPosition(this);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var rect = (Rectangle)sender;
                    var position = ptrPoint;

                    switch (rect.Name)
                    {
                        case "topRect":
                            {
                                point1 = new Point(point1.X, position.Y);
                                pointA = new Point(pointA.X, position.Y);
                                line4.Y2 = line1.Y1 = line1.Y2 = line2.Y1 = position.Y;

                                var rectY = point1.Y + ((pointB.Y - point1.Y) / 2) - (lineThickness + adjustment);

                                SetTop(topRect, position.Y - (lineThickness * 2));
                                SetTop(leftRect, rectY);
                                SetTop(rightRect, rectY);
                            }
                            break;

                        case "bottomRect":
                            {
                                point2 = new Point(point2.X, position.Y);
                                pointB = new Point(pointB.X, position.Y);
                                line4.Y1 = line3.Y2 = line3.Y1 = line2.Y2 = position.Y;

                                var rectY = point1.Y + ((pointB.Y - point1.Y) / 2) - (lineThickness + adjustment);

                                SetTop(bottomRect, position.Y - (lineThickness * 2));
                                SetTop(leftRect, rectY);
                                SetTop(rightRect, rectY);
                            }
                            break;

                        case "leftRect":
                            {
                                point1 = new Point(position.X, point1.Y);
                                pointB = new Point(position.X, pointB.Y);
                                line3.X2 = line4.X1 = line4.X2 = line1.X1 = position.X;

                                var rectX = point1.X + ((pointA.X - point1.X) / 2) - (lineThickness + adjustment);

                                SetLeft(leftRect, position.X - (lineThickness * 2));
                                SetLeft(topRect, rectX);
                                SetLeft(bottomRect, rectX);
                            }
                            break;

                        case "rightRect":
                            {
                                pointA = new Point(position.X, pointA.Y);
                                point2 = new Point(position.X, point2.Y);
                                line1.X2 = line2.X1 = line2.X2 = line3.X1 = position.X;

                                var rectX = point1.X + ((pointA.X - point1.X) / 2) - (lineThickness + adjustment);

                                SetLeft(rightRect, position.X - (lineThickness * 2));
                                SetLeft(topRect, rectX);
                                SetLeft(bottomRect, rectX);
                            }
                            break;

                        default:
                            break;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var rect = (Rectangle)sender;
                rect.ReleaseMouseCapture();
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        public void SetCoordinates(Point point2)
        {
            try
            {
                this.point2 = point2;
                pointA = new Point(point2.X, point1.Y);
                pointB = new Point(point1.X, point2.Y);

                line1.X1 = point1.X + adjustment;
                line1.Y1 = point1.Y + adjustment;
                line1.X2 = pointA.X + adjustment;
                line1.Y2 = pointA.Y + adjustment;

                line2.X1 = pointA.X + adjustment;
                line2.Y1 = pointA.Y + adjustment;
                line2.X2 = point2.X + adjustment;
                line2.Y2 = point2.Y + adjustment;

                line3.X1 = point2.X + adjustment;
                line3.Y1 = point2.Y + adjustment;
                line3.X2 = pointB.X + adjustment;
                line3.Y2 = pointB.Y + adjustment;

                line4.X1 = pointB.X + adjustment;
                line4.Y1 = pointB.Y + adjustment;
                line4.X2 = point1.X + adjustment;
                line4.Y2 = point1.Y + adjustment;

                if ((pointA.X - point1.X) > 16 || (pointA.X - point1.X) < -16)
                {
                    var rectX = point1.X + ((pointA.X - point1.X) / 2) - (lineThickness + adjustment);

                    SetLeft(topRect, rectX);
                    SetTop(topRect, point1.Y - (lineThickness + adjustment));

                    SetLeft(bottomRect, rectX);
                    SetTop(bottomRect, point2.Y - (lineThickness + adjustment));

                    topRect.Visibility = Visibility.Visible;
                    bottomRect.Visibility = Visibility.Visible;
                }
                else
                {
                    topRect.Visibility = Visibility.Collapsed;
                    bottomRect.Visibility = Visibility.Collapsed;
                }

                if ((pointB.Y - point1.Y) > 16 || (pointB.Y - point1.Y) < -16)
                {
                    var rectY = point1.Y + ((pointB.Y - point1.Y) / 2) - (lineThickness + adjustment);

                    SetLeft(leftRect, point1.X - (lineThickness + adjustment));
                    SetTop(leftRect, rectY);

                    SetLeft(rightRect, point2.X - (lineThickness + adjustment));
                    SetTop(rightRect, rectY);

                    leftRect.Visibility = Visibility.Visible;
                    rightRect.Visibility = Visibility.Visible;
                }
                else
                {
                    leftRect.Visibility = Visibility.Collapsed;
                    rightRect.Visibility = Visibility.Collapsed;
                }

            }
            catch (Exception)
            {
            }
        }

        public bool IsZeroSize()
        {
            return point1.X == pointA.X && pointA.X == point2.X && point2.X == pointB.X && pointB.X == point1.X &&
                point1.Y == pointA.Y && pointA.Y == point2.Y && point2.Y == pointB.Y && pointB.Y == point1.Y;
        }
    }
}