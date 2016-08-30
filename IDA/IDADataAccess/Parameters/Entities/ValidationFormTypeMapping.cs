using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.Parameters.Entities
{
    public class ValidationFormTypeMapping
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual int ValidationTableColumnDefId { get; set; }

        [Required]
        public virtual int ValidationFormTypeId { get; set; }

        [StringLength(500)]
        public virtual string RelatedQuestions { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string Mappings { get; set; }

        [Required]
        public virtual int ViewOrder { get; set; }
    }
}
