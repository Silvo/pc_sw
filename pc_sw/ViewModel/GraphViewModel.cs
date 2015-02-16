using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using pc_sw.Helpers;
using pc_sw.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace pc_sw.ViewModel
{
    public class GraphViewModel : ViewModelBase
    {
        private IDataStorage _model;
        private Dictionary<byte, GraphData> _sensorIdMap;
        private GraphData _selectedSource;

        public const string SelectedSourcePropertyName = "SelectedSource";
        
        public ObservableCollection<GraphData> Data { get; set; }
        public GraphData SelectedSource
        {
            get
            {
                return _selectedSource;
            }

            set
            {
                if (_selectedSource == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSourcePropertyName);
                _selectedSource = value;
                RaisePropertyChanged(SelectedSourcePropertyName);
            }
        }

        public GraphViewModel(IDataStorage model)
        {
            _model = model;
            _sensorIdMap = new Dictionary<byte, GraphData>();

            this.Data = new ObservableCollection<GraphData>();
            
            IdToColorBrushConverter colorConverter = new IdToColorBrushConverter();

            foreach (var source in _model.GetDataSources())
            {
                System.Drawing.SolidBrush brush =
                    (System.Drawing.SolidBrush)colorConverter.Convert(
                        source.SensorId, typeof(System.Drawing.SolidBrush), new object(),
                        CultureInfo.CurrentCulture);

                System.Drawing.Pen pen = new System.Drawing.Pen(brush, 3f);
                pen.MiterLimit = 1;
                
                source.Samples.Sort(); // SIC

                _sensorIdMap.Add(source.SensorId, new GraphData(pen, source.Samples));
                source.SampleAdded += source_SampleAdded;
            }

            Messenger.Default.Register<SensorVisibilityChangedMessage>(
                this, message => { UpdateGraphVisibility(message.SensorId,
                    message.IsVisible); });

            Messenger.Default.Register<SelectSensorMessage>(
                this, message => { this.SelectedSource = this._sensorIdMap[message.SensorId]; });

            Messenger.Default.Register<SensorInfoMessage>(
                this, message =>
                {
                    this.SetWarningLevels(message.Id, message.LowWarningLevel,
                        message.HighWarningLevel);
                });

            _model.SourceAdded += _model_SourceAdded;
        }

        void SetWarningLevels(Byte sensorId, Nullable<Double> lowLevel, Nullable<Double> highLevel)
        {
            GraphData gd = _sensorIdMap[sensorId];

            if (lowLevel.HasValue)
            {
                gd.LowWarningLevel = (Single)lowLevel.Value;
            }
            if (highLevel.HasValue)
            {
                gd.HighWarningLevel = (Single)highLevel.Value;
            }
        }

        void _model_SourceAdded(object sender, EventArgs e)
        {
            IdToColorBrushConverter colorConverter = new IdToColorBrushConverter();
            DataSource source = ((SourceAddedEventArgs)e).Source;

            System.Drawing.SolidBrush brush =
                (System.Drawing.SolidBrush)colorConverter.Convert(
                    source.SensorId, typeof(System.Drawing.SolidBrush), new object(),
                    CultureInfo.CurrentCulture);

            System.Drawing.Pen pen = new System.Drawing.Pen(brush, 3f);
            pen.MiterLimit = 1;

            source.Samples.Sort(); // SIC

            _sensorIdMap.Add(source.SensorId, new GraphData(pen, source.Samples));

            source.SampleAdded += source_SampleAdded;
        }

        private void source_SampleAdded(object sender, SampleAddedEventArgs e)
        {
            byte id = ((DataSource)sender).SensorId;
            GraphData gd = _sensorIdMap[id];
            
            if (e.Sample.Item1 >= gd.Points[gd.Points.Count-1].Item1)
            {
                gd.Points.Add(e.Sample);
            }
            else
            {
                ((DataSource)sender).Samples.Sort();
                
                bool visible = false;
                if (this.Data.Contains(gd))
                {
                    this.Data.Remove(gd);
                    visible = true;
                }
                
                System.Drawing.Pen pen = gd.DrawingPen;
                _sensorIdMap.Remove(id);
                _sensorIdMap.Add(id,new GraphData(pen,((DataSource)sender).Samples));

                if (visible)
                {
                    this.Data.Add(_sensorIdMap[id]);
                }
            }
        }

        private void UpdateGraphVisibility(byte sensorId, bool isVisible)
        {
            GraphData gd = _sensorIdMap[sensorId];

            if (isVisible && !this.Data.Contains(gd))
            {
                this.Data.Add(gd);
            }
            else if (!isVisible && this.Data.Contains(gd))
            {
                this.Data.Remove(gd);
            }
        }
    }
}