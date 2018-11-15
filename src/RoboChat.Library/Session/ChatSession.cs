using System;
using RoboChat.Library.Settings;

namespace RoboChat.Library
{
    public class ChatSession
    {
        public RoboChat RoboChat { get; private set; }
        public string SessionOwner { get; private set; }
        public string RoomName { get; private set; }

        public ChatSession(string fullUsername, string roomName, SessionSettings sessionSettings)
        {
            SessionOwner = fullUsername;
            RoomName = roomName;
            RoboChat = new RoboChat(sessionSettings);
        }
    }
}
