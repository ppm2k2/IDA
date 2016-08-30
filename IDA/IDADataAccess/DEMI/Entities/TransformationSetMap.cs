using System.Data.Entity.ModelConfiguration;

namespace IDADataAccess.DEMI.Entities
{
    public class TransformationSetMap : EntityTypeConfiguration<TransformationSet>
    {

        public TransformationSetMap()
        {
            //Primary Key
            HasKey(x => x.Id);

            //Table & Column mappings
            ToTable("Transformation_Sets");
            Property(x => x.CreateDateTime).HasColumnName("Create_Date_Time");
        }
    }

}

