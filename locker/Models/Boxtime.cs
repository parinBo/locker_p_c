using System;
using System.Collections.Generic;

#nullable disable

namespace locker.Models
{
    public partial class Boxtime
    {
        public DateTime? Bookingstart { get; set; }
        public DateTime? BookingEnd { get; set; }
        public int? Boxid { get; set; }
        public int Timeid { get; set; }
        public int? Userid { get; set; }

    }
}
