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
    public class PatientService
    {
        private readonly IBaseRepository<Patient> _PatientRepository;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly BookingService _BookingService;
        private readonly Context _Context;
        public PatientService(IBaseRepository<Patient> PatientRepository, IBaseRepository<Booking> BookingRepository,
            IBaseRepository<Doctor> DoctorRepository,BookingService BookingService , Context Context)
        {
            _PatientRepository = PatientRepository;
            _BookingRepository = BookingRepository;
            _DoctorRepository = DoctorRepository;
            _BookingService = BookingService;
            _Context = Context;
        }

        public IEnumerable<GetAllPatientsForAdminDTO> GetAllPatientsForAdmin(int Page, int PageSize, string search)
        {
                var AllPatients = _Context.Set<Patient>().ToList();

            IEnumerable<Patient> SearchApplied = null;
            IEnumerable<GetAllPatientsForAdminDTO> PatientsPerPage = null;

            if (search != null)
            {
                SearchApplied = AllPatients.Where(x => x.Email.Contains(search)
                || x.FirstName.Contains(search)
                || x.LastName.Contains(search)
                || x.PhoneNumber.Contains(search));

                PatientsPerPage = SearchApplied
                    .Select(x => new GetAllPatientsForAdminDTO
                    {
                        FullName = $"{x.FirstName} {x.LastName}",
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Gender = x.Gender,
                        DateOfBirth = x.DateOFBirth

                    }).ToList()
                      .Skip((Page - 1) * PageSize)
                      .Take(PageSize).ToList();

            }
            else
            {
                PatientsPerPage = AllPatients
                    .Select(x => new GetAllPatientsForAdminDTO
                    {
                        FullName = $"{x.FirstName} {x.LastName}",
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Gender = x.Gender,
                        DateOfBirth = x.DateOFBirth

                    }).ToList()
                      .Skip((Page - 1) * PageSize)
                      .Take(PageSize).ToList();
            }
            return PatientsPerPage;
        }

        public GetPatientByIdForAdminDTO GetPatientByIdForAdmin (int patientId)
        {

            var bookings = _BookingService.GetAllBookingsForPatient(patientId).ToList();
            var PatientById= _Context.Set<Patient>()
                .Where(x=>x.Id == patientId)
                .Select(x=> new GetPatientByIdForAdminDTO
                {
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Gender = x.Gender,
                    DateOfBirth = x.DateOFBirth,
                    Bookings = bookings
                }).FirstOrDefault();

            return PatientById;
        }

    }
}
