namespace MultiLoader.TelegramFacade.Infrastructure
{
    public class Settings
    {
        public string TelegramBotToken { get; set; }
        public string TelegramUsersAccess { get; set; }
        public Proxy Proxy { get; set; }
    }

    public class Proxy
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}