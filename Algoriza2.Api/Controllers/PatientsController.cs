using Algoriza2.Core.DTOs;
using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using Algoriza2.EF;
using Algoriza2.EF.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Api.Controllers
{

    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly IBaseRepository<Patient> _PatientRepository;
        private readonly DoctorService _DoctorService;
        private readonly Context _Context;
        public PatientsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IBaseRepository<Patient> patientRepository, IBaseRepository<Doctor> DoctorRepository,
            DoctorService DoctorService, IBaseRepository<Booking> BookingRepository, Context Context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _DoctorRepository = DoctorRepository;
            _PatientRepository = patientRepository;
            _DoctorService = DoctorService;
            _BookingRepository = BookingRepository;
            _Context = Context;
        }

        [HttpPost]
        [Route("api/[controller]/Registration/[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var Result = await _userManager.CreateAsync(user, model.Password);


            if (!Result.Succeeded)
            {
                return BadRequest(Result);
            }

            Patient patient = new Patient();
            patient.FirstName = model.FirstName;
            patient.LastName = model.LastName;
            patient.Email = model.Email;
            patient.Password = user.PasswordHash;
            patient.PhoneNumber = model.PhoneNumber;
            patient.Gender = model.Gender;
            patient.DateOFBirth = model.DateOfBirth;
            _PatientRepository.Add(patient);
            return Ok();

        }



        [HttpPost]
        [Route("api/[controller]/Login/[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (!result.Succeeded)
            {
                return Unauthorized(model);
            }
            return Accepted();


        }


        [HttpGet]
        [Route("api/[controller]/Search/Doctors/GetAll")]
        public IActionResult GetAllDoctors(int Page, int PageSize)
        {
            var result = _DoctorService.GetDoctorsForPatient(Page, PageSize);
            return Ok(result);
        }

        //[HttpGet]
        //[Route("api/[controller]/Search/Bookings/GetAll")]
        //public IActionResult GetAllBookings(int PatientId)
        //{
        //    var result = _DoctorService.GetBooking(PatientId);

        //    return Ok(result);
        //}

        [HttpGet]
        [Route("api/[controller]/Cancelation/Bookings/[action]")]
        public IActionResult Cancel(int id)
        {
            var result = _Context.Set<Booking>().Include(x=>x.AppointmentTime).FirstOrDefault(x => x.Id == id);
            result.Status = BookingStatus.Canceled;
            result.AppointmentTime.IsAvailable = true;
            _Context.SaveChanges();
            return Ok(true);
        }

    }
}
