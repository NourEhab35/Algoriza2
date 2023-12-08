using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Core.DTOs
{
    public class BookingInfoForPatient
    {
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public Days Day { get; set; }
        public int Time { get; set; }
        public int Price { get; set; }
        public string DiscountCode { get; set; }
        public decimal FinalPrice { get; set; }
        public BookingStatus BookingStatus { get; set; }

    }
}
