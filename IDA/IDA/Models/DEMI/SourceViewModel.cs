using System.ComponentModel.DataAnnotations;

namespace IDA.Models.DEMI
{
    public class SourceViewModel
    {
        [Required]
        [Display(Name = "Source Columns")]
        public virtual string ColumnName { get; set; }
    }
}