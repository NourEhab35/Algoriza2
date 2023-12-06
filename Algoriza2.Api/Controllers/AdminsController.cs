using Algoriza2.Core;
using Algoriza2.Core.DTOs;
using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using Algoriza2.EF;
using Algoriza2.EF.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Algoriza2.Api.Controllers
{

    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly IBaseRepository<Patient> _PatientRepository;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly DoctorService _DoctorService;
        private readonly PatientService _PatientService;
        private readonly BookingService _BookingService;
        private readonly Context _Context;
        public AdminsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IBaseRepository<Patient> patientRepository, IBaseRepository<Doctor> DoctorRepository, IBaseRepository<Booking> BookingRepository,
            DoctorService DoctorService, BookingService BookingService, Context Context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _PatientRepository = patientRepository;
            _DoctorRepository = DoctorRepository;
            _BookingRepository = BookingRepository;
            _DoctorService = DoctorService;
            _BookingService = BookingService;
            _Context = Context;

        }
        [HttpGet]
        [Route("api/[controller]/Dashboard/[action]")]
        public int NumOfDoctors()
        {
            return _DoctorRepository.Count();

        }

        [HttpGet]
        [Route("api/[controller]/Dashboard/[action]")]
        public int NumOfPatients()
        {
            return _PatientRepository.Count();
        }

        [HttpGet]
        [Route("api/[controller]/Dashboard/[action]")]
        public IActionResult NumOfBookings()
        {
            NumberOfBookingsDTO numberOfBookingsDTO = new NumberOfBookingsDTO();
            numberOfBookingsDTO.NumberOfBookings = _BookingService.NumOfBookings();
            numberOfBookingsDTO.NumberOfPendingBookings = _BookingService.NumOfPendingBookings();
            numberOfBookingsDTO.NumberOfCompletedBookings = _BookingService.NumOfCompletedBookings();
            numberOfBookingsDTO.NumberOfCanceledBookings = _BookingService.NumOfCanceledBookings();
            return Ok(numberOfBookingsDTO);

        }

        [HttpGet]
        [Route("api/[controller]/Dashboard/[action]")]
        public IActionResult Top5Specializations()
        {
            var top5Specializations = _Context.Set<Doctor>().Include(x=>x.Bookings).ToList()
             
        .GroupBy(d => d.Specialization)
        .Select(g => new SpecializationBookingDTO
        {
            SpecializationName = g.Key,
            NumberOfBookings = g.Sum(d => d.Bookings.Count)
        })
        .OrderByDescending(dto => dto.NumberOfBookings)
        .Take(5)
        .ToList();

            return Ok(top5Specializations);
        }

        [HttpGet]
        [Route("api/[controller]/Dashboard/[action]")]
        public IActionResult Top10Doctors()
        {
            var top10Doctors = _Context.Set<Doctor>()
                .Include(d => d.Bookings)
                .ToList()
                .Select(d => new
                {
                    FullName = $"{d.FirstName} {d.LastName}",
                    Specialization = d.Specialization,
                    NumberOfBookings = d.Bookings.Count
                })
                .OrderByDescending(dto => dto.NumberOfBookings)
                .Take(10);

            return Ok(top10Doctors);
        }


        //[HttpGet]
        //[Route("api/[controller]/Search/Doctors/[action]")]
        //public IActionResult GetAll(int Page, int PageSize)
        //{
        //    var result = _DoctorService.GetDoctorsForAdmin(Page, PageSize);
        //    return Ok(result);
        //}



    }
}
