using Algoriza2.Core.Interfaces;
using Algoriza2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.EF.Repositories
{
    public class PatientService
    {
        private readonly IBaseRepository<Patient> _patientRepository;
        private readonly IBaseRepository<Booking> _BookingRepository;
        private readonly IBaseRepository<Doctor> _DoctorRepository;
        private readonly Context _Context;
        public PatientService(IBaseRepository<Patient> PatientRepository, IBaseRepository<Booking> BookingRepository,
            IBaseRepository<Doctor> DoctorRepository, Context Context)
        {
            _patientRepository = PatientRepository;
            _BookingRepository = BookingRepository;
            _DoctorRepository = DoctorRepository;
            _Context = Context;
        }
        //public IEnumerable<Booking> GetBookings(int id)
        //{ 
        //    var result = _BookingRepository.GetAll(x=>x.PatientId==id);
        //    return result;
        //}



    }
}
