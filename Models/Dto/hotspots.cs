using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class hotspots
    {
        public int recordid { get; set; }
        public string illustrationnumber { get; set; }
        public int partreferencenumber { get; set; }
        public int max_x { get; set; }
        public int max_y { get; set; }
        public int min_x { get; set; }
        public int min_y { get; set; }
        public string npl { get; set; }

        public override string ToString()
        {
            return $"{illustrationnumber} {partreferencenumber}";
        }

    }
}
