﻿using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Queries;
using SIO.Domain.Users.Queries;

namespace SIO.Domain.Emails.Processors
{
    internal sealed class EmailProcessor : IEmailProcessor
    {
        private readonly SmtpOptions _smtpOptions;

        public EmailProcessor(IOptions<SmtpOptions> smtpOptions)
        {
            if (smtpOptions == null)
                throw new ArgumentNullException(nameof(smtpOptions));

            _smtpOptions = smtpOptions.Value;
        }

        public async Task ProcessAsync(MailMessage mailMessage)
        {
            using (var smtpClient = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port))
            {
                smtpClient.DeliveryMethod = _smtpOptions.DeliveryMethod;
                smtpClient.EnableSsl = _smtpOptions.UseSsl;

                if(!string.IsNullOrWhiteSpace(_smtpOptions.Username))
                    smtpClient.Credentials = new NetworkCredential(_smtpOptions.Username, _smtpOptions.Password);

                if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                    smtpClient.PickupDirectoryLocation = _smtpOptions.DirectoryPickupLocation;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
