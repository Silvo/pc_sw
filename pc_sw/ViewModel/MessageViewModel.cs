using GalaSoft.MvvmLight;
using pc_sw.Model;
using pc_sw.device_if;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using pc_sw.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace pc_sw.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MessageViewModel : ViewModelBase
    {
        public const string PauseButtonContentPropertyName = "PauseButtonContent";
        public const string MessagesPropertyName = "Messages";

        private object _pauseButtonContent;
        private bool _isPaused = false;
        private ObservableQueue<Message> _messages;
        private IMessageInterface _source;
        private List<Message> _messageQueueSnapshot;

        public object PauseButtonContent
        {
            get
            {
                return _pauseButtonContent;
            }

            set
            {
                if (_pauseButtonContent == value)
                {
                    return;
                }

                RaisePropertyChanging(PauseButtonContentPropertyName);
                _pauseButtonContent = value;
                RaisePropertyChanged(PauseButtonContentPropertyName);
            }
        }

        public ObservableQueue<Message> Messages
        {
            get
            {
                return _messages;
            }

            set
            {
                if (_messages == value)
                {
                    return;
                }

                RaisePropertyChanging(MessagesPropertyName);
                _messages = value;
                RaisePropertyChanged(MessagesPropertyName);
            }
        }
        public RelayCommand PauseCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public MessageViewModel(IMessageInterface source)
        {
            _messages = new ObservableQueue<Message>(500);

            _source = source;
            _source.MessageReceived += (msg) => { _messages.Enqueue(msg); };
            _source.MessageSent += (msg) => { _messages.Enqueue(msg); };

            PauseButtonContent = "Pause";
            PauseCommand = new RelayCommand(PauseHandler, () => true);
            SaveCommand = new RelayCommand(SaveHandler, () => true);

            Messenger.Default.Register<StatusChangedMessage>(
                this, message => { _messages.Enqueue(new WrapperMessage(message.Text)); });
        }


        public void PauseHandler()
        {
            if (_isPaused)
            {
                _isPaused = false;
                _messages.ResumeUpdates();
                PauseButtonContent = "Pause";
            }
            else
            {
                _isPaused = true;
                _messageQueueSnapshot = new List<Message>(_messages);
                _messages.PauseUpdates();
                PauseButtonContent = "Play";
            }
        }

        public void SaveHandler()
        {
            if (_messages.NotifyUpdates == false &&
                _messageQueueSnapshot != null)
            {
                SaveMessages(_messageQueueSnapshot);
            }
            else
            {
                SaveMessages(_messages);
            }
        }

        private void SaveMessages(IEnumerable<Message> _messages)
        {
            var dialog = new SaveFileDialog()
            {
                FileName = "MessageLog" + DateTime.Now.ToString("s"),
                DefaultExt = ".txt",
                Filter = "Text documents|*.txt"
            };

            Nullable<bool> result = dialog.ShowDialog();

            if (result == true)
            {
                using (var sw = new StreamWriter(File.Open(
                    dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None)))
                {
                    foreach (var message in _messages)
                    {
                        sw.WriteLine(message.ToString());
                    }
                }
            }
        }
    }
}