namespace RoboChat.Library.Settings
{
    public class SessionSettings
    {
        public string SessionName { get; private set; }
        public bool LearnFaster { get; private set; }

        public SessionSettings(string sessionName, bool learnFaster)
        {
            SessionName = sessionName;
            LearnFaster = learnFaster;
        }
    }
}
