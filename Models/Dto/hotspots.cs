using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class hotspots
    {
        public string hotspot_id { get; set; }
        public string image_id { get; set; }
        public int x1 { get; set; }
        public int y1 { get; set; }
        public int x2 { get; set; }
        public int y2 { get; set; }

        public override string ToString()
        {
            return $"{hotspot_id}-{image_id}";
        }
    }
}
