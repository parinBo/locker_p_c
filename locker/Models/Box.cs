using System;
using System.Collections.Generic;

#nullable disable

namespace locker.Models
{
    public partial class Box
    {
        public int Boxid { get; set; }
        public int? Boxstatus { get; set; }
        public int? BoxCheck { get; set; }
        public int? Pin { get; set; }
        public int? Userid { get; set; }
    }
}
