using System;
using System.Collections.Generic;

#nullable disable

namespace locker.Models
{
    public partial class User
    {
        public int Userid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? Status { get; set; }
        public int? Has { get; set; }
    }
}
