using System.ComponentModel.DataAnnotations;
using IDALibrary.Enums;

namespace IDA.Models.Applications
{
    public class EnvironmentViewModel
    {
        [Required]
        public virtual Environment EnvironmentType { get; set; }

        [Required]
        [Display(Name = "Environment")]
        public virtual string EnvironmentTypeDesc { get; set; }

        [Required]
        [Display(Name = "Environment Path")]
        public virtual string EnvironmentPath { get; set; }

    }
}