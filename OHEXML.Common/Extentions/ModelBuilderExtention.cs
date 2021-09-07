using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OHEXML.Common.Extentions
{
    public static class ModelBuilderExtention
    {
        public static ModelBuilder LoadEntityConfiguration<T>(this ModelBuilder modelBuilder)
        {
            var types = typeof(T).Assembly.GetTypes();
            foreach (var type in types.Where(t => t.GetInterfaces().Select(i => i.Name).Contains(typeof(IEntityTypeConfiguration<>).Name)))
            {
                dynamic instance = type.GetConstructor(new Type[0]).Invoke(null);
                modelBuilder.ApplyConfiguration(instance);
            }
            return modelBuilder;
        }

        public static ModelBuilder AddEntityTypes<T>(this ModelBuilder modelBuilder)
        {
            var types = typeof(T).Assembly.GetTypes();
            var entityTypes = types.Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
            foreach (var type in entityTypes)
            {
                if (modelBuilder.Model.FindEntityType(type) is null)
                    modelBuilder.Model.AddEntityType(type);
            }
            return modelBuilder;
        }
    }
}
