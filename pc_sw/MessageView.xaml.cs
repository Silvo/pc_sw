using pc_sw.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace pc_sw
{
    /// <summary>
    /// Description for MessageView.
    /// </summary>
    public partial class MessageView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MessageView class.
        /// </summary>
        public MessageView()
        {
            InitializeComponent();
            //Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}