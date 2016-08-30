using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace IDA.Models
{
    public class RiskReportDataTableColumnDefViewModel
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [Display(Name = "Client")]
        public virtual string Client { get; set; }

        [Required]
        [Display(Name = "Portfolio")]
        public virtual string Portfolio { get; set; }

        [Required]
        [Display(Name = "AUM")]
        public virtual string AUM { get; set; }

        [Required]
        [Display(Name = "EffectiveDate")]
        public virtual string EffectiveDate { get; set; }

        [Required]
        [Display(Name = "IsStatic")]
        public virtual string IsStatic { get; set; }

        [Required]
        [Display(Name = "sysDate")]
        public virtual string sysDate { get; set; }

        [Required]
        [Display(Name = "ModifiedBy")]
        public virtual string ModifiedBy { get; set; }

    }
}