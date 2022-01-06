using System.Net.Mail;

namespace SIO.Domain.Emails
{
    public class SmtpOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string From { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }
        public string? DirectoryPickupLocation { get; set; }
        public bool UseSsl { get; set; }
    }
}
