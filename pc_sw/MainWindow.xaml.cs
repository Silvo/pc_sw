using System.Windows;
using pc_sw.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;
using pc_sw.Helpers;
using System.Windows.Input;
using System.Windows.Media;

namespace pc_sw
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += (s, e) => ViewModelLocator.Cleanup();

            Messenger.Default.Register<CloseProgramMessage>(
                this, message => { this.Close(); });
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.M && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                var uniqueKey = System.Guid.NewGuid().ToString();
                var debugWindowVM = SimpleIoc.Default.GetInstance<DebugWindowViewModel>(uniqueKey);
                var debugWindow = new DebugWindow()
                {
                    DataContext = debugWindowVM
                };
                debugWindow.Closed += (s, ea) => SimpleIoc.Default.Unregister(uniqueKey);
                debugWindow.Show();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}