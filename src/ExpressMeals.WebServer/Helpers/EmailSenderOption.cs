namespace ExpressMeals.WebServer.Helpers
{
    public class EmailSenderOption
    {
        public string From { get; set; }
        public bool IsAuthenticate { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public bool Ssl { get; set; }
        public int Port { get; set; }
        public bool IsBodyHtml { get; set; } = true;
        public string Bcc { get; set; }
    }
}
