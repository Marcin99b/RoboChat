using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboChat.Library
{
    public interface IRoboChat
    {
        string SendMessage(string username, string message);
        void MergeSession();
        void DeleteSession();
    }
}
