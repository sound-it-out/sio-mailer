using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIO.Infrastructure.EntityFrameworkCore.EntityConfiguration;
using SIO.Infrastructure.EntityFrameworkCore.Extensions;

namespace SIO.Domain.Emails.Projections.Configurations
{
    internal sealed class EmailQueueTypeConfiguration : IProjectionTypeConfiguration<EmailQueue>
    {
        public void Configure(EntityTypeBuilder<EmailQueue> builder)
        {
            builder.ToTable(nameof(EmailQueue));
            builder.HasKey(epq => epq.Subject);
            builder.Property(epq => epq.Subject)
                   .ValueGeneratedNever();
        }
    }
}
