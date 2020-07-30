using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot
{
    public class CreateVMDetails
    {
        public string Location { get; set; }

        public string Size { get; set; }

        public string DiskSize { get; set; }

        public string Image { get; set; }

        public string Name { get; set; }
    }
}
