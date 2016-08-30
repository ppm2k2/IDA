using System.ComponentModel.DataAnnotations;

namespace IDA.Models
{
    public class ValidationFormTypeMappingViewModel
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [Display(Name = "Column Def Id")]
        public virtual int ValidationTableColumnDefId { get; set; }

        [Required]
        [Display(Name = "Form Type Id")]
        public virtual int ValidationFormTypeId { get; set; }

        [Display(Name = "Related Questions")]
        [StringLength(500)]
        public virtual string RelatedQuestions { get; set; }

        [Required]
        [Display(Name = "Mappings")]
        [StringLength(500)]
        public virtual string Mappings { get; set; }

        [Required]
        [Display(Name = "View Order")]
        public virtual int ViewOrder { get; set; }
    }
}