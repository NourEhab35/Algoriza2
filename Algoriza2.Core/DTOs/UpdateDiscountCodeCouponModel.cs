using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Algoriza2.Core.Enums;

namespace Algoriza2.Core.DTOs
{
    public class UpdateDiscountCodeCouponModel
    {
        public int Id { get; set; }
        public string Code { set; get; }

       
        public int NumOfCompletedBookings { set; get; }
        public DiscountType DiscountType { set; get; }

        public int Value { get; set; }
    }
}
