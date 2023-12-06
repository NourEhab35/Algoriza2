using Algoriza2.Core.DTOs;
using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.EF.Repositories
{
    public class DoctorService
    {
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<AppointmentTime> _AppointmentTimeRepository;
        private readonly Context _context;
        public DoctorService(IBaseRepository<Doctor> DoctorRepository, IBaseRepository<Booking> BookingRepository,
           IBaseRepository<AppointmentTime> AppointmentTimeRepository, Context context)
        {
            _DoctorRepository = DoctorRepository;
            _BookingRepository = BookingRepository;
            _AppointmentTimeRepository = AppointmentTimeRepository;
            _context = context;
        }
        public IEnumerable<Doctor> GetDoctorsForAdmin(int Page, int PageSize)
        {

            var DoctorsPerPage = _DoctorRepository.GetAll()
                .Skip((Page - 1) * PageSize)
                .Take(PageSize);
            return DoctorsPerPage;
        }
        public IEnumerable<DoctorBasicInfo> GetDoctorsForPatient(int Page, int PageSize)
        {
            var DoctorsWithAppointmentsAndTimes = _context.Set<Doctor>()
                .Include(x => x.Appointments);
            // ThenInclude(x => x.AppointmentTime).ToList();
            var DoctorsPerPage = DoctorsWithAppointmentsAndTimes
                .Select(x => new DoctorBasicInfo
                {
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    Phone = x.PhoneNumber,
                    Specialization = x.Specialization,
                    Price = x.Price,
                    Gender = x.Gender,
                    Appointments = x.Appointments,
                    AppointmentTimes = _context.Set<AppointmentTime>().Where(at=>at.Appointment.Doctor.Id == x.Id && at.IsAvailable==true).ToList()
                }).ToList()
                  .Skip((Page - 1) * PageSize)
                  .Take(PageSize).ToList();
            return DoctorsPerPage;
        }

    }

  


    //public IEnumerable<Booking> GetBooking(int id)
    //{
    //    var result = _context.Set<Booking>().Where(x => x.PatientId == id).Include(d => d.Doctor.FirstName);

    //    return result;
    //}

}

