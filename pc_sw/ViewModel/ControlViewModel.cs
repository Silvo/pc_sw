using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using pc_sw.Helpers;
using pc_sw.Model;
using System;

namespace pc_sw.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ControlViewModel : ViewModelBase
    {
        private IDeviceControl _control;

        public const String IsSensorHighWarningUpToDatePropertyName = "IsSensorHighWarningUpToDate";
        public const String SensorHighWarningPropertyName = "SensorHighWarning";
        public const String IsSensorLowWarningUpToDatePropertyName = "IsSensorLowWarningUpToDate";
        public const String SensorLowWarningPropertyName = "SensorLowWarning";
        public const String SensorNamePropertyName = "SensorName";
        public const String IsSensorMeasurementIntervalUpToDatePropertyName = "IsSensorMeasurementIntervalUpToDate";
        public const String SensorMeasurementIntervalPropertyName = "SensorMeasurementInterval";

        private Boolean _isSensorHighWarningUpToDate = false;
        private Nullable<Double> _sensorHighWarning;
        private Boolean _isSensorLowWarningUpToDate = false;
        private Nullable<Double> _sensorLowWarning;
        private String _sensorName = "Sensor 0";
        private Boolean _isSensorMeasurementIntervalUpToDate = false;
        private Byte _sensorMeasurementInterval;
        private Byte _sensorId;
        private Boolean _device_updating_enabled = true;

        public Boolean IsSensorMeasurementIntervalUpToDate
        {
            get
            {
                return _isSensorMeasurementIntervalUpToDate;
            }

            set
            {
                if (_isSensorMeasurementIntervalUpToDate == value)
                {
                    return;
                }

                RaisePropertyChanging(IsSensorMeasurementIntervalUpToDatePropertyName);
                _isSensorMeasurementIntervalUpToDate = value;
                RaisePropertyChanged(IsSensorMeasurementIntervalUpToDatePropertyName);
            }
        }

        public Byte SensorMeasurementInterval
        {
            get
            {
                return _sensorMeasurementInterval;
            }

            set
            {
                if (_sensorMeasurementInterval == value)
                {
                    return;
                }

                RaisePropertyChanging(SensorMeasurementIntervalPropertyName);
                _sensorMeasurementInterval = value;
                RaisePropertyChanged(SensorMeasurementIntervalPropertyName);
                if (_device_updating_enabled)
                {
                    SetMeasurementInterval();
                    IsSensorMeasurementIntervalUpToDate = false;
                }
            }
        }

        public Boolean IsSensorLowWarningUpToDate
        {
            get
            {
                return _isSensorLowWarningUpToDate;
            }

            set
            {
                if (_isSensorLowWarningUpToDate == value)
                {
                    return;
                }

                RaisePropertyChanging(IsSensorLowWarningUpToDatePropertyName);
                _isSensorLowWarningUpToDate = value;
                RaisePropertyChanged(IsSensorLowWarningUpToDatePropertyName);
            }
        }

        public Nullable<Double> SensorLowWarning
        {
            get
            {
                return _sensorLowWarning;
            }

            set
            {
                if (_sensorLowWarning == value)
                {
                    return;
                }

                RaisePropertyChanging(SensorLowWarningPropertyName);
                _sensorLowWarning = value;
                RaisePropertyChanged(SensorLowWarningPropertyName);
                if (_device_updating_enabled)
                {
                    SetWarningLevels();
                    IsSensorLowWarningUpToDate = false;
                }
            }
        }

        public bool IsSensorHighWarningUpToDate
        {
            get
            {
                return _isSensorHighWarningUpToDate;
            }

            set
            {
                if (_isSensorHighWarningUpToDate == value)
                {
                    return;
                }

                RaisePropertyChanging(IsSensorHighWarningUpToDatePropertyName);
                _isSensorHighWarningUpToDate = value;
                RaisePropertyChanged(IsSensorHighWarningUpToDatePropertyName);
            }
        }

        public Nullable<Double> SensorHighWarning
        {
            get
            {
                return _sensorHighWarning;
            }

            set
            {
                if (_sensorHighWarning == value)
                {
                    return;
                }

                RaisePropertyChanging(SensorHighWarningPropertyName);
                _sensorHighWarning = value;
                RaisePropertyChanged(SensorHighWarningPropertyName);
                if (_device_updating_enabled)
                {
                    SetWarningLevels();
                    IsSensorHighWarningUpToDate = false;
                }
            }
        }

        public string SensorName
        {
            get
            {
                return _sensorName;
            }

            set
            {
                if (_sensorName == value)
                {
                    return;
                }

                RaisePropertyChanging(SensorNamePropertyName);
                _sensorName = value;
                RaisePropertyChanged(SensorNamePropertyName);
            }
        }

        public ControlViewModel(IDeviceControl control)
        {
            _control = control;

            Messenger.Default.Register<SelectSensorMessage>(
                this, message => { UpdateSensorInfo(message.SensorId, message.SensorName); });
        }

        private void UpdateSensorInfo(byte sensorId, string sensorName)
        {
            this.SensorName = sensorName;
            _sensorId = sensorId;
            _control.SetDeviceTime(DeviceTimeSetCallback);
            _control.GetWarningLevels(WarningLevelsReceivedCallback, sensorId);
            _control.GetMeasurementInterval(MeasurementIntervalReceivedCallback, sensorId);
        }

        private void SetMeasurementInterval()
        {
            _control.SetMeasurementInterval(MeasurementIntervalSetCallback,
                _sensorId, this.SensorMeasurementInterval);
        }

        private void SetWarningLevels()
        {
            Int16 low, high;

            if (this.SensorLowWarning.HasValue == false)
            {
                low = MagicNumbers.LOW_WARNING_OFF_INT16;
            }
            else
            {
                low = (Int16)(Math.Round(this.SensorLowWarning.Value * 10));
            }

            if (this.SensorHighWarning.HasValue == false)
            {
                high = MagicNumbers.HIGH_WARNING_OFF_INT16;
            }
            else
            {
                high = (Int16)(Math.Round(this.SensorHighWarning.Value * 10));
            }

            _control.SetWarningLevels(WarningLevelsSetCallback, _sensorId, low, high);
        }

        /* Callback funtions */

        void DeviceTimeSetCallback()
        {
            
        }

        void MeasurementIntervalReceivedCallback(byte sensorId, byte interval)
        {
            if (sensorId == _sensorId)
            {
                // Disable device updating temporarily to avoid infinite update loops
                _device_updating_enabled = false;
                this.SensorMeasurementInterval = interval;
                _device_updating_enabled = true;
                this.IsSensorMeasurementIntervalUpToDate = true;
            }
        }

        void MeasurementIntervalSetCallback(byte sensorId, byte interval)
        {
            if (sensorId == _sensorId && this.SensorMeasurementInterval == interval)
            {
                this.IsSensorMeasurementIntervalUpToDate = true;
            }
            Messenger.Default.Send<StatusChangedMessage>(
                new StatusChangedMessage(String.Format(
                    "Measurement interval for sensor {0} set", sensorId)));
        }

        void WarningLevelsReceivedCallback(byte sensorId, Int16 lowWarning, Int16 highWarning)
        {
            Double low = ((Double)lowWarning) / 10.0;
            Double high = ((Double)highWarning) / 10.0;

            if (sensorId == _sensorId)
            {
                // Disable device updating temporarily to avoid infinite update loops
                _device_updating_enabled = false;
                if (lowWarning == MagicNumbers.LOW_WARNING_OFF_INT16)
                {
                    this.SensorLowWarning = null;
                }
                else
                {
                    this.SensorLowWarning = low;
                }

                if (highWarning == MagicNumbers.HIGH_WARNING_OFF_INT16)
                {
                    this.SensorHighWarning = null;
                }
                else
                {
                    this.SensorHighWarning = high;
                }
                
                _device_updating_enabled = true;
                this.IsSensorLowWarningUpToDate = true;
                this.IsSensorHighWarningUpToDate = true;
            }
            
            Messenger.Default.Send<SensorInfoMessage>(
                new SensorInfoMessage(sensorId, null, low, high));
        }

        void WarningLevelsSetCallback(byte sensorId, Int16 lowWarning, Int16 highWarning)
        {
            Double low = ((Double)lowWarning) / 10.0;
            Double high = ((Double)highWarning) / 10.0;

            if (sensorId == _sensorId)
            {
                if ((this.SensorLowWarning.HasValue == false &&
                     lowWarning == MagicNumbers.LOW_WARNING_OFF_INT16) ||
                    (this.SensorLowWarning.HasValue &&
                     this.SensorLowWarning.Value == low))
                {
                    this.IsSensorLowWarningUpToDate = true;
                }

                if ((this.SensorHighWarning.HasValue == false &&
                     highWarning == MagicNumbers.HIGH_WARNING_OFF_INT16) ||
                    (this.SensorHighWarning.HasValue &&
                     this.SensorHighWarning.Value == high))
                {
                    this.IsSensorHighWarningUpToDate = true;
                }
            }

            Messenger.Default.Send<SensorInfoMessage>(
                new SensorInfoMessage(sensorId, null, low, high));

            Messenger.Default.Send<StatusChangedMessage>(
                new StatusChangedMessage(String.Format(
                    "Warning levels for sensor {0} set", sensorId)));
        }
    }
}