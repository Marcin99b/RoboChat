using System;
using System.Collections.Generic;
using System.Linq;
using RoboChat.Library;

namespace RoboChat.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            new ChatApp().RunConsoleChat();
        }
        
        public class ChatApp
        {
            private readonly List<TextLine> history = new List<TextLine>();
            private readonly Library.RoboChat roboChat;

            public ChatApp()
            {
            }

            public void RunConsoleChat()
            {
                System.Console.Write("Set you username: ");
                var userName = GetUsername();
                while (true)
                {
                    var textLine = GetTextLine(userName);
                    roboChat.SendMessage(textLine);
                }
            }

            private string GetUsername()
            {
                var userName = System.Console.ReadLine();
                System.Console.Clear();
                return userName;
            }

            private TextLine GetTextLine(string userName)
            {
                System.Console.Write($"{userName}: ");
                var text = System.Console.ReadLine();
                System.Console.Clear();
                return new TextLine(userName, text);
            }
            
        }
    }
}
