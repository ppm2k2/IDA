using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

namespace IDA.Models.Applications
{
    public class AppVersionsViewModel
    {
        public IEnumerable<EnvironmentViewModel> Environments { get; set; }
    }
}