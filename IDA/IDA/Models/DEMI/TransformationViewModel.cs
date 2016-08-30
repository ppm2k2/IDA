using System;
using System.ComponentModel.DataAnnotations;

namespace IDA.Models.DEMI
{
    public class TransformationViewModel
    {
        [Key]
        public int Id { get; set; }

        public int TransformationSetId { get; set; }
                
        [Required]
        [Display(Name = "Target Column")]
        public string TargetColumn { get; set; }

        [Display(Name = "Transformation Rule")]
        public string TransformationRule { get; set; }

        public virtual DateTime Create_Date_Time { get; set; }
        public virtual DateTime Update_Date_Time { get; set; }
    }
}