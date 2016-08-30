using System.ComponentModel.DataAnnotations;

namespace IDA.Models
{
    public class ValidationRulesErrorMessageViewModel
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [Display(Name = "Column Id")]
        public virtual int ColumnID { get; set; }

        [Required]
        [Display(Name = "Message")]
        [StringLength(500)]
        public virtual string Message { get; set; }

        [Display(Name = "Form Type Ids")]
        [StringLength(100)]
        public virtual string ValidationFormTypeIds { get; set; }
    }
}