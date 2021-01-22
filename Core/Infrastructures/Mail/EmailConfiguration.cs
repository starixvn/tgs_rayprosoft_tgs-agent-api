namespace TGS.Core.Infrastructures.Mail
{
    public class EmailConfiguration
    {
        public string FromMail { get; set; }
        public string FromName { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
}