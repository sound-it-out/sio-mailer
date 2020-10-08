using netDumbster.smtp;

namespace SIO.Testing.Fixtures
{
    public class SmtpServerFixture
    {
        public SimpleSmtpServer Server { get; }

        public SmtpServerFixture()
        {
            Server = SimpleSmtpServer.Start(25);
        }
    }
}
