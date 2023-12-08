using Algoriza2.Core.DTOs;
using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using Algoriza2.EF;
using Algoriza2.EF.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Api.Controllers
{

    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly IBaseRepository<Patient> _PatientRepository;
        private readonly IBaseRepository<AppointmentTime> _AppointmentTimeRepository;
        private readonly IBaseRepository<DiscountCodeCoupon> _DiscountCodeCouponRepository;
        private readonly DoctorService _DoctorService;
        private readonly BookingService _BookingService;
        private readonly Context _Context;
        public PatientController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IBaseRepository<Patient> patientRepository, IBaseRepository<Doctor> DoctorRepository,
            DoctorService DoctorService, BookingService BookingService, IBaseRepository<Booking> BookingRepository,
            IBaseRepository<AppointmentTime> AppointmentTimeRepository,
            IBaseRepository<DiscountCodeCoupon> DiscountCodeCouponRepository,
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
            _DiscountCodeCouponRepository = DiscountCodeCouponRepository;
            _Context = Context;
        }




        [HttpPost]
        [Route("api/[controller]/[action]")]
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
        [Route("api/[controller]/[action]")]
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

        //Search is optional
        public IActionResult GetAllDoctors(int Page, int PageSize, string Search)
        {
            var result = _DoctorService.GetDoctorsForPatient(Page, PageSize, Search);
            return Ok(result);
        }



        [HttpPost]
        [Route("api/[controller]/Booking/Add")]
        public IActionResult AddBooking(int PatientId, int TimeId, string Coupon)
        {

            Booking booking = new Booking();
            var appointmentTime = _Context.Set<AppointmentTime>()
                .Include(x => x.Appointment)
                .SingleOrDefault(x => x.Id == TimeId);
            if (appointmentTime == null)
            {
                return BadRequest("Invalid appointment time.");
            }

            int doctorId = appointmentTime.Appointment.DoctorId;
            var doctor = _DoctorRepository.GetById(doctorId);

            DiscountCodeCoupon discountCodeCoupon = null;
            if (Coupon != null)
            {
                discountCodeCoupon = _DiscountCodeCouponRepository.Find(x => x.Code == Coupon);
            }

            booking.Status = BookingStatus.Pending;
            booking.AppointmentTimeId = TimeId;
            booking.DoctorId = doctorId;
            booking.PatientId = PatientId;

            var numberOfCompletedBookings = _Context.Set<Booking>()
                .Where(x => x.PatientId == PatientId && x.Status == BookingStatus.Completed).Count();


            if (discountCodeCoupon != null && discountCodeCoupon.IsActive == true && discountCodeCoupon.NumOfCompletedBookings <= numberOfCompletedBookings)
            {
                booking.DiscountCodeCouponId = discountCodeCoupon.Id;

                if (discountCodeCoupon.DiscountType == DiscountType.Value)
                {
                    if (discountCodeCoupon.Value <= doctor.Price)
                    {
                        booking.FinalPrice = doctor.Price - discountCodeCoupon.Value;
                    }
                    else
                    {
                        booking.FinalPrice = 0;
                    }
                }
                else if (discountCodeCoupon.DiscountType == DiscountType.Percentage)
                {
                    var actualValue = (doctor.Price * discountCodeCoupon.Value) / 100;
                    booking.FinalPrice = doctor.Price - actualValue;

                }
            }
            else
            {
                booking.DiscountCodeCouponId = null;
                booking.FinalPrice = doctor.Price;
            }
            _BookingRepository.Add(booking);


            return Ok();
        }



        [HttpGet]
        [Route("api/[controller]/Search/Bookings/GetAll")]
        public IActionResult GetAllBookings(int PatientId)
        {
            var result = _BookingService.GetAllBookingsForPatient(PatientId);
            return Ok(result);
        }



        [HttpPost]
        [Route("api/[controller]/Cancelation/Booking/[action]")]
        public IActionResult Cancel(int id)
        {
            var result = _Context.Set<Booking>().FirstOrDefault(x => x.Id == id);
            if (result.Status == BookingStatus.Pending)
            {
                result.Status = BookingStatus.Canceled;
                _Context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

    }
}
