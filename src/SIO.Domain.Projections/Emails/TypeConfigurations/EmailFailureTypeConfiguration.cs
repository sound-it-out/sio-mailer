using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.EntityConfiguration;
using SIO.Domain.Emails.Projections;

namespace SIO.Domain.Projections.Emails.TypeConfigurations
{
    internal class EmailFailureTypeConfiguration : IProjectionTypeConfiguration<EmailFailure>
    {
        public void Configure(EntityTypeBuilder<EmailFailure> builder)
        {
            builder.ToTable(nameof(EmailFailure));
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                   .ValueGeneratedNever();
            builder.HasIndex(i => i.EmailId);
        }
    }
}
