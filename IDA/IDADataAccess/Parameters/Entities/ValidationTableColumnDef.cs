using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.Parameters.Entities
{
    public class ValidationTableColumnDef
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string TableName { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string ColumnName { get; set; }
    }
}
