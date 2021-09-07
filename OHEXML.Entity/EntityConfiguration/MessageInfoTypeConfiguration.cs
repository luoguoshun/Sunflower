using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OHEXML.Entity.Entities;
using System;

namespace OHEXML.Entity.EntityConfiguration
{
    public class MessageInfoTypeConfiguration : IEntityTypeConfiguration<MessageInfo>
    {
        public void Configure(EntityTypeBuilder<MessageInfo> builder)
        {
            builder.ToTable("MessageInfo").HasKey(f => f.Id);

            //备注：在一对多关系中，具有引用导航的实体是依赖实体，具有集合导航的实体是主体实体。
            builder.HasOne(x => x.log)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade)/*联级删除*/
                   .HasForeignKey<MessageInfo>(x => x.logId);

        }
    }
}
