using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboChat.Library
{
    public class HistoryMessage
    {
        public Guid Id { get; private set; }
        public TextLine RobotMessage { get; private set; }
        public TextLine UserResponse { get; private set; }
        public bool UserResponsed => UserResponse != null;

        public HistoryMessage(TextLine robotMessage, TextLine userResponse = null)
        {
            Id = Guid.NewGuid();
            RobotMessage = robotMessage;
            UserResponse = userResponse;
        }

        public void SetUserResponse(TextLine userResponse)
        {
            UserResponse = userResponse;
        }
    }
}
