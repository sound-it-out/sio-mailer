using System;
using OpenEventSourcing.Queries;
using SIO.Domain.Users.Projections;

namespace SIO.Domain.Users.Queries
{
    public class UserQueryResult : IQueryResult
    {
        public User User { get; }

        public UserQueryResult(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User = user;
        }
    }
}
