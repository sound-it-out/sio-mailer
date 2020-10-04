using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.EntityConfiguration;
using SIO.Domain.Emails.Projections;

namespace SIO.Domain.Projections.Users.TypeConfigurations
{
    internal class UserTypeConfiguration : IProjectionTypeConfiguration<Domain.Users.Projections.User>
    {
        public void Configure(EntityTypeBuilder<Domain.Users.Projections.User> builder)
        {
            builder.ToTable(nameof(Domain.Users.Projections.User));
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                   .ValueGeneratedNever();
            builder.HasIndex(i => i.Email);
        }
    }
}
