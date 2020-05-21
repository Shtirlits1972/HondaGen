using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaGen.Models.Dto
{
    public class PartsGroup
    {
        public string Id { get; set; }
        public string xplgrp { get; set; }

        public override string ToString()
        {
            return xplgrp;
        }
    }
}
