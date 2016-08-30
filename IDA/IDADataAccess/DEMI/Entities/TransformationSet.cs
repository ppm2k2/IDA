using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.DEMI.Entities
{
    public class TransformationSet
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreateDateTime { get; set; }
        public string Name { get; set; }

        [StringLength(250)]
        public string UserName { get; set; }

        public virtual ICollection<Transformation> Transformations { get; set; }

    }
}
