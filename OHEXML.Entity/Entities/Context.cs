using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace OHEXML.Entity.Entities
{
    public partial class Context : DbContext
    {
        public Context()
        {
        }

        public Context(DbContextOptions<Context> options): base(options)
        {
        }

        public virtual DbSet<LogInfo> LogInfos { get; set; }
        public virtual DbSet<WordPathInfo> WordPathInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=192.168.1.9;Database=Kr_OHE;uid=sa;pwd=Kr123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_PRC_CI_AS");

            modelBuilder.Entity<LogInfo>(entity =>
            {
                entity.ToTable("LogInfo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ErroeMessage)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);//将属性配置为能够持久化unicode字符

                entity.Property(e => e.ErrorTime)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);//将属性配置为能够持久化unicode字符
            });

            modelBuilder.Entity<WordPathInfo>(entity =>
            {
                entity.ToTable("WordPathInfo");

                entity.Property(e => e.Id).HasColumnName("id");//设置实体名与数据库字段名称一样

                entity.Property(e => e.BasePath)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);

                entity.Property(e => e.WordName)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);

                entity.Property(e => e.SaveTime)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);

                entity.Property(e => e.UnitName)
                    .IsRequired()
                    .HasMaxLength(125)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
