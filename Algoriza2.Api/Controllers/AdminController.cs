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
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly IBaseRepository<Patient> _PatientRepository;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<Appointment> _AppointmentRepository;
        private readonly IBaseRepository<AppointmentTime> _AppointmentTimeRepository;
        private readonly IBaseRepository<DiscountCodeCoupon> _DiscountCodeCouponRepository;
        private readonly DoctorService _DoctorService;
        private readonly PatientService _PatientService;
        private readonly BookingService _BookingService;
        private readonly Context _Context;
        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IBaseRepository<Patient> patientRepository, IBaseRepository<Doctor> DoctorRepository, IBaseRepository<Booking> BookingRepository,
            IBaseRepository<DiscountCodeCoupon> DiscountCodeCouponRepository,
            IBaseRepository<Appointment> AppointmentRepository,IBaseRepository<AppointmentTime>AppointmentTimeRepository,DoctorService DoctorService, BookingService BookingService,
            PatientService PatientService, Context Context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _PatientRepository = patientRepository;
            _DoctorRepository = DoctorRepository;
            _BookingRepository = BookingRepository;
            _AppointmentRepository = AppointmentRepository;
            _AppointmentTimeRepository = AppointmentTimeRepository;
            _DiscountCodeCouponRepository = DiscountCodeCouponRepository;
            _DoctorService = DoctorService;
            _PatientService = PatientService;
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
            var top5Specializations = _Context.Set<Doctor>().Include(x => x.Bookings).ToList()

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


        [HttpGet]
        [Route("api/[controller]/Doctors/GetAll")]
        public IActionResult GetAllDoctors(int Page, int PageSize, string Search)
        {
            var Doctors = _DoctorService.GetAllDoctorsForAdmin(Page, PageSize, Search);
            return Ok(Doctors);
        }

        [HttpGet]
        [Route("api/[controller]/Doctor/GetById")]
        public IActionResult GetDoctorById(int DoctorId)
        {
            var Doctor = _DoctorService.GetDoctorByIdForAdmin(DoctorId);
            return Ok(Doctor);
        }


        [HttpPost]
        [Route("api/[controller]/Doctor/Add")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorDTO model)
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

            Doctor doctor = new Doctor();
            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.Email = model.Email;
            doctor.Password = user.PasswordHash;
            doctor.PhoneNumber = model.PhoneNumber;
            doctor.Specialization = model.Specialization;
            doctor.Gender = model.Gender;
            doctor.DateOFBirth = model.DateOfBirth;
            _DoctorRepository.Add(doctor);
            return Ok();
        }

        [HttpPost]
        [Route("api/[controller]/Doctor/update")]
        public async Task<IActionResult> UpdateDoctor([FromBody] UpdateDoctorDTO model)
        {
            var doctor = _DoctorRepository.GetById(model.Id);
            if(doctor == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(doctor.Email);
            doctor.FirstName = model.FirstName;
            doctor.LastName = model.LastName;
            doctor.Email = model.Email;
            doctor.PhoneNumber = model.PhoneNumber;
            doctor.Specialization = model.Specialization;
            doctor.Gender = model.Gender;
            doctor.DateOFBirth= model.DateOfBirth;
            user.Email= model.Email;
            user.UserName = model.Email;
            await _userManager.UpdateAsync(user);
            _Context.SaveChanges();

            return Ok();

        }


            [HttpPost]
        [Route("api/[controller]/Doctor/Delete")]
        public async Task<IActionResult> DeleteDoctorAsync(int DoctorId)
        {
            var doctor = _DoctorRepository.GetById(DoctorId);
           
            if (doctor == null)
            {
                return BadRequest();
            }

            var bookings=_Context.Set<Booking>().Any(x=>x.DoctorId == DoctorId);

            if(bookings ==true)
            {
                return BadRequest("This doctor has bookings, and cannot be deleted");
            }

            var appointments = _AppointmentRepository.FindAll(x=>x.DoctorId == DoctorId);
            if (appointments != null)
            {
                IEnumerable <AppointmentTime> appointmentTime = null;
                appointmentTime = _AppointmentTimeRepository.GetAll();
               foreach (var appointment in appointments)
                {
                    foreach (var time in appointmentTime)
                    {
                        if(time.AppointmentId==appointment.Id)
                        {
                            _AppointmentTimeRepository.Delete(time);
                        }
                    }
                    
                        _AppointmentRepository.Delete(appointment);
                }
            }
            var user = await _userManager.FindByEmailAsync(doctor.Email);
            await _userManager.DeleteAsync(user);
            _DoctorRepository.Delete(doctor);
            return Ok();
        }


        [HttpGet]
        [Route("api/[controller]/Patients/GetAll")]
        //Search is optional
        public IActionResult GetAllPatients(int Page, int PageSize, string Search)
        {
            var Patients = _PatientService.GetAllPatientsForAdmin(Page, PageSize, Search);
            return Ok(Patients);
        }


        [HttpGet]
        [Route("api/[controller]/Patient/GetById")]
        public IActionResult GetPatientById(int PatientId)
        {
            var Patient = _PatientService.GetPatientByIdForAdmin(PatientId);
            return Ok(Patient);
        }


        [HttpPost]
        [Route("api/[controller]/DiscountCodeCoupon/Add")]
        public IActionResult AddDiscountCodeCoupon(AddDiscountCodeCouponDTO discountCodeCouponModel)
        {

            DiscountCodeCoupon NewDiscountCodeCoupon = new DiscountCodeCoupon();
            NewDiscountCodeCoupon.Code = discountCodeCouponModel.Code;
            NewDiscountCodeCoupon.IsActive = discountCodeCouponModel.IsActive;
            NewDiscountCodeCoupon.NumOfCompletedBookings = discountCodeCouponModel.NumOfCompletedBookings;
            NewDiscountCodeCoupon.DiscountType = discountCodeCouponModel.DiscountType;
            NewDiscountCodeCoupon.Value = discountCodeCouponModel.Value;
            _DiscountCodeCouponRepository.Add(NewDiscountCodeCoupon);

            return Ok();
        }

        [HttpPost]
        [Route("api/[controller]/DiscountCodeCoupon/Update")]
        public IActionResult UpdateDiscountCodeCoupon(UpdateDiscountCodeCouponDTO discountCodeCouponModel)
        {
            var discountCodeCoupon = _DiscountCodeCouponRepository.GetById(discountCodeCouponModel.Id);
            if (discountCodeCoupon == null)
            {
                return BadRequest("Invalid Discount Code Coupon ID");
            }
            var Bookings = _BookingRepository.Find(x => x.DiscountCodeCouponId == discountCodeCoupon.Id);
            if (Bookings != null)
            {
                return BadRequest("Used discount code coupon can not be updated");
            }
            discountCodeCoupon.Code = discountCodeCouponModel.Code;
            discountCodeCoupon.NumOfCompletedBookings = discountCodeCouponModel.NumOfCompletedBookings;
            discountCodeCoupon.DiscountType = discountCodeCouponModel.DiscountType;
            discountCodeCoupon.Value = discountCodeCouponModel.Value;
            _Context.SaveChanges();

            return Ok();
        }


        [HttpPost]
        [Route("api/[controller]/DiscountCodeCoupon/Delete")]
        public IActionResult DeleteDiscountCodeCoupon(int DiscountCodeCouponId)
        {
            var discountCodeCoupon = _DiscountCodeCouponRepository.GetById(DiscountCodeCouponId);
            if (discountCodeCoupon == null)
            {
                return BadRequest("Invalid Discount Code Coupon ID");
            }
            _DiscountCodeCouponRepository.Delete(discountCodeCoupon);

            return Ok();
        }



        [HttpPost]
        [Route("api/[controller]/DiscountCodeCoupon/Deactivate")]
        public IActionResult DeactivateDiscountCodeCoupon(int DiscountCodeCouponId)
        {
            var discountCodeCoupon = _DiscountCodeCouponRepository.GetById(DiscountCodeCouponId);
            if (discountCodeCoupon == null)
            {
                return BadRequest("Invalid Discount Code Coupon ID");
            }
            discountCodeCoupon.IsActive = false;
            _Context.SaveChanges();
            return Ok();
        }


    }
}
