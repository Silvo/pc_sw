using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using pc_sw.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using pc_sw.Helpers;

namespace pc_sw
{
    public class TimeGraph : Panel
    {
        private enum SelectedElement
        {
            Nothing,
            StartMarker,
            EndMarker,
            ZoomWindow,
            MainWindow
        }
        private SelectedElement _selection;
        private Double _oldX;

        private WriteableBitmap _bmp;

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(ObservableCollection<GraphData>),
            typeof(TimeGraph), new PropertyMetadata(OnDataChanged));
        public static readonly DependencyProperty SelectedSourceProperty =
            DependencyProperty.Register("SelectedSource", typeof(GraphData),
            typeof(TimeGraph), new PropertyMetadata(OnSelectedSourceChanged));
        public static readonly DependencyProperty SkipProperty =
            DependencyProperty.Register("Skip", typeof(int),
            typeof(TimeGraph), new PropertyMetadata(1));
        public static readonly DependencyProperty ShowDataCursorProperty =
            DependencyProperty.Register("ShowDataCursor", typeof(bool),
            typeof(TimeGraph), new PropertyMetadata(false));
        public static readonly DependencyProperty ZoomStartHandlePositionProperty =
            DependencyProperty.Register("ZoomStartHandlePosition", typeof(Double),
            typeof(TimeGraph), new FrameworkPropertyMetadata(10.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ZoomEndHandlePositionProperty =
            DependencyProperty.Register("ZoomEndHandlePosition", typeof(Double),
            typeof(TimeGraph), new FrameworkPropertyMetadata(300.0,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsRender));

        private Tuple<DateTime, float, System.Drawing.Pen> _nearestDataPoint;
        private DateTime _startTime, _endTime;
        private DateTime _mainStartTime, _mainEndTime;
        private Single _zoomGraphHeight = 75f;
        private Single _mainGraphHeight;
        private Single _graphPadding = 25f;
        private ZoomGraphHandle _zoomStartHandle, _zoomEndHandle;
        private Rectangle _zoomStartMask, _zoomEndMask, _zoomWindowMask;
        private Cursor _closedHandCursor, _openHandCursor;
        private bool _zoomWindowClicked = false;
        private List<Tuple<DateTime, string>> _markers;
        private Double _zoomScale = 1.0;

        public int Skip
        {
            get { return (int)GetValue(SkipProperty); }
            set { SetValue(SkipProperty, value); }
        }

        public bool ShowDataCursor
        {
            get { return (bool)GetValue(ShowDataCursorProperty); }
            set { SetValue(ShowDataCursorProperty, value); }
        }

        public GraphData SelectedSource
        {
            get { return (GraphData)GetValue(SelectedSourceProperty); }
            set { SetValue(SelectedSourceProperty, value); }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            private set
            {
                _startTime = value;
                ZoomStartHandlePosition = GetOffsetFromDate(_mainStartTime) * this.ActualWidth;
            }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            private set
            {
                _endTime = value;
                ZoomEndHandlePosition = GetOffsetFromDate(_mainEndTime) * this.ActualWidth;
            }
        }

        public Double ZoomStartHandlePosition
        {
            get { return (Double)GetValue(ZoomStartHandlePositionProperty); }
            set
            {
                if (this.ActualWidth > 0)
                {
                    Double newPosition = Math.Min(value, ZoomEndHandlePosition - 3);
                
                    Double offset = newPosition / this.ActualWidth;
                    _zoomStartHandle.LabelContent = GetDateStringFromOffset(offset);
                    _mainStartTime = GetDateFromOffset(offset);
                    _zoomScale = (_mainEndTime.Ticks - _mainStartTime.Ticks) / this.ActualWidth;
                    UpdateMarkerCollection();
                    SetValue(ZoomStartHandlePositionProperty, newPosition);
                }
            }
        }

        public Double ZoomEndHandlePosition
        {
            get { return (Double)GetValue(ZoomEndHandlePositionProperty); }
            set
            {
                if (this.ActualWidth > 0)
                {
                    Double newPosition = Math.Max(value, ZoomStartHandlePosition + 3);

                    Double offset = newPosition / this.ActualWidth;
                    _zoomEndHandle.LabelContent = GetDateStringFromOffset(offset);
                    _mainEndTime = GetDateFromOffset(offset);
                    _zoomScale = (_mainEndTime.Ticks - _mainStartTime.Ticks) / this.ActualWidth;
                    UpdateMarkerCollection();
                    SetValue(ZoomEndHandlePositionProperty, newPosition);
                }
            }
        }


        private static void OnSelectedSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeGraph graph = (TimeGraph)d;

            graph._nearestDataPoint = null;
            graph.InvalidateVisual();
        }

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeGraph graph = (TimeGraph)d;

            if (graph.Data != null)
            {
                graph.Data.CollectionChanged -= graph.Data_CollectionChanged;
            }
            
            graph.Data.CollectionChanged += graph.Data_CollectionChanged;

            graph.InvalidateVisual();
        }

