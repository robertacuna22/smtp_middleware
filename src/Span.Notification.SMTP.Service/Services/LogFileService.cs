using Microsoft.Extensions.Options;
using Span.Notification.SMTP.Core;
using Span.Notification.SMTP.Service.Services.IContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Services
{
    public class LogFileService : ILogFileService
    {
        private readonly AppSettings _appSettings;

        public LogFileService(IOptions<AppSettings> settings)
        {
            _appSettings = settings.Value;

        }
        public void FailedLog(FileInfo reportFile)
        {
            var directoryPath = (string.IsNullOrEmpty(_appSettings.SentFileLogLocation) ?
                Directory.GetCurrentDirectory() : _appSettings.SentFileLogLocation);
            
            var fullPath = Path.Combine(directoryPath,
                                _appSettings.SentFailedFileName??"SentFailed");

            bool exists = Directory.Exists(fullPath);   

            if (!exists) Directory.CreateDirectory(fullPath);

            reportFile.MoveTo($@"{fullPath}\{reportFile.Name}", true);
          
        }

        public void SucessLog(FileInfo reportFile)
        {

            var directoryPath = (string.IsNullOrEmpty(_appSettings.SentFileLogLocation) ?
              Directory.GetCurrentDirectory() : _appSettings.SentFileLogLocation);

            var fullPath = Path.Combine(directoryPath,
                                _appSettings.SentSuccessFileName??"SentSuccess");

            bool exists = Directory.Exists(fullPath);

            if (!exists) Directory.CreateDirectory(fullPath);

            reportFile.MoveTo($@"{fullPath}\{reportFile.Name}", true);
 
        }
    }
}
