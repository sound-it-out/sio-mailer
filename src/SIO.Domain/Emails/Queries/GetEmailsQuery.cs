using System;
using OpenEventSourcing.Queries;

namespace SIO.Domain.Emails.Queries
{
    public class GetEmailsQuery : Query<EmailsQueryResult>
    {
        public int PageSize { get; }

        public GetEmailsQuery(Guid correlationId, string userId, int pageSize) : base(correlationId, userId)
        {
            PageSize = pageSize;
        }
    }
}
