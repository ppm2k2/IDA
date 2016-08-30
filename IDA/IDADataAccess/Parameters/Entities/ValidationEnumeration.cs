using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDADataAccess.Parameters.Entities
{
    public class ValidationEnumeration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        public virtual int? ValidationEnumerationId { get; set; }

        [Required]
        [StringLength(2000)]
        public virtual string Value { get; set; }
    }
}
