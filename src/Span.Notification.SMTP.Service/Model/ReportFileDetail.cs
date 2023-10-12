using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Model
{
    public class ReportFileDetail
    {
        public string? CustomerId { get; set; }
        public string? ReportPeriod { get; set; }
        public string? ReportType { get; set; }
        public string? FileName { get; set; }
        public string? CustomerNm { get; set; }
        public string? StartDateStamp { get; set; } 
    }
}
