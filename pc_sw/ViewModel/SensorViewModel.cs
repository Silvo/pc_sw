using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using pc_sw.Helpers;
using pc_sw.Model;
using System;
using System.Windows.Media;

namespace pc_sw.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SensorViewModel : ViewModelBase
    {
        private DataSource _model;
        private string _average = "0.0";
        private string _unit;
        public const string IsVisiblePropertyName = "IsVisible";
        public const string AveragePropertyName = "Average";

        public string Name { get; set; }
        public byte Id { get; set; }

        private bool _isVisible = false;
        private ObservableQueue<Tuple<DateTime, float>> _mostRecentSamples;

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IsVisiblePropertyName);
                _isVisible = value;
                RaisePropertyChanged(IsVisiblePropertyName);
                Messenger.Default.Send<SensorVisibilityChangedMessage>(
                    new SensorVisibilityChangedMessage(this.Id, value));
            }
        }

        public string Average
        {
            get
            {
                return _average;
            }

            set
            {
                if (_average == value)
                {
                    return;
                }

                RaisePropertyChanging(AveragePropertyName);
                _average = value;
                RaisePropertyChanged(AveragePropertyName);
            }
        }

        public SensorViewModel(DataSource model)
        {
            _model = model;
            _mostRecentSamples = new ObservableQueue<Tuple<DateTime, float>>();

            switch (_model.SensorType)
            {
                case SensorType.TemperatureSensor:
                    this.Name = "Temperature " + _model.SensorId.ToString();
                    this._unit = " °C";
                    break;
                case SensorType.AirHumiditySensor:
                    this.Name = "Humidity " + _model.SensorId.ToString();
                    this._unit = " %";
                    break;
                case SensorType.SoilMoistureSensor:
                    this.Name = "Soil moisture " + _model.SensorId.ToString();
                    this._unit = "";
                    break;
                default:
                    this.Name = "Unknown " + _model.SensorId.ToString();
                    this._unit = "";
                    break;
            }

            this.Id = _model.SensorId;

            foreach (var sample in _model.Samples)
            {
                if (sample.Item1 > DateTime.Now.Subtract(TimeSpan.FromHours(24)))
                {
                    _mostRecentSamples.Enqueue(sample);
                }
            }

            UpdateAverage();

            _model.SampleAdded += _model_SampleAdded;
        }

        void _model_SampleAdded(object sender, SampleAddedEventArgs e)
        {
            _mostRecentSamples.Enqueue(e.Sample);
            UpdateAverage();
        }

        private void UpdateAverage()
        {
            if (_mostRecentSamples.Count == 0)
            {
                this.Average = "0.0" + _unit;
            }
            else
            {
                /* Remove all samples that are older than 24 hours from the queue */
                while (_mostRecentSamples.PeekFirst().Item1 <
                    DateTime.Now.Subtract(TimeSpan.FromHours(24)))
                {
                    _mostRecentSamples.Dequeue();
                }
                Int32 count = _mostRecentSamples.Count;

                if (count > 0)
                {
                    float sum = 0;
                    foreach (var sample in _mostRecentSamples)
                    {
                        sum += sample.Item2;
                    }

                    this.Average = string.Format("{0:#0.0}{1}", sum / count, _unit);
                }
                else
                {
                    this.Average = "0.0" + _unit;
                }
            }
        }

    }
}