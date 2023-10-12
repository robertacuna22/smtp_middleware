namespace Span.Notification.SMTP.Core
{
    public class AppSettings
    {
        public string? ConnectionString { get; set; }
        public string? SMTPHost { get; set; }
        public string? SMTPMailFromAddress { get; set; }
        public string? SMTPUserName { get; set; }
        public string? SMTPPassword { get; set; }
        public int SMTPPort { get; set; }
        public string? SMTPSubject { get; set; }
        public string? SMTPBodyTemplate { get; set; }
        public bool IsSMTPHtmBody { get; set; }
        public bool SMTPEnableSSL { get; set; }
        public string? STRFileLocation { get; set; }
        public string? SentSuccessFileName { get; set; }
        public string? SentFailedFileName { get; set; }
        public string? FileExtension { get; set; }
        public string? NoReplyEmailAdd { get; set; }
        public string? SentFileLogLocation { get; set; }
 
    }
}
