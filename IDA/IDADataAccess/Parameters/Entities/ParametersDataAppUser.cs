using System;
using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.Parameters.Entities
{
    public class ParametersDataAppUser
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(255)]
        public virtual string WindowsUsername { get; set; }

        [Required]
        [StringLength(50)]
        public virtual string Role { get; set; }

        [StringLength(255)]
        public virtual string TableEditLockOn { get; set; }

        public virtual Nullable<System.DateTime> TableEditLockUntil { get; set; }
    }
}
