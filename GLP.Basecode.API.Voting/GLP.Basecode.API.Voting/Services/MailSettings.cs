namespace GLP.Basecode.API.Voting.Services
{
    public class MailSettings
    {
        public string MailSender { get; set; } = null!;
        public string MailSenderAppPassword { get; set; } = null!;
        public string SmtpHost { get; set; } = null!;
        public int SmtpPort { get; set; }
    }

}
