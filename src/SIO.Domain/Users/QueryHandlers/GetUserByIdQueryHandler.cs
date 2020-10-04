using System;
using System.Threading.Tasks;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Queries;
using SIO.Domain.Users.Projections;
using SIO.Domain.Users.Queries;

namespace SIO.Domain.Users.QueryHandlers
{
    internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserQueryResult>
    {
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;

        public GetUserByIdQueryHandler(IProjectionDbContextFactory projectionDbContextFactory)
        {
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));

            _projectionDbContextFactory = projectionDbContextFactory;
        }

        public async Task<UserQueryResult> RetrieveAsync(GetUserByIdQuery query)
        {
            using (var context = _projectionDbContextFactory.Create())
            {
                var user = await context.Set<User>().FindAsync(query.AggregateId);
                return new UserQueryResult(user);
            }
        }
    }
}
