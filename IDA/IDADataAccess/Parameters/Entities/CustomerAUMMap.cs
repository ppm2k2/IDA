using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace IDADataAccess.Parameters.Entities
{
    public class CustomerAUMMap : EntityTypeConfiguration<CustomAUMMapping>
    {
        public CustomerAUMMap()
        {
            //Primary Key
            this.HasKey(x => x.Id);

            this.ToTable("CustomAUM");

            this.Property(x => x.Client).HasColumnName("Client");
            this.Property(x => x.Portfolio).HasColumnName("Portfolio");
            this.Property(x => x.AUM).HasColumnName("AUM");
            this.Property(x => x.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(x => x.IsStatic).HasColumnName("IsStatic");
            this.Property(x => x.sysDate).HasColumnName("sysDate");
            this.Property(x => x.ModifiedBy).HasColumnName("ModifiedBy");
        }
    }
}
