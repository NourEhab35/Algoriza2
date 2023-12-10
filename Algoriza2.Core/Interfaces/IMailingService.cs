using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoriza2.Core.Interfaces
{
    public interface IMailingService
    {
        Task SendEmailAsync(string mailTO, string subject, string body);
    }
}
