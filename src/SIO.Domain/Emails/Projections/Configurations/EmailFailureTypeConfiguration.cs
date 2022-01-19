using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration;
using SIO.Infrastructure.EntityFrameworkCore.Extensions;

namespace SIO.Domain.Emails.Projections.Configurations
{
    internal sealed class EmailFailureTypeConfiguration : IProjectionTypeConfiguration<EmailFailure>
    {
        public void Configure(EntityTypeBuilder<EmailFailure> builder)
        {
            builder.ToTable(nameof(EmailFailure));
            builder.HasKey(epf => epf.Id);
            builder.Property(epf => epf.Id)
                   .ValueGeneratedNever();
            builder.HasIndex(ef => ef.Subject);
            builder.Property(epq => epq.Recipients)
                .HasJsonValueConversion();
        }
    }
}
