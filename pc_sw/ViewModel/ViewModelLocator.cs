/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:pc_sw.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using pc_sw.Design;
using pc_sw.Model;
using System;

namespace pc_sw.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IMessageInterface, RandomDevice>();
                SimpleIoc.Default.Register<IDataStorage, DummySensorDataStorage>();

                SimpleIoc.Default.Register<DummyDeviceController>();
                SimpleIoc.Default.Register<IDeviceControl>(() =>
                {
                    return SimpleIoc.Default.GetInstance<DummyDeviceController>();
                });
                SimpleIoc.Default.Register<ISensorDataProvider>(() =>
                {
                    return SimpleIoc.Default.GetInstance<DummyDeviceController>();
                });
            }
            else
            {
                SimpleIoc.Default.Register<IMessageInterface, SerialPortDevice>();//SerialPortDevice>();
                SimpleIoc.Default.Register<IDataStorage, SimpleSensorDataStorage>();

                SimpleIoc.Default.Register<DeviceController>();
                SimpleIoc.Default.Register<IDeviceControl>(() =>
                {
                    return SimpleIoc.Default.GetInstance<DeviceController>();
                });
                SimpleIoc.Default.Register<ISensorDataProvider>(() =>
                {
                    return SimpleIoc.Default.GetInstance<DeviceController>();
                });
            }
            
            SimpleIoc.Default.Register<ControlViewModel>();

            SimpleIoc.Default.Register<MessageViewModel>(true);
            SimpleIoc.Default.Register<DebugWindowViewModel>();

            SimpleIoc.Default.Register<GraphViewModel>();
            SimpleIoc.Default.Register<SensorListViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }

        /// <summary>
        /// Gets the MessageVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MessageViewModel MessageVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MessageViewModel>();
            }
        }

        /// <summary>
        /// Gets the DebugWindowVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DebugWindowViewModel DebugWindowVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DebugWindowViewModel>();
            }
        }

        /// <summary>
        /// Gets the ControlVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ControlViewModel ControlVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ControlViewModel>();
            }
        }

        /// <summary>
        /// Gets the SensorListVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SensorListViewModel SensorListVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SensorListViewModel>();
            }
        }

        /// <summary>
        /// Gets the GraphVM property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GraphViewModel GraphVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GraphViewModel>();
            }
        }
    }
}