using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.Core.DTOs
{
    public class NumberOfBookingsDTO
    {
        public int NumberOfBookings { get; set; }
        public int NumberOfPendingBookings { get; set; }
        public int NumberOfCompletedBookings { get; set; }
        public int NumberOfCanceledBookings { get; set; }

    }
}
