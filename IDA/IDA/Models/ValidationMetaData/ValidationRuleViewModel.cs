using System.ComponentModel.DataAnnotations;

namespace IDA.Models
{
    public class ValidationRuleViewModel
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [Display(Name = "Column Def Id")]
        public virtual int ValidationTableColumnDefId { get; set; }

        [StringLength(1000)]
        [Display(Name = "Operand Column")]
        public virtual string OperandColumn { get; set; }

        [StringLength(20)]
        [Display(Name = "Operator")]
        public virtual string ValidationOperator { get; set; }

        [StringLength(1000)]
        [Display(Name = "Operand Value")]
        public virtual string OperandValue { get; set; }

        [Required]
        [Display(Name = "Step")]
        public virtual int ValidationStep { get; set; }

        [StringLength(100)]
        [Display(Name = "Form Type Ids")]
        public virtual string ValidationFormTypeIds { get; set; }
    }
}