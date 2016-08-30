using System.Data.Entity.ModelConfiguration;

namespace IDADataAccess.DEMI.Entities
{
    public class TransformationMap : EntityTypeConfiguration<Transformation>
    {

        public TransformationMap()
        {
            //Primary Key
            HasKey(x => x.Id);

            //Table & Column mappings
            ToTable("Transformations");
            Property(x => x.CreateDateTime).HasColumnName("Create_Date_Time");
            Property(x => x.UpdateDateTime).HasColumnName("Update_Date_Time");
            Property(x => x.TransformationSetId).HasColumnName("Transformation_Set_Id");
            Property(x => x.TargetColumn).HasColumnName("Target_Column");
            Property(x => x.TransformationRule).HasColumnName("Transformation_Rule");
        }
    }

}

