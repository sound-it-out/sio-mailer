using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Queries;
using SIO.Domain.Emails.Projections;
using SIO.Domain.Emails.Queries;

namespace SIO.Domain.Emails.QueryHandlers
{
    public class GetEmailsQueryHandler : IQueryHandler<GetEmailsQuery, EmailsQueryResult>
    {
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;

        public GetEmailsQueryHandler(IProjectionDbContextFactory projectionDbContextFactory)
        {
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));

            _projectionDbContextFactory = projectionDbContextFactory;
        }

        public async Task<EmailsQueryResult> RetrieveAsync(GetEmailsQuery query)
        {
            using(var context = _projectionDbContextFactory.Create())
            {
                var emails = await context.Set<EmailQueue>().Take(query.PageSize).ToArrayAsync();
                return new EmailsQueryResult(emails);
            }
        }
    }
}
