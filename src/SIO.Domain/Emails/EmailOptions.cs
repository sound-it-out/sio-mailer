namespace SIO.Domain.Emails
{
    public class EmailOptions
    {
        public EmailUrls Urls { get; set; }

        public class EmailUrls
        {
            public string ResetPassword { get; set; }
            public string Activate { get; set; }
        }
    }
}
