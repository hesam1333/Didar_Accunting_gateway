using Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gateway.Infrastructure.Persistence;

public class ConnectorEntityTypeConfiguration : IEntityTypeConfiguration<Connector>
{
    public void Configure(EntityTypeBuilder<Connector> builder)
    {
        builder.ToTable("Connectors");

        builder.HasKey(connector => connector.Id);
        builder.Property(connector => connector.Id).ValueGeneratedNever();

        builder.Property(connector => connector.BizDomainId)
            .IsRequired();

        builder.Property(connector => connector.Provider)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(connector => connector.ApiKey)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(connector => connector.ApiKey).IsUnique();

        builder.HasIndex(connector => new { connector.BizDomainId, connector.Provider }).IsUnique();

        builder.Property(connector => connector.LoginTokenEncrypted)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(connector => connector.AgentSecretEncrypted)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(connector => connector.IsActive)
            .IsRequired();

        builder.Property(connector => connector.CreatedAt)
            .IsRequired();

        builder.Property(connector => connector.UpdatedAt)
            .IsRequired();
    }
}
