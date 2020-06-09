using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class node
    {
        public string code { get; set; }
        public string name { get; set; }
        public string node_ids { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
}
