using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OHEXML.Entity.Entities;

namespace OHEXML.Entity.EntityConfiguration
{
    public class CharFriendTypeConfiguration : IEntityTypeConfiguration<CharFriend>
    {
        public void Configure(EntityTypeBuilder<CharFriend> builder)
        {
            builder.HasKey(c => new { c.UserId, c.FriendID });

            builder.Property(b => b.UserId)
           .HasMaxLength(125)
           .IsRequired();

            builder.Property(b => b.FriendID)
           .HasMaxLength(125)
           .IsRequired();
        }
    }
}
