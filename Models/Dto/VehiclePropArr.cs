using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class VehiclePropArr
    {
        public string model_name { get; set; }
        public List<attributes> attributes { get; set; }
        public override string ToString()
        {
            return model_name;
        }
    }
}
