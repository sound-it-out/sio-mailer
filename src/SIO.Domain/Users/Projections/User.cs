using System;

namespace SIO.Domain.Users.Projections
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordToken { get; set; }
        public string ActivationToken { get; set; }
    }
}
