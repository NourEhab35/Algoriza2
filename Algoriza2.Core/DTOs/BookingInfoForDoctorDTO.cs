using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Core.DTOs
{
    public class BookingInfoForDoctorDTO
    {
        public string PatientName { get; set; }
       // public int Age { get; set; }
        public Gender Gender { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public Days Day { get; set; }
        public int Time { get; set; }    
    }
}
