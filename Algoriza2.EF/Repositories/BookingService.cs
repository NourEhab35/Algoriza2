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
        public BookingService(IBaseRepository<Booking> BookingRepository, Context context)
        {
            _BookingRepository = BookingRepository;
            _context = context;
        }

        public int NumberOfBookings()
        {
            var NumberOfTotalBookings = _BookingRepository.Count();
            return NumberOfTotalBookings;
        }
        public int NumberOfPendingBookings()
        {
            var NumberOfPendingBookings = _BookingRepository.FindAll(x => x.Status == Enums.BookingStatus.Pending).Count();
            return NumberOfPendingBookings;
        }
        public int NumberOfCompletedBookings()
        {
            var NumberOfCompletedBookings = _BookingRepository.FindAll(x => x.Status == Enums.BookingStatus.Completed).Count();
            return NumberOfCompletedBookings;
        }
        public int NumberOfCanceledBookings()
        {
            var NumberOfCanceledBookings = _BookingRepository.FindAll(x => x.Status == Enums.BookingStatus.Canceled).Count();
            return NumberOfCanceledBookings;
        }

        public IEnumerable<BookingInfoDTO> GetAllBookingsForPatient(int PatientId)
        {
            var bookingsForPatient = _context.Set<Booking>()
                .Include(x => x.Doctor)
                .Include(x => x.DiscountCodeCoupon)
                .Include(x => x.AppointmentTime)
                .ThenInclude(x => x.Appointment)
                .Where(x => x.PatientId == PatientId);

            var selectedProperties = bookingsForPatient.Select(x => new BookingInfoDTO
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
            return selectedProperties;
        }

        public IEnumerable<BookingInfoForDoctorDTO> GetAllBookingsForDoctor(int DoctorId)
        {
            var bookingsForDoctor = _context.Set<Booking>()
                .Include(x => x.Patient)
                .Include(x => x.AppointmentTime)
                .ThenInclude(x => x.Appointment)
                .Where(x => x.DoctorId == DoctorId);
                 
            var selectedBookings = bookingsForDoctor.Select(x => new BookingInfoForDoctorDTO
            {
                PatientName = $"{x.Patient.FirstName} {x.Patient.LastName}",
                Gender = x.Patient.Gender,
                Age=CalculateAge(x.Patient.DateOFBirth),
                Phone = x.Patient.PhoneNumber,
                Email = x.Patient.Email,
                Day = x.AppointmentTime.Appointment.Day,
                Time = x.AppointmentTime.FreeTime
            });
            return selectedBookings;
        }
        public static int CalculateAge(DateTime dateOfBirth)
        {
            DateTime currentDate = new DateTime(2023, 12, 10);
            int age = currentDate.Year - dateOfBirth.Year;

            // Check if the birthday has occurred this year already
            if (dateOfBirth.Date > currentDate.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
