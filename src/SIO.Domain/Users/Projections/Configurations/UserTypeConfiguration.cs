using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration;

namespace SIO.Domain.Users.Projections.Configurations
{
    internal sealed class UserTypeConfiguration : IProjectionTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(epf => epf.Subject);
            builder.Property(epf => epf.Subject)
                   .ValueGeneratedNever();
        }
    }
}
