using System;
using System.ComponentModel.DataAnnotations;

namespace IDA.Models.DEMI
{
    public class TransformationSetViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Set Name")]
        public string Name { get; set; }
        
        public virtual DateTime Create_Date_Time { get; set; }

        [Display(Name = "User name")]
        public string UserName { get; set; }

    }
}