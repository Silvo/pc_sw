using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using pc_sw.Helpers;
using pc_sw.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace pc_sw.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SensorListViewModel : ViewModelBase
    {
        public const string SourcesPropertyName = "Sources";
        
        private IDataStorage _model;
        private ObservableCollection<SensorViewModel> _sources;

        public ObservableCollection<SensorViewModel> Sources
        {
            get
            {
                return _sources;
            }

            set
            {
                if (_sources == value)
                {
                    return;
                }

                RaisePropertyChanging(SourcesPropertyName);
                _sources = value;
                RaisePropertyChanged(SourcesPropertyName);
            }
        }

        public RelayCommand<SelectionChangedEventArgs> SelectSensorCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the SensorListViewModel class.
        /// </summary>
        public SensorListViewModel(IDataStorage model)
        {
            _model = model;
            Sources = new ObservableCollection<SensorViewModel>();
            foreach (var source in _model.GetDataSources())
            {
                Sources.Add(new SensorViewModel(source));
            }

            this.SelectSensorCommand = new RelayCommand<SelectionChangedEventArgs>(
                e =>
                {
                    byte id = ((SensorViewModel)e.AddedItems[0]).Id;
                    string name = ((SensorViewModel)e.AddedItems[0]).Name;
                    Messenger.Default.Send<SelectSensorMessage>(new SelectSensorMessage(id, name));
                });

            _model.SourceAdded += (s, e) => { Sources.Add(new SensorViewModel(((SourceAddedEventArgs)e).Source)); };
        }



    }
}