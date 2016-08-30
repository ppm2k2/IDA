using System.ComponentModel.DataAnnotations;

namespace IDA.Models
{
    public class ValidationTableColumnDefViewModel
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [Display(Name = "Table Name")]
        public virtual string TableName { get; set; }

        [Required]
        [Display(Name = "Column Name")]
        public virtual string ColumnName { get; set; }
    }
}