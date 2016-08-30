using System;
using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.DEMI.Entities
{
    public class Transformation
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int TransformationSetId { get; set; }
        public string TargetColumn { get; set; }
        public string TransformationRule { get; set; }
    }
}
