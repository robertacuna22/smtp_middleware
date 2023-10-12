using Span.Notification.SMTP.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Repository.IContract
{
    public interface IClientEmailRepository
    {
       Task<List<ClientEmail>> GetByCustomerId(object param);
    }
}
