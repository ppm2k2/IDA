using System.ComponentModel.DataAnnotations;

namespace IDA.Models
{
    public class ValidationEnumerationViewModel
    {
        [Key]
        public virtual int Id { get; set; }

        [Display(Name = "Enumeration Id")]
        public virtual int? ValidationEnumerationId { get; set; }

        [Required]
        [Display(Name = "Value")]
        [StringLength(2000)]
        public virtual string Value { get; set; }

    }
}