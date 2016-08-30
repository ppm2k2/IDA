using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.Parameters.Entities
{
    public class ValidationRules
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual int ValidationTableColumnDefId { get; set; }

        [StringLength(1000)]
        public virtual string OperandColumn { get; set; }

        [StringLength(20)]
        public virtual string ValidationOperator { get; set; }

        [StringLength(1000)]
        public virtual string OperandValue { get; set; }

        [Required]
        public virtual int ValidationStep { get; set; }

        [StringLength(100)]
        public virtual string ValidationFormTypeIds { get; set; }
    }
}
