using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OHEXML.Entity.Entities;

namespace OHEXML.Entity.EntityConfiguration
{
    public class UserInfoTypeConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            //builder.HasMany(x => x.Roles)
            //       .WithMany(x => x.users)
            //       .UsingEntity(j => j.ToTable("USER_VS_ROLE"));
        }
    }
}
