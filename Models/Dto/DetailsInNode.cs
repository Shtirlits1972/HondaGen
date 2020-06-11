using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class DetailsInNode
    {
        // список деталей в узле
        public string node_id { get; set; }
        public string name { get; set; }
        public List<Detail> parts { get; set; }
        public List<images> images { get; set; }
        public List<attributes> attributes { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
