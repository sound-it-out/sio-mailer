using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenEventSourcing.EntityFrameworkCore.EntityConfiguration;
using OpenEventSourcing.EntityFrameworkCore.Extensions;
using SIO.Domain.Emails.Projections;

namespace SIO.Domain.Projections.Emails.TypeConfigurations
{
    internal class EmailQueueTypeConfiguration : IProjectionTypeConfiguration<EmailQueue>
    {
        public void Configure(EntityTypeBuilder<EmailQueue> builder)
        {
            builder.ToTable(nameof(EmailQueue));
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                   .ValueGeneratedNever();
        }
    }
}
