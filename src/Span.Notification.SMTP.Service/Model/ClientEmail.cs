using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Model
{
    public class ClientEmail
    {
        public int ClientsEmailId { get; set; }

        public string? MatchCd { get; set; }

        public string? CustomerId { get; set; }

        public string? EmailAddr { get; set; }

        public bool IsActive { get; set; }

        public string CustomerNm { get; set; }
    }
}
