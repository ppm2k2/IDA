using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.Parameters.Entities
{
    public class CustomAUMMapping
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual string Client { get; set; }

        [Required]
        public virtual string Portfolio { get; set; }

        [StringLength(500)]
        public virtual string AUM { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string EffectiveDate { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string IsStatic { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string sysDate { get; set; }

        [Required]
        [StringLength(500)]
        public virtual string ModifiedBy { get; set; }

    }
}
