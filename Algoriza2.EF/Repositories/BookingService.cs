using Algoriza2.Core;
using Algoriza2.Core.DTOs;
using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.EF.Repositories
{
    public class BookingService
    {
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly Context _context;
        public BookingService(IBaseRepository<Booking> BookingRepository,Context context)
        {
            _BookingRepository = BookingRepository;
            _context = context;
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

        public IEnumerable<BookingInfo> GetAllBookingForPatient(int PatientId) 
        {
            var bookingsForPatient = _context.Set<Booking>()
                .Include(x=>x.Doctor)
                .Include(x=>x.DiscountCodeCoupon)
                .Include(x=>x.AppointmentTime)
                .ThenInclude(x=>x.Appointment).Where(x=>x.PatientId == PatientId);
            var selected = bookingsForPatient.Select(x => new BookingInfo
            {
                DoctorName = $"{x.Doctor.FirstName} {x.Doctor.LastName}",
                Specialization = x.Doctor.Specialization,
                Day = x.AppointmentTime.Appointment.Day,
                Time = x.AppointmentTime.FreeTime,
                Price = x.Doctor.Price,
                DiscountCode = x.DiscountCodeCoupon.Code,
                FinalPrice = x.FinalPrice,
                BookingStatus = x.Status

            });
            return selected;
        }
    }
}
