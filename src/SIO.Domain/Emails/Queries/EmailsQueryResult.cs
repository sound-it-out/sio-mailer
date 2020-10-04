using System;
using System.Collections.Generic;
using OpenEventSourcing.Queries;
using SIO.Domain.Emails.Projections;

namespace SIO.Domain.Emails.Queries
{
    public class EmailsQueryResult : IQueryResult
    {
        public IEnumerable<EmailQueue> Emails { get; set; }

        public EmailsQueryResult(IEnumerable<EmailQueue> emails)
        {
            if (emails == null)
                throw new ArgumentNullException(nameof(emails));

            Emails = emails;
        }
    }
}
