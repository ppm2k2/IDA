using System.Data.Entity.ModelConfiguration;

namespace IDADataAccess.Parameters.Entities
{
    public class ParametersDataAppUsersMap : EntityTypeConfiguration<ParametersDataAppUser>
    {
        public ParametersDataAppUsersMap()
        {
            //Primary Key
            this.HasKey(x => x.Id);

            this.ToTable("Parameters_DataApp_Users");

            this.Property(x => x.Role).HasColumnName("Role");
            this.Property(x => x.TableEditLockOn).HasColumnName("Table_Edit_Lock_On");
            this.Property(x => x.TableEditLockUntil).HasColumnName("Table_Edit_Lock_Until");
            this.Property(x => x.WindowsUsername).HasColumnName("Windows_Username");
        }
    }
}
