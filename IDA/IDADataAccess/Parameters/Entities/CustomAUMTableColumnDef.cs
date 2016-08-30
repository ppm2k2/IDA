using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IDADataAccess.Parameters.Entities
{
    public class CustomAUMTableColumnDef
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string TableName { get; set; }

        [Required]
        [StringLength(200)]
        public virtual string ColumnName { get; set; }
    }
}
