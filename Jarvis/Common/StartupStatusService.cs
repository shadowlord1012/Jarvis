using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Loader
{
    public class StartupStatusService
    {

        private readonly ObservableCollection<StartupMessage> _messages = new();

        public ReadOnlyObservableCollection<StartupMessage> Messages { get; }

        public StartupStatusService()
        {
            Messages = new ReadOnlyObservableCollection<StartupMessage>(_messages);
        }

        public void Info(string text)
        {
            _messages.Add(new StartupMessage
            {
                Time = DateTime.Now,
                Status = StartupStatus.Information,
                Message = text
            });
        }

        public void Success(string text)
        {
            _messages.Add(new StartupMessage
            {
                Time = DateTime.Now,
                Status = StartupStatus.Success,
                Message = text
            });
        }

        public void Error(string text)
        {
            _messages.Add(new StartupMessage
            {
                Time = DateTime.Now,
                Status = StartupStatus.Error,
                Message = text
            });
        }
    }
}
