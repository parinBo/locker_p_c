using System;
using System.ComponentModel.DataAnnotations;

namespace locker.Models
{
    public class Booking
    {
        public DateTime startdate { get; set; }
        public string starttime { get; set; }
        public DateTime enddate { get; set; }
        public string endtime { get; set; }
    }
}
