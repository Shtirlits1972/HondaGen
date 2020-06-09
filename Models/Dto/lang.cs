using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class lang
    {
        public string code { get; set; }
        public string name { get; set; }
        public bool is_default { get; set; }
        public override string ToString()
        {
            return $"{code} {name} {is_default}";
        }
    }
}
