using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA.Models
{
    public class Navigation
    {
        public List<ConceptMenuItem> MenuItems { get; set; }
        public string Name { get; set; }

        public Navigation()
        {
            MenuItems = new List<ConceptMenuItem>();
        }
    }

    public class ConceptMenuItem
    {
        public bool IsModule { get; set; }
        public List<ConceptMenuItem> MenuItems { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public ConceptMenuItem()
        {
            MenuItems = new List<ConceptMenuItem>();
        }
    }
}