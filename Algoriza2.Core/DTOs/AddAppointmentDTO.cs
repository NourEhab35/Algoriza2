using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Core.DTOs
{
    public class AddAppointmentDTO
    {
        public int DoctorId { get; set; }
        public List<Days> Days { get; set; }
        public List<int> Times { get; set; }
    }
}
