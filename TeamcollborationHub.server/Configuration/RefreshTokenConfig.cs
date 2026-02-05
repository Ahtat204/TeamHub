using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Configuration;

public class RefreshTokenConfig:IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Expires).IsRequired();
        builder.Property(x=>x.Token).HasMaxLength(256);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
    }
}