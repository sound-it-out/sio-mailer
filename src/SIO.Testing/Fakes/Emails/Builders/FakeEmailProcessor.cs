using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using SIO.Domain.Emails.Processors;

namespace SIO.Testing.Fakes.Notifications.Builders
{
    public sealed class FakeEmailProcessor : IEmailProcessor
    {
        private readonly bool _throwException;
        private readonly string _exceptionMessage;

        public FakeEmailProcessor(bool throwException, string exceptionMessage)
        {
            _throwException = throwException;
            _exceptionMessage = exceptionMessage;
        }

        public Task ProcessAsync(MailMessage mailMessage)
        {
            if (_throwException)
                throw new Exception(_exceptionMessage);

            return Task.CompletedTask;
        }
    }
}
