namespace SIO.Domain.Emails.Services
{
    public class EmailPublisherOptions
    {
        public int Interval { get; set; }
        public int MaxRetries { get; set; }
    }
}
