using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class images
    {
        public string id { get; set; }
        public string ext { get; set; }
        public override string ToString()
        {
            return $"{id} {ext}";
        }
    }
}
