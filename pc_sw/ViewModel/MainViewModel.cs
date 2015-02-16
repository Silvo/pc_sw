using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using pc_sw.Helpers;
using pc_sw.Model;
using System;
using System.Windows;

namespace pc_sw.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public const string StatusMessagesPropertyName = "StatusMessages";
        public RelayCommand CloseCommand { get; private set; }

        private ObservableQueue<String> _statusMessages = new ObservableQueue<string>();

        public ObservableQueue<String> StatusMessages
        {
            get
            {
                return _statusMessages;
            }

            set
            {
                if (_statusMessages == value)
                {
                    return;
                }

                RaisePropertyChanging(StatusMessagesPropertyName);
                _statusMessages = value;
                RaisePropertyChanged(StatusMessagesPropertyName);
            }
        }

        public MainViewModel()
        {
            this.CloseCommand = new RelayCommand(() =>
                Messenger.Default.Send<CloseProgramMessage>(new CloseProgramMessage()));

            Messenger.Default.Register<StatusChangedMessage>(
                this, message => { StatusMessages.Enqueue(message.Text); });
        }
    }
}