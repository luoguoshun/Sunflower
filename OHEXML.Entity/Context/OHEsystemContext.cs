using Microsoft.EntityFrameworkCore;
using OHEXML.Common.Extentions;
using OHEXML.Entity.Base;
using OHEXML.Entity.Entities;
using OHEXML.Entity.EntityConfiguration;
using System.Reflection;

namespace OHEXML.Entity.Context
{
    public class OHEsystemContext : DbContext
    {
        /// <summary>
        /// 这是将 AddDbContext 的上下文配置传递到 DbContext 的方式
        /// </summary>
        /// <param name="options"></param>
        public OHEsystemContext(DbContextOptions<OHEsystemContext> options) : base(options)
        {
        }

        /// <summary>
        /// 创建模型
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new AdminInfoTypeConfiguration().Configure(modelBuilder.Entity<AdminInfo>());
            modelBuilder
              .LoadEntityConfiguration<OHEsystemContext>()
              .AddEntityTypes<BaseEntity>();
            //在程序集中(OHEXML.Entity)实现 IEntityTypeConfiguration 的类型中指定的所有配置
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("OHEXML.Entity"));
        }
    }
}
