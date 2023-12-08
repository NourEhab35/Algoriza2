using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using Algoriza2.EF.Repositories;
using Algoriza2.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Algoriza2.Core.DTOs;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Algoriza2.Api.Controllers
{

    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly IBaseRepository<Patient> _PatientRepository;
        private readonly IBaseRepository<AppointmentTime> _AppointmentTimeRepository;
        private readonly IBaseRepository<Appointment> _AppointmentRepository;
        private readonly DoctorService _DoctorService;
        private readonly BookingService _BookingService;
        private readonly Context _Context;
        public DoctorController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IBaseRepository<Patient> patientRepository, IBaseRepository<Doctor> DoctorRepository,
            DoctorService DoctorService, BookingService BookingService, IBaseRepository<Booking> BookingRepository,
            IBaseRepository<AppointmentTime> AppointmentTimeRepository,IBaseRepository<Appointment> AppointmentRepository,
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
            _AppointmentRepository = AppointmentRepository;
            _Context = Context;
        }



        [HttpPost]
        [Route("api/[controller]/[action]")]
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


        [HttpGet]
        [Route("api/[controller]/Bookings/GetAll")]
        public IActionResult GetAllBookings(int DoctorId)
        {
            var result=_BookingService.GetAllBookingsForDoctor(DoctorId);
            return Ok(result);
        }


        [HttpPost]
        [Route("api/[controller]/Booking/[action]")]
        public IActionResult ConfirmCheckUp(int BookingId)
        {
            var Booking = _BookingRepository.GetById(BookingId);
            if (Booking != null && Booking.Status==BookingStatus.Pending)
            {
                Booking.Status = BookingStatus.Completed;
                _Context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost]
        [Route("api/[controller]/Appointment/Add")]
        public IActionResult AddAppointment([FromBody] AddAppointmentModel addAppointmentModel)
        {
            foreach (var day in addAppointmentModel.Days)
            {
                Appointment NewAppointment = new Appointment();
                NewAppointment.DoctorId = addAppointmentModel.DoctorId;
                NewAppointment.Day= day;
                _AppointmentRepository.Add(NewAppointment);
                foreach(var time in addAppointmentModel.Times)
                {
                    int AppointmentId= _Context.Set<Appointment>().OrderByDescending(x => x.Id).FirstOrDefault().Id;
                    AppointmentTime NewAppointmentTime = new AppointmentTime();
                    NewAppointmentTime.FreeTime = time;
                    NewAppointmentTime.IsAvailable = true;

                    NewAppointmentTime.AppointmentId = AppointmentId;
                    _AppointmentTimeRepository.Add(NewAppointmentTime);

                }

            }
            return Ok();
        }


        [HttpPost]
        [Route("api/[controller]/Appointment/Update")]
        public IActionResult UpdateAppointment(int AppointmentTimeId, int NewTime)
        {
            var AppointmentTime = _AppointmentTimeRepository.GetById(AppointmentTimeId);
            if (AppointmentTime == null)
            {
                return BadRequest("Invalid Appointment Time ID");
            }
            if (AppointmentTime.IsAvailable == false)
            {
                return BadRequest("Booked time can not be updated");
            }
            AppointmentTime.FreeTime = NewTime;
            _Context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Route("api/[controller]/Appointment/Delete")]
        public IActionResult DeleteAppointment(int AppointmentTimeId)
        {
            var AppointmentTime = _AppointmentTimeRepository.GetById(AppointmentTimeId);
            if (AppointmentTime == null)
            {
                return BadRequest("Invalid Appointment Time ID");
            }
            if (AppointmentTime.IsAvailable == false)
            {
                return BadRequest("Booked Time Can not be deleted");
            }
            _AppointmentTimeRepository.Delete(AppointmentTime);
            return Ok();
        }



    }
}
