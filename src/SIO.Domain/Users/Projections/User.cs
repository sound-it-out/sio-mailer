using SIO.Infrastructure.Projections;

namespace SIO.Domain.Users.Projections
{
    public class User : IProjection
    {
        public string Subject { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
    }
}
