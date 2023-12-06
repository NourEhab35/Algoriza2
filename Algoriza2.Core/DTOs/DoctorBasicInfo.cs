using Algoriza2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Core.DTOs
{
    public class DoctorBasicInfo
    {
        public string FullName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Specialization { set; get; }
        public int Price { get; set; }
        public Gender Gender { get; set; }
        public ICollection<Appointment> Appointments { set; get; }
        public ICollection<Booking> Bookings { set; get; }
        public ICollection<AppointmentTime> AppointmentTimes { set; get;}

        
    }
}
