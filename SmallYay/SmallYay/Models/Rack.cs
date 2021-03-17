using System;
using System.Collections.Generic;
using System.Text;

namespace SmallYay.Models
{
    public class Rack
    {
        public string guid { get; set; }
        public long rows { get; set; }
        public long cols { get; set; }
        public string owner_guid { get; set; }
        public string rack_name { get; set; }
        public long bottleCount { get; set; }
    }
}
