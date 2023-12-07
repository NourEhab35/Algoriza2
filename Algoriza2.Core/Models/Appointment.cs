using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Core.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public Days Day { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

    }
}
