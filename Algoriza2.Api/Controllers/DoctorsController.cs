using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using Algoriza2.EF.Repositories;
using Algoriza2.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Algoriza2.Core.DTOs;
using System.Threading.Tasks;

namespace Algoriza2.Api.Controllers
{
    
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly IBaseRepository<Patient> _PatientRepository;
        private readonly IBaseRepository<AppointmentTime> _AppointmentTimeRepository;
        private readonly DoctorService _DoctorService;
        private readonly BookingService _BookingService;
        private readonly Context _Context;
        public DoctorsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IBaseRepository<Patient> patientRepository, IBaseRepository<Doctor> DoctorRepository,
            DoctorService DoctorService, BookingService BookingService, IBaseRepository<Booking> BookingRepository,
            IBaseRepository<AppointmentTime> AppointmentTimeRepository,
            Context Context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _DoctorRepository = DoctorRepository;
            _PatientRepository = patientRepository;
            _DoctorService = DoctorService;
            _BookingService = BookingService;
            _BookingRepository = BookingRepository;
            _AppointmentTimeRepository = AppointmentTimeRepository;
            _Context = Context;
        }



        [HttpPost]
        [Route("api/[controller]/Login/[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();

            }
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (!result.Succeeded)
            {
                return Unauthorized(model);
            }
            return Accepted();


        }







    }


    

}