        public ObservableCollection<GraphData> Data
        {
            get { return (ObservableCollection<GraphData>)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public TimeGraph()
        {
            _bmp = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Bgr24, null);

            this.SizeChanged += ZoomBar_SizeChanged;
            this.Loaded += ZoomBar_Loaded;

            _mainStartTime = new DateTime(0);
            _mainEndTime = DateTime.Now;
            _startTime = DateTime.Now - TimeSpan.FromSeconds(1);
            _endTime = DateTime.Now;

            _openHandCursor = new Cursor(new MemoryStream(pc_sw.Properties.Resources.openhand));
            _closedHandCursor = new Cursor(new MemoryStream(pc_sw.Properties.Resources.closedhand));

            _zoomStartMask = new Rectangle();
            _zoomStartMask.Height = 100;
            _zoomStartMask.Width = 100;
            _zoomStartMask.Fill = Brushes.White;
            SetZIndex(_zoomStartMask, 1);
            _zoomStartMask.Opacity = 0.75;

            _zoomEndMask = new Rectangle();
            _zoomEndMask.Height = 100;
            _zoomEndMask.Width = 100;
            _zoomEndMask.Fill = Brushes.White;
            SetZIndex(_zoomEndMask, 1);
            _zoomEndMask.Opacity = 0.75;

            _zoomWindowMask = new Rectangle();
            _zoomWindowMask.Height = 100;
            _zoomWindowMask.Width = 100;
            _zoomWindowMask.Fill = Brushes.White;
            SetZIndex(_zoomWindowMask, 1);
            _zoomWindowMask.Opacity = 0.00;

            _zoomStartHandle = new ZoomGraphHandle();
            _zoomStartHandle.Height = 100;
            _zoomStartHandle.LabelContent = "start";
            SetZIndex(_zoomStartHandle, 2);

            _zoomEndHandle = new ZoomGraphHandle();
            _zoomEndHandle.Height = 100;
            _zoomEndHandle.LabelContent = "end";
            SetZIndex(_zoomEndHandle, 2);

            _zoomWindowMask.MouseLeftButtonDown += ZoomWindowClicked;
            _zoomWindowMask.MouseMove += _zoomWindowMask_MouseMove;
            _zoomStartHandle.MouseLeftButtonDown += GrabStartMarker;
            _zoomStartHandle.MouseEnter += _zoomHandle_MouseEnter;
            _zoomEndHandle.MouseLeftButtonDown += GrabEndMarker;
            _zoomEndHandle.MouseEnter += _zoomHandle_MouseEnter;

            this.MouseMove += TimeGraph_MouseMove;
            this.MouseLeftButtonDown += TimeGraph_MouseLeftButtonDown;
            this.MouseLeftButtonUp += ReleaseMarkers;
            this.MouseLeave += ReleaseMarkers;
            this.MouseWheel += TimeGraph_MouseWheel;

            this.Children.Add(_zoomStartHandle);
            this.Children.Add(_zoomEndHandle);
            this.Children.Add(_zoomStartMask);
            this.Children.Add(_zoomEndMask);
            this.Children.Add(_zoomWindowMask);

            SetZoomControlVisibility(Visibility.Hidden);
        }

        void _zoomHandle_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void SetZoomControlVisibility(Visibility visibility)
        {
            foreach (UIElement child in this.Children)
            {
                child.Visibility = visibility;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Double tempWidth;
            
            _zoomStartMask.Width = ZoomStartHandlePosition;
            _zoomStartMask.Arrange(new Rect(0, _mainGraphHeight, ZoomStartHandlePosition, 100));

            tempWidth = Math.Max(this.ActualWidth - ZoomEndHandlePosition, 1);
            _zoomEndMask.Width = tempWidth;
            _zoomEndMask.Arrange(new Rect(ZoomEndHandlePosition, _mainGraphHeight, tempWidth, 100));

            tempWidth = Math.Max(ZoomEndHandlePosition - ZoomStartHandlePosition, 1);
            _zoomWindowMask.Width = tempWidth;
            _zoomWindowMask.Arrange(new Rect(ZoomStartHandlePosition, _mainGraphHeight, tempWidth, 100));

            _zoomStartHandle.Arrange(new Rect(ZoomStartHandlePosition, _mainGraphHeight, 50, 100));
            _zoomEndHandle.Arrange(new Rect(ZoomEndHandlePosition, _mainGraphHeight, 50, 100));

            return finalSize;
        }

        void TimeGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Data != null && this.Data.Count != 0)
            {
                if (e.GetPosition((UIElement)sender).Y > _mainGraphHeight)
                {
                    Double newWindowStart, newWindowEnd;
                    Double newX = e.GetPosition((FrameworkElement)sender).X;

                    switch (_selection)
                    {
                        case SelectedElement.StartMarker:
                            newWindowStart = e.GetPosition((FrameworkElement)sender).X;
                            if (newWindowStart >= 0.0)
                            {
                                ZoomStartHandlePosition = newWindowStart;
                            }
                            break;
                        case SelectedElement.EndMarker:
                            newWindowEnd = e.GetPosition((FrameworkElement)sender).X;
                            if (newWindowEnd <= ((FrameworkElement)sender).ActualWidth)
                            {
                                ZoomEndHandlePosition = newWindowEnd;
                            }
                            break;
                        case SelectedElement.ZoomWindow:
                            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                            {
                                newWindowStart = ZoomStartHandlePosition - (newX - _oldX);
                                newWindowEnd = ZoomEndHandlePosition + (newX - _oldX);
                            }
                            else
                            {
                                newWindowStart = ZoomStartHandlePosition + (newX - _oldX);
                                newWindowEnd = ZoomEndHandlePosition + (newX - _oldX);
                            }

                            if (newWindowStart >= 0.0 && newWindowEnd <= ((FrameworkElement)sender).ActualWidth)
                            {
                                ZoomStartHandlePosition = newWindowStart;
                                ZoomEndHandlePosition = newWindowEnd;
                            }
                            _oldX = newX;
                            break;
                        case SelectedElement.Nothing:
                            break;
                        default:
                            break;
                    }
                    e.Handled = true;
                }
                else if (this.ShowDataCursor == true)
                {
                    System.Windows.Point point = e.GetPosition((UIElement)sender);

                    if (_selection == SelectedElement.MainWindow)
                    {
                        DateTime mainStartTime;
                        DateTime mainEndTime;

                        if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                        {
                            mainStartTime = _mainStartTime.AddTicks((long)((_oldX - point.X) * _zoomScale));
                            mainEndTime = _mainEndTime.AddTicks((long)-((_oldX - point.X) * _zoomScale));
                        }
                        else
                        {
                            mainStartTime = _mainStartTime.AddTicks((long)((_oldX - point.X) * _zoomScale));
                            mainEndTime = _mainEndTime.AddTicks((long)((_oldX - point.X) * _zoomScale));
                        }

                        if (mainStartTime >= StartTime && mainEndTime <= EndTime)
                        {
                            _mainStartTime = mainStartTime;
                            _mainEndTime = mainEndTime;
                        }
                    }

                    _oldX = point.X;

                    if (this.Data.Contains(SelectedSource))
                    {
                        _nearestDataPoint = GetNearestDataPoint(point);
                    }

                    this.InvalidateVisual();
                }
            }
        }

