using Microsoft.EntityFrameworkCore;
using OHEXML.Common.Extentions;
using OHEXML.Entity.Base;
using OHEXML.Entity.Entities;
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
            modelBuilder
              .LoadEntityConfiguration<OHEsystemContext>()
              .AddEntityTypes<BaseEntity>();
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("OHEXML.Entity"));
        }
    }
}
