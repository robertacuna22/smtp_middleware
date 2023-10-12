using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.IContract
{
    public interface IEmailService
    {
        string Send(EmailDetail emailDetail, out bool isSuccess);
    }
}
