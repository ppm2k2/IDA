using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using IDADataAccess.Parameters.Entities;

namespace IDADataAccess.Parameters
{
    public class ParametersContext : DbContext
    {
        public ParametersContext()
            : base("ParametersConnection")
        {
            Database.SetInitializer<ParametersContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new ParametersDataAppUsersMap());
        }

        // Validation Metadata
        public DbSet<ParametersDataAppUser> ParametersDataAppUsers { get; set; }
        public DbSet<ValidationEnumeration> ValidationEnumeration { get; set; }
        public DbSet<ValidationFormTypeMapping> ValidationFormTypeMapping { get; set; }
        public DbSet<ValidationRules> ValidationRules { get; set; }
        public DbSet<ValidationRulesErrorMessages> ValidationRulesErrorMessages { get; set; }
        public DbSet<ValidationTableColumnDef> ValidationTableColumnDef { get; set; }
    }
}
