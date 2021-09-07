using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OHEXML.Entity.Entities;

namespace OHEXML.Entity.EntityConfiguration
{
    /// <summary>
    /// WordPathInfo模型配置
    /// </summary>
    public class WordPathInfoTypeConfiguration : IEntityTypeConfiguration<WordPathInfo>
    {
        public void Configure(EntityTypeBuilder<WordPathInfo> builder)
        {

            builder.Property(b => b.Id)
           .HasColumnName("id")//设置别名
           .IsRequired();

            builder.Property(b => b.BasePath)
           .HasMaxLength(125)
           .IsUnicode(false);//将属性配置为能够持久化unicode字符

            builder.Property(b => b.WordName)
           .HasMaxLength(125)
           .IsUnicode(false);//将属性配置为能够持久化unicode字符

            builder.Property(b => b.SaveTime)
          .HasMaxLength(125)
          .IsUnicode(false);//将属性配置为能够持久化unicode字符

            builder.Property(b => b.UnitName)
          .HasMaxLength(125)
          .IsUnicode(false);//将属性配置为能够持久化unicode字符
        }
    }
}
