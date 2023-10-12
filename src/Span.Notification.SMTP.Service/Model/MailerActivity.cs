using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Model
{
    public class MailerActivity
    {
        public int id { get; set; } = 0;
        public string? event_type { get; set; }
        public string? report_period { get; set; }
        public string? report_type { get; set; }
        public string? cust_id { get; set; }
        public string? recipients { get; set; }
        public string? sender { get; set; }
        public string? sms_msg { get; set; }
        public DateTime start_timestamp { get; set; }
        public DateTime end_timestamp { get; set; }
        public string? file_names { get; set; }
        public string? status { get; set; }       
        public string? ret_msg { get; set; }
    }
}
