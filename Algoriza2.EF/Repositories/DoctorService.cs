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
        public IEnumerable<GetAllDoctorsForAdminDTO> GetAllDoctorsForAdmin(int Page, int PageSize, string Search)
        {
            var AllDoctors = _context.Set<Doctor>().ToList();


            IEnumerable<Doctor> SearchApplied = null;
            IEnumerable<GetAllDoctorsForAdminDTO> DoctorsPerPage = null;
            if (Search != null)
            {

                SearchApplied = AllDoctors.Where(x => x.Email.Contains(Search)
                     || x.FirstName.Contains(Search)
                     || x.LastName.Contains(Search)
                     || x.Specialization.Contains(Search)
                     || x.PhoneNumber.Contains(Search));

                DoctorsPerPage = SearchApplied
                    .Select(x => new GetAllDoctorsForAdminDTO
                    {
                        FullName = $"{x.FirstName} {x.LastName}",
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Specialization = x.Specialization,
                        Gender = x.Gender,

                    }).ToList()
                      .Skip((Page - 1) * PageSize)
                      .Take(PageSize).ToList();
            }
            else
            {
                DoctorsPerPage = AllDoctors
                        .Select(x => new GetAllDoctorsForAdminDTO
                        {
                            FullName = $"{x.FirstName} {x.LastName}",
                            Email = x.Email,
                            PhoneNumber = x.PhoneNumber,
                            Specialization = x.Specialization,
                            Gender = x.Gender,

                        }).ToList()
                      .Skip((Page - 1) * PageSize)
                      .Take(PageSize).ToList();
            }


            return DoctorsPerPage;
        }



        public IEnumerable<GetAllDoctorsForPatientDTO> GetAllDoctorsForPatient(int Page, int PageSize, string Search)
        {
            var AllDoctorsWithAppointmentsAndTimes = _context.Set<Doctor>()
                .Include(x => x.Appointments).Include(x => x.Bookings).ToList();

            IEnumerable<Doctor> SearchApplied = null;
            IEnumerable<GetAllDoctorsForPatientDTO> DoctorsPerPage = null;

            if (Search != null)
            {

                SearchApplied = AllDoctorsWithAppointmentsAndTimes.Where(x => x.Email.Contains(Search)
                     || x.FirstName.Contains(Search)
                     || x.LastName.Contains(Search)
                     || x.Specialization.Contains(Search)
                     || x.PhoneNumber.Contains(Search));


                DoctorsPerPage = SearchApplied
                .Select(x => new GetAllDoctorsForPatientDTO
                {
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    Phone = x.PhoneNumber,
                    Specialization = x.Specialization,
                    Price = x.Price,
                    Gender = x.Gender,
                    //Appointments = x.Appointments,
                    AppointmentTimes = _context.Set<AppointmentTime>().Where(at => at.Appointment.Doctor.Id == x.Id && at.IsAvailable == true).ToList()
                }).ToList()
                  .Skip((Page - 1) * PageSize)
                  .Take(PageSize).ToList();

            }
            else
            {
                DoctorsPerPage = AllDoctorsWithAppointmentsAndTimes
                .Select(x => new GetAllDoctorsForPatientDTO
                {
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    Phone = x.PhoneNumber,
                    Specialization = x.Specialization,
                    Price = x.Price,
                    Gender = x.Gender,
                    //Appointments = x.Appointments,
                    AppointmentTimes = _context.Set<AppointmentTime>().Where(at => at.Appointment.Doctor.Id == x.Id && at.IsAvailable == true).ToList()
                }).ToList()
                  .Skip((Page - 1) * PageSize)
                  .Take(PageSize).ToList();
            }
            return DoctorsPerPage;
        }

        public GetDoctorByIdForAdminDTO GetDoctorByIdForAdmin(int DoctorId)
        {
            var DoctorById = _context.Set<Doctor>()
                .Where(x => x.Id == DoctorId)
                .Select(x => new GetDoctorByIdForAdminDTO
                {
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Specialization = x.Specialization,
                    Gender = x.Gender,
                    DateOfBirth = x.DateOFBirth
                }).FirstOrDefault();
            return DoctorById;
        }

    }



}

