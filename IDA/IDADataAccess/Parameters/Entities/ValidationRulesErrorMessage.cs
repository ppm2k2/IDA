using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.Parameters.Entities
{
    public class ValidationRulesErrorMessages
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual int ColumnID { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string Message { get; set; }

        [StringLength(100)]
        public virtual string ValidationFormTypeIds { get; set; }
    }
}
