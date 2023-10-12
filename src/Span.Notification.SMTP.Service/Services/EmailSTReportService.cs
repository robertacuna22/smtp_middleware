using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Span.Notification.SMTP.Core;
using Span.Notification.SMTP.Service.IContract;
using Span.Notification.SMTP.Service.Model;
using Span.Notification.SMTP.Service.Repository;
using Span.Notification.SMTP.Service.Repository.IContract;
using Span.Notification.SMTP.Service.Services.IContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Services
{
    public class EmailSTReportService : IEmailSTReportService
    {
        private readonly AppSettings _appSettings;
        private readonly IClientEmailRepository _clientEmailRepository;
        private readonly IEmailService _emailService;
        private readonly ILogFileService _logFileService;
        private readonly IMailerActivityRepository _mailerActivityRepository;

        public EmailSTReportService(IOptions<AppSettings> settings,
            IClientEmailRepository clientEmailRepository, IEmailService emailService,
            ILogFileService logFileService, IMailerActivityRepository mailerActivityRepository)
        {
            _appSettings = settings.Value;
            _clientEmailRepository = clientEmailRepository;
            _emailService = emailService;
            _logFileService = logFileService;
            _mailerActivityRepository = mailerActivityRepository;
        }

        public async Task Send()
        {
            await Task.Run(() =>
            {
                Console.WriteLine("Doing a process with validating the STR file.");

                var datePeriod = DateTime.Now.ToString("dd-MMM-yyyy");
                Console.WriteLine(datePeriod);

                var reportFileExtentionList = JsonConvert.DeserializeObject<List<string>>(_appSettings.FileExtension ?? "")
                ?? new List<string>();

                List<string> reportFiles = Directory.GetFiles(_appSettings.STRFileLocation ?? "", "*.*", SearchOption.AllDirectories)
                    .Where(file => reportFileExtentionList
                    .Contains(Path.GetExtension(file)) && file.Contains(datePeriod))
                    .ToList();

                if (!reportFiles.Any())
                {
                    Console.WriteLine("No File Found!");
                    return;
                }

                Console.WriteLine("Do the process of sending email per files.");
                reportFiles.ForEach(async file =>
                {
                    FileInfo fileInfo = new FileInfo(file);
                    Console.WriteLine(fileInfo.Name);

                    ReportFileDetail reportFileDetail = GetReportFileDetail(fileInfo.Name);

                    if (reportFileDetail == null) return;

                    var clientEmailAddList = GetClientsEmailAddress(reportFileDetail.CustomerId ?? string.Empty
                        , out var customerName, out List<string> invalidEmailList);

                    reportFileDetail.CustomerNm = customerName;

                    var logEmailActivity = GetMailerActivity(
                            JsonConvert.SerializeObject(clientEmailAddList), reportFileDetail);

                    if (clientEmailAddList.Any())
                        DoSendingReportViaEmail(logEmailActivity, fileInfo, clientEmailAddList, reportFileDetail);

                    if (invalidEmailList.Any())
                        LogsInvalidEmail(reportFileDetail, invalidEmailList);
                });
            });
        }

        private void LogsInvalidEmail(ReportFileDetail reportFileDetail, List<string> invalidEmails)
        {
            var logInvalidEmails = GetMailerActivity(
            JsonConvert.SerializeObject(invalidEmails), reportFileDetail);
            logInvalidEmails.status = "ERROR";
            logInvalidEmails.ret_msg = "Invalid Email!";
            logInvalidEmails.end_timestamp = Convert.ToDateTime(DateTime.Now.ToString());

            _mailerActivityRepository.Log(logInvalidEmails);  
        }

        private void DoSendingReportViaEmail(MailerActivity logActivity, FileInfo fileInfo, 
            List<string> recipients, ReportFileDetail fileDetail)
        {
            var execMessage = string.Empty;

            try
            {
                using (FileStream fs = fileInfo.OpenRead())
                {                     
                    var emailDetail = GetEmailDetail(fs, recipients, fileDetail);
                    var resultMsg = _emailService.Send(emailDetail, out bool isSuccess);
                    Console.WriteLine(execMessage);
                    fs.Close();

                    if (isSuccess)
                    {
                        LogsAfterEmailed(logActivity, fileInfo, resultMsg, "SUCCESS", true);
                        Console.WriteLine($"Email Success");
                    }
                    else
                    {
                        LogsAfterEmailed(logActivity, fileInfo, resultMsg, "ERROR", false);
                        Console.WriteLine($"Email Failed");
                    }             
                }
                
            }
            catch (Exception ex)
            {
                LogsAfterEmailed(logActivity, fileInfo, ex.Message, "ERROR", false);
            }
        }

        private void LogsAfterEmailed(MailerActivity logActivity, FileInfo fileInfo, 
            string errorMsg, string status, bool isSuccess)
        {
            LogFile(fileInfo, isSuccess);
            logActivity.ret_msg = errorMsg;
            logActivity.end_timestamp = Convert.ToDateTime(DateTime.Now.ToString());
            logActivity.status = status;
            _mailerActivityRepository.Log(logActivity);
        }

        private List<string> GetClientsEmailAddress(string customerId, out string customerName, 
            out List<string> invalidEmails)
        {
            customerName = string.Empty;
            var clientEmailAddList = new List<string>();
            var listOfInvalidEmail = new List<string>();    
            var clientEmaiInfolList = Task.Run(async () => await _clientEmailRepository.GetByCustomerId(new { customerId })).Result;

            if (clientEmaiInfolList.Any())
            {
                customerName = clientEmaiInfolList.FirstOrDefault().CustomerNm;
                clientEmaiInfolList
                    .ForEach(x => {

                        if (IsValidEmail(x.EmailAddr))
                        {
                           clientEmailAddList.Add(x.EmailAddr);
                        }else
                        {
                            Console.WriteLine($"Invalid email address {x.EmailAddr}");
                            listOfInvalidEmail.Add(x.EmailAddr);
                        }
                });
            }

            invalidEmails = listOfInvalidEmail;

            return clientEmailAddList;
        }

        private MailerActivity GetMailerActivity(string recipeints, ReportFileDetail reportFileDetail)
        {
            var mailerActivity = new MailerActivity()
            {
                start_timestamp = Convert.ToDateTime(DateTime.Now.ToString()),
                event_type = "EMAIL_NOTIF",
                report_period = reportFileDetail.ReportPeriod,
                report_type = reportFileDetail.ReportType,
                cust_id = reportFileDetail.CustomerId,
                recipients = recipeints,
                file_names = reportFileDetail.FileName
            };

            return mailerActivity;
        }

        private void LogFile(FileInfo fileInfo, bool isSuccess)
        {
            try
            {                
                if (isSuccess) _logFileService.SucessLog(fileInfo);
                else _logFileService.FailedLog(fileInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }           
        }
        private EmailDetail GetEmailDetail(FileStream fs, List<string> clientEmailAddList,
            ReportFileDetail fileDetail)
        {
            var generatedId = _mailerActivityRepository.GenerateId();

            var readyTheEmailDetail = new EmailDetail()
            {
                Subject = GetSmtpSubject(fileDetail),
                To = clientEmailAddList,
                Attachment = new Attachment(fs, fs.Name),
                Body = GetSmtpHtmlBody(fileDetail, generatedId.ToString())
            };

            return readyTheEmailDetail;
        }

        private string GetSmtpSubject(ReportFileDetail fileDetail)
        {
            if (string.IsNullOrEmpty(_appSettings.SMTPSubject))
                return string.Empty;

            var subject = _appSettings.SMTPSubject.Replace("@companyName", fileDetail.CustomerNm)
                .Replace("@reportType", fileDetail.ReportType)
                .Replace("@dateStamp", fileDetail.ReportPeriod);

            return subject;
        }

        private string GetSmtpHtmlBody(ReportFileDetail fileDetail, string generatedId)
        {
            if (string.IsNullOrEmpty(_appSettings.SMTPBodyTemplate))
                return string.Empty;

            var body = _appSettings.SMTPBodyTemplate
                .Replace("@companyName", fileDetail.CustomerNm)
                .Replace("@runId", generatedId.ToString())
                .Replace("@dateStamp", fileDetail.StartDateStamp)
                .Replace("@reportType", fileDetail.ReportType)
                .Replace("@noReplyEmailAdd", _appSettings.NoReplyEmailAdd);

            return body;
        }
        private ReportFileDetail GetReportFileDetail(string fileName)
        {
            var reportFileName = Path.GetFileNameWithoutExtension(fileName);
            var splitFileName = reportFileName.Split('_');

            if (splitFileName == null) return null;

            var reportFileDetail = new ReportFileDetail()
            {
                CustomerId = splitFileName[3],
                ReportPeriod = splitFileName[1],
                ReportType = splitFileName[0],
                FileName = fileName,
                StartDateStamp = DateTime.Now.ToString("dd-MMMM-yyyy HH:mm:ss")
            };

            return reportFileDetail;
        }

        //Temp
        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
