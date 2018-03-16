using RMS.Map.Shared;
using RMS.Models.Shared;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Reflection;

namespace RMS.Infrastructure.Context
{
    public class RmsContext : DbContext
    {
        public RmsContext() : base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var assemblyEntity = Assembly.GetAssembly(typeof(ForModels));
            foreach (var ass in assemblyEntity.GetTypes().Where(x => x.IsClass && x.FullName.Contains("Modules")))
            {
                this.Set(ass);
            }

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            var assembly = Assembly.GetAssembly(typeof(ForMap));
            modelBuilder.Configurations.AddFromAssembly(assembly);
            
        }
    
    }
}

