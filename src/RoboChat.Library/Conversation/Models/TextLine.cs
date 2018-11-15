using System;

namespace RoboChat.Library
{
    public class TextLine
    {
        public string UserName { get; private set; }
        public string Message { get; private set; }
        public DateTime Date { get; private set; }

        public TextLine(string userName, string message)
        {
            UserName = userName;
            Message = message;
            Date = DateTime.UtcNow;
        }
    }
}
