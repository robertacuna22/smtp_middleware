using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Repository.IContract
{
    public interface IMailerActivityRepository
    {
        void Log(object param);

        int GenerateId();    
    }
}
