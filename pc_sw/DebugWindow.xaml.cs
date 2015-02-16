using GalaSoft.MvvmLight.Messaging;
using pc_sw.Helpers;
using pc_sw.ViewModel;
using System.Windows;

namespace pc_sw
{
    /// <summary>
    /// Description for DebugWindow.
    /// </summary>
    public partial class DebugWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the DebugWindow class.
        /// </summary>
        public DebugWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<CloseProgramMessage>(
                this, message => { this.Close(); });
        }
    }
}