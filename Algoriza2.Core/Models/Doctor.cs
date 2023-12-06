using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.Core.Models
{
    public class Doctor : Person
    {
        public string Specialization { set; get; }
        public int Price { set; get; }

        public ICollection<Appointment> Appointments { set; get; }
        public ICollection<Booking> Bookings { set; get; }

    }
}
