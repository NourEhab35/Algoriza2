using Algoriza2.Core;
using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.EF.Repositories
{
    public class BookingService
    {
        private readonly IBaseRepository<Booking> _BookingRepository;
        public BookingService(IBaseRepository<Booking> BookingRepository)
        {
            _BookingRepository = BookingRepository;
            
        }

        public int NumOfBookings()
        {
            var NumOfTotalBookings = _BookingRepository.Count();
            return NumOfTotalBookings;

        }
        public int NumOfPendingBookings()
        {
            var NumOfPendingBookings = _BookingRepository.FindAll(x => x.Status == Enums.BookingStatus.Pending).Count();
            return NumOfPendingBookings;
        }
        public int NumOfCompletedBookings()
        {
            var NumOfCompletedBookings = _BookingRepository.FindAll(x => x.Status == Enums.BookingStatus.Completed).Count();
            return NumOfCompletedBookings;
        }
        public int NumOfCanceledBookings()
        {
            var NumOfCanceledBookings = _BookingRepository.FindAll(x => x.Status == Enums.BookingStatus.Canceled).Count();
            return NumOfCanceledBookings;
        }
    }
}
