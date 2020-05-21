using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class Filters
    {
        public string Id { get; set; }
        public string name { get; set; }

        public List<values> values { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