        private Tuple<DateTime, float, System.Drawing.Pen> GetNearestDataPoint(Point point)
        {
            float graphWidth = Math.Max((float)_bmp.PixelWidth, 1);
            float relativeX = (float)point.X / graphWidth;

            long timeTicks = (long)(relativeX * (_mainEndTime.Ticks - _mainStartTime.Ticks)) + _mainStartTime.Ticks;

            Tuple<DateTime, float> dataPoint = this.SelectedSource.Points.OrderBy(n => Math.Abs(n.Item1.Ticks - timeTicks)).First();

            return new Tuple<DateTime, float, System.Drawing.Pen>(dataPoint.Item1, dataPoint.Item2, this.SelectedSource.DrawingPen);
        }

        void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GraphData dataSource in e.NewItems)
                {
                    dataSource.Points.CollectionChanged += dataSource_CollectionChanged;
                }
                
                SetZoomControlVisibility(Visibility.Visible);
            }

            if (e.OldItems != null)
            {
                foreach (GraphData dataSource in e.OldItems)
                {
                    dataSource.Points.CollectionChanged -= dataSource_CollectionChanged;
                }

                if (this.Data.Count == 0)
                {
                    SetZoomControlVisibility(Visibility.Hidden);
                }
            }
            if (this.Data != null && this.Data.Count != 0)
            {
                this.Cursor = Cursors.Arrow;
            }

            this.InvalidateVisual();
        }

        void dataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        void ZoomBar_Loaded(Object sender, RoutedEventArgs e)
        {
            _bmp = new WriteableBitmap((Int32)this.ActualWidth, (Int32)this.ActualHeight, 96, 96, PixelFormats.Bgr24, null);
            _mainGraphHeight = Math.Max((Single)_bmp.PixelHeight - _zoomGraphHeight - _graphPadding, 1f);
            this.InvalidateVisual();
        }

        void ZoomBar_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            _bmp = null;
            Int32 _bmp_height = Math.Max((Int32)this.ActualHeight, 1);
            Int32 _bmp_width = Math.Max((Int32)this.ActualWidth, 1);
            GC.Collect();
            _bmp = new WriteableBitmap(_bmp_width, _bmp_height, 96, 96, PixelFormats.Bgr24, null);
            _mainGraphHeight = Math.Max((Single)_bmp.PixelHeight - _zoomGraphHeight - _graphPadding, 1f);
            this.InvalidateVisual();
        }
        
        protected override void OnRender(DrawingContext dc)
        {
            UInt32 heightScaler = 255;
            Single graphWidth = Math.Max((Single)_bmp.PixelWidth, 1f);

            _bmp.Lock();
            System.Drawing.Bitmap backbuffer =
                new System.Drawing.Bitmap(
                    _bmp.PixelWidth,
                    _bmp.PixelHeight,
                    _bmp.BackBufferStride,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                    _bmp.BackBuffer);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(backbuffer))
            {
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                g.Clear(System.Drawing.Color.White);

                var data = this.Data;
                 
                if (data != null && data.Count != 0)
                {
                    DateTime startTime = DateTime.Now - TimeSpan.FromSeconds(1);
                    DateTime endTime = new DateTime(0);
                    
                    int maxCount = 0;

                    foreach (var source in data)
                    {
                        maxCount = Math.Max(maxCount, source.Points.Count);

                        if (source.Points.Count > 0)
                        {
                            if (source.Points[0].Item1 < startTime)
                            {
                                startTime = source.Points[0].Item1;
                            }

                            if (source.Points[source.Points.Count - 1].Item1 > endTime)
                            {
                                endTime = source.Points[source.Points.Count - 1].Item1;
                            }
                        }
                    }
                    _mainStartTime = _mainStartTime < startTime ? startTime : _mainStartTime;
                    _mainEndTime = _mainEndTime > endTime ? endTime : _mainEndTime;
                    StartTime = startTime;
                    EndTime = endTime;

                    Single mainMiddleYPos = _mainGraphHeight / 2f;
                    System.Drawing.Font font = new System.Drawing.Font("Segoe UI Semilight", 10);
                    System.Drawing.Pen markerPen = System.Drawing.Pens.Black;

                    // Draw markers

                    Single markerHeight;

                    foreach (var marker in _markers)
                    {
                        markerHeight = 4;
                        Single xPos = graphWidth * (float)((marker.Item1 - _mainStartTime).TotalSeconds /
                            (_mainEndTime - _mainStartTime).TotalSeconds);
                        
                        if (marker.Item2 != "")
                        {
                            System.Drawing.SizeF stringSize = g.MeasureString(marker.Item2, font);
                            
                            System.Drawing.PointF pt =
                                new System.Drawing.PointF(xPos - (stringSize.Width / 2), mainMiddleYPos + markerHeight + 3);
                            g.DrawString(marker.Item2, font, System.Drawing.Brushes.Black, pt);
                            markerHeight = 8;
                        }

                        System.Drawing.PointF p0 =
                        new System.Drawing.PointF(xPos, mainMiddleYPos + markerHeight);
                        System.Drawing.PointF p1 =
                            new System.Drawing.PointF(xPos, mainMiddleYPos - markerHeight);
                        g.DrawLine(markerPen, p0, p1);
                    }

                    // Draw main graph

                    foreach (var dataSource in data)
                    {
                        var skip = this.Skip;
                        int z = 0, totalPoints = ((dataSource.Points.Count - z) + (skip - 1)) / skip;
                        while (dataSource.Points[z].Item1 < _mainStartTime) ++z;
                        if (z > 0) --z;
                        System.Drawing.PointF[] points = new System.Drawing.PointF[totalPoints];

                        for (int j = z, k = 0; j < dataSource.Points.Count; j += skip, ++k)
                        {
                            var point = dataSource.Points[j];
                            points[k].X = graphWidth * (float)((point.Item1 - _mainStartTime).TotalSeconds /
                                                                (_mainEndTime - _mainStartTime).TotalSeconds);
                            points[k].Y = mainMiddleYPos - _mainGraphHeight * point.Item2 / heightScaler;
                            
                            if (point.Item1 > _mainEndTime && k < totalPoints - 1)
                            {
                                Array.Resize(ref points, k+1);
                                break;
                            }
                        }

                        if (points.Length > 1)
                        {
                            g.DrawLines(dataSource.DrawingPen, points);
                        }
                        else if (points.Length == 1)
                        {
                            g.DrawRectangle(dataSource.DrawingPen, points[0].X, points[0].Y, 2f, 2f);
                        }
                    }

                    // Draw warning levels

                    if (this.Data.Contains(this.SelectedSource))
                    {
                        if (this.SelectedSource.HighWarningLevel != MagicNumbers.HIGH_WARNING_OFF_SINGLE)
                        {
                            float highY = mainMiddleYPos - _mainGraphHeight * this.SelectedSource.HighWarningLevel / heightScaler;
                            System.Drawing.PointF hP1 = new System.Drawing.PointF(0, highY);
                            System.Drawing.PointF hP2 = new System.Drawing.PointF(graphWidth, highY);
                            g.DrawLine(markerPen, hP1, hP2);
                        }

                        if (this.SelectedSource.LowWarningLevel != MagicNumbers.LOW_WARNING_OFF_SINGLE)
                        {
                            float lowY = mainMiddleYPos - _mainGraphHeight * this.SelectedSource.LowWarningLevel / heightScaler;
                            System.Drawing.PointF lP1 = new System.Drawing.PointF(0, lowY);
                            System.Drawing.PointF lP2 = new System.Drawing.PointF(graphWidth, lowY);
                            g.DrawLine(markerPen, lP1, lP2);
                        }
                    }

                    // Draw data cursor

                    if (_nearestDataPoint != null && this.Data.Contains(this.SelectedSource))
                    {
                        float pointX = graphWidth * (float)((_nearestDataPoint.Item1 - _mainStartTime).TotalSeconds /
                                                                (_mainEndTime - _mainStartTime).TotalSeconds);
                        float pointY = mainMiddleYPos - _mainGraphHeight * _nearestDataPoint.Item2 / heightScaler;

                        g.FillEllipse(System.Drawing.Brushes.Black, pointX-2.5f, pointY-2.5f, 5f, 5f);
                        String string1 = _nearestDataPoint.Item1.ToString();
                        String string2 = _nearestDataPoint.Item2.ToString();

                        System.Drawing.SizeF size1 = g.MeasureString(string1, font);
                        System.Drawing.SizeF size2 = g.MeasureString(string2, font);
                        Single maxWidth = Math.Max(size1.Width, size2.Width);

                        System.Drawing.PointF p1 = new System.Drawing.PointF(pointX - (size1.Width / 2), pointY - size1.Height - size2.Height - 3);
                        System.Drawing.PointF p2 = new System.Drawing.PointF(pointX - (size2.Width / 2), pointY - size2.Height - 3);
                        System.Drawing.RectangleF rect1 = new System.Drawing.RectangleF(pointX - 53f, pointY - 34f, 106f, 15f);
                        System.Drawing.RectangleF rect2 = new System.Drawing.RectangleF(pointX - 18f, pointY - 22f, 36f, 15f);
                        
                        System.Drawing.SolidBrush bgBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(unchecked((int)0x88FFFFFF)));

                        g.FillRectangle(bgBrush,
                            pointX - (maxWidth / 2), pointY - size1.Height - size2.Height - 5,
                            maxWidth, size1.Height + size2.Height + 3);
                        g.DrawString(string1, font, System.Drawing.Brushes.Black, p1);
                        g.DrawString(string2, font, System.Drawing.Brushes.Black, p2);
                    }

                    // Draw zoom graph

                    Single zoomGraphHeight = _zoomGraphHeight;
                    Single zoomMiddleYPos = _mainGraphHeight + _graphPadding + (zoomGraphHeight / 2f);

                    foreach (var dataSource in data)
                    {
                        int skip = Math.Max(dataSource.Points.Count / 1000, 1);
                        int count = (int)Math.Ceiling((Double)dataSource.Points.Count / skip);

                        System.Drawing.PointF[] points = new System.Drawing.PointF[count];

                        for (int j = 0, i = 0; j < dataSource.Points.Count; j += skip, i++)
                        {
                            var point = dataSource.Points[j];
                            points[i].X = graphWidth * (float)((point.Item1 - startTime).TotalSeconds /
                                                                (endTime - startTime).TotalSeconds);
                            points[i].Y = zoomMiddleYPos - zoomGraphHeight * point.Item2 / heightScaler;
                        }

                        if (points.Length > 1)
                        {
                            g.DrawLines(dataSource.DrawingPen, points);
                        }
                        else if (points.Length == 1)
                        {
                            g.DrawRectangle(dataSource.DrawingPen, points[0].X, points[0].Y, 2f, 2f);
                        }
                    }

                }
                else
                {
                    System.Drawing.Font font = new System.Drawing.Font("Segoe UI Semilight", 24);
                    String text = "All datasources disabled";
                    System.Drawing.SizeF textSize = g.MeasureString(text, font);
                    System.Drawing.PointF point = new System.Drawing.PointF(
                        (graphWidth / 2) - (textSize.Width / 2),
                        ((Single)_bmp.PixelHeight / 2f) - (textSize.Height / 2));
                    //System.Drawing.SizeF size = new System.Drawing.SizeF(370f, 50f);
                    //System.Drawing.RectangleF rect = new System.Drawing.RectangleF(point, size);

                    g.DrawString(text, font, System.Drawing.Brushes.Black, point);
                }

                base.OnRender(dc);
            }
            _bmp.AddDirtyRect(new Int32Rect(0, 0, _bmp.PixelWidth, _bmp.PixelHeight));
            _bmp.Unlock();

            dc.DrawImage(_bmp, new Rect(this.RenderSize));
        }

        private DateTime GetDateFromOffset(double offset)
        {
            offset = Double.IsNaN(offset) ? 0.0 : offset;
            return StartTime.AddSeconds((EndTime - StartTime).TotalSeconds * offset);
        }

        private String GetDateStringFromOffset(double offset)
        {
            offset = Double.IsNaN(offset) ? 0.0 : offset;
            DateTime temp = GetDateFromOffset(offset);

            return String.Format("{0}.{1}", temp.Day, temp.Month);
        }

        private Double GetOffsetFromDate(DateTime date)
        {
            if (date.Ticks < StartTime.Ticks)
            {
                return 0.0;
            }
            else
            {
                return (Double)(date.Ticks - StartTime.Ticks) / (Double)(EndTime.Ticks - StartTime.Ticks);
            }
        }

        private void GrabStartMarker(object sender, MouseButtonEventArgs e)
        {
            _selection = SelectedElement.StartMarker;
            e.Handled = true;
        }

        private void GrabEndMarker(object sender, MouseButtonEventArgs e)
        {
            _selection = SelectedElement.EndMarker;
            e.Handled = true;
        }

        private void ZoomWindowClicked(object sender, MouseEventArgs e)
        {
            _zoomWindowClicked = true;
        }

        private void ClickMask(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ReleaseMarkers(object sender, MouseEventArgs e)
        {
            if (this.Data != null && this.Data.Count != 0)
            {
                _selection = SelectedElement.Nothing;
                this.Cursor = _openHandCursor;
                e.Handled = true;
            }
        }

        private void TimeGraph_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Data != null && this.Data.Count != 0)
            {
                _oldX = e.GetPosition((Panel)sender).X;

                if (_zoomWindowClicked == true)
                {
                    _selection = SelectedElement.ZoomWindow;
                    this.Cursor = _closedHandCursor;
                }
                else
                {
                    _selection = SelectedElement.MainWindow;
                    this.Cursor = _closedHandCursor;
                }

                _zoomWindowClicked = false;
                e.Handled = true;
            }
        }

        private void TimeGraph_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.Data != null && this.Data.Count != 0)
            {
                Double temp = e.Delta / 70.0;
                Double scaledTemp = temp * (1 + 8 * (ZoomEndHandlePosition - ZoomStartHandlePosition) / ((FrameworkElement)sender).ActualWidth);

                Double newWindowStart = ZoomStartHandlePosition + scaledTemp;
                Double newWindowEnd = ZoomEndHandlePosition - scaledTemp;
                if (newWindowStart >= 0.0 && newWindowEnd <= ((FrameworkElement)sender).ActualWidth)
                {
                    ZoomStartHandlePosition = newWindowStart;
                    ZoomEndHandlePosition = newWindowEnd;
                }

                e.Handled = true;
            }
        }

        private void _zoomWindowMask_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Data != null && this.Data.Count != 0)
            {
                if (_selection == SelectedElement.ZoomWindow)
                {
                    this.Cursor = _closedHandCursor;
                }
                else
                {
                    this.Cursor = _openHandCursor;
                }
            }
        }

        private void UpdateMarkerCollection()
        {
            _markers = new List<Tuple<DateTime, string>>();
            int mainTimeSpan = (int)(_mainEndTime - _mainStartTime).TotalHours;

            if (mainTimeSpan < 3)
            {
                DateTime date = new DateTime(_mainStartTime.Year, _mainStartTime.Month, _mainStartTime.Day, _mainStartTime.Hour, 0, 0);

                while (date < _mainEndTime)
                {
                    if (date.Hour == 0 && date.Minute == 0)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}.{1}", date.Day, date.Month)));
                    }
                    else if (date.Minute % 15 == 0)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}:{1:00}", date.Hour, date.Minute)));
                    }
                    else
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, ""));
                    }
                    date = date.AddMinutes(5);
                }
            }
            else if (mainTimeSpan < 8)
            {
                DateTime date = new DateTime(_mainStartTime.Year, _mainStartTime.Month, _mainStartTime.Day, _mainStartTime.Hour, 0, 0);

                while (date < _mainEndTime)
                {
                    if (date.Hour == 0 && date.Minute == 0)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}.{1}", date.Day, date.Month)));
                    }
                    else if (date.Minute == 0)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}:{1:00}", date.Hour, date.Minute)));
                    }
                    else
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, ""));
                    }
                    date = date.AddMinutes(15);
                }
            }
            else if (mainTimeSpan < 48)
            {
                DateTime date = new DateTime(_mainStartTime.Year, _mainStartTime.Month, _mainStartTime.Day);

                while (date < _mainEndTime)
                {
                    if (date.Hour == 0)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}.{1}", date.Day, date.Month)));
                    }
                    else if (date.Hour % 6 == 0)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}:{1:00}", date.Hour, date.Minute)));
                    }
                    else
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, ""));
                    }

                    date = date.AddHours(1);
                }
            }
            else if (mainTimeSpan < 24 * 7)
            {
                DateTime date = new DateTime(_mainStartTime.Year, _mainStartTime.Month, _mainStartTime.Day);

                while (date < _mainEndTime)
                {
                    if (date.Hour == 0)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}.{1}", date.Day, date.Month)));
                    }
                    else
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, ""));
                    }

                    date = date.AddHours(6);
                }
            }
            else if (mainTimeSpan < 24 * 60)
            {
                DateTime date = new DateTime(_mainStartTime.Year, _mainStartTime.Month, _mainStartTime.Day);

                while (date < _mainEndTime)
                {
                    _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}.{1}", date.Day, date.Month)));
                    date = date.AddDays(1);
                }
            }
            else if (mainTimeSpan < 24 * 90)
            {
                DateTime date = new DateTime(_mainStartTime.Year, _mainStartTime.Month, 1);

                while (date < _mainEndTime)
                {
                    if (date.DayOfWeek == DayOfWeek.Monday)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, String.Format("{0}.{1}", date.Day, date.Month)));
                    }
                    date = date.AddDays(1);
                }
            }
            else if (mainTimeSpan < 24 * 365)
            {
                DateTime date = new DateTime(_mainStartTime.Year, 1, 1);

                while (date < _mainEndTime)
                {
                    _markers.Add(new Tuple<DateTime, string>(date, date.ToString("MMMM")));
                    date = date.AddMonths(1);
                }
            }
            else if (mainTimeSpan < 24 * 365 * 1.5)
            {
                DateTime date = new DateTime(_mainStartTime.Year, _mainStartTime.Month, 1);

                while (date < _mainEndTime)
                {
                    if (date.Month == 1)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, date.Year.ToString()));
                    }
                    else if (date.Month % 3 == 1)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, date.ToString("MMMM")));
                    }
                    else
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, ""));
                    }

                    date = date.AddMonths(1);
                }
            }
            else if (mainTimeSpan < 24 * 365 * 6)
            {
                DateTime date = new DateTime(_mainStartTime.Year, 1, 1);

                while (date < _mainEndTime)
                {
                    if (date.Month == 1)
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, date.Year.ToString()));
                    }
                    else
                    {
                        _markers.Add(new Tuple<DateTime, string>(date, ""));
                    }

                    date = date.AddMonths(3);
                }
            }
            else
            {
                DateTime date = new DateTime(_mainStartTime.Year, 1, 1);

                while (date < _mainEndTime)
                {
                    _markers.Add(new Tuple<DateTime, string>(date, date.Year.ToString()));

                    date = date.AddYears(1);
                }
            }
        }
    }
}
