using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Services.IContract
{
    public interface IEmailSTReportService
    {
        Task Send();
    }
}
