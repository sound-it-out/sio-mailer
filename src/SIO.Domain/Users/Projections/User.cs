using SIO.Infrastructure.Projections;

namespace SIO.Domain.Users.Projections
{
    public class User : IProjection
    {
        public string Subject { get; set; }
        public string Email { get; set; }
    }
}
