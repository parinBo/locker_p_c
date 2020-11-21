using System;
namespace locker.Models
{
    public class showclass
    {
        public DateTime Bookingstart { get; set; }
        public DateTime BookingEnd { get; set; }
        public int Boxid { get; set; }
        public int Timeid { get; set; }
        public int Userid { get; set; }
        public string username { get; set; }
    }
}
