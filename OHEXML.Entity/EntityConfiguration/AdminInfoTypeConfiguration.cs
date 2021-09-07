using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OHEXML.Entity.Entities;
using System;

namespace OHEXML.Entity.EntityConfiguration
{
    public class AdminInfoTypeConfiguration : IEntityTypeConfiguration<AdminInfo>
    {
        public void Configure(EntityTypeBuilder<AdminInfo> builder)
        {
            builder.HasMany(x => x.Roles)
                   .WithMany(x => x.admins)
                   .UsingEntity(j => j.ToTable("USER_VS_ROLE"));
        }
    }
}
