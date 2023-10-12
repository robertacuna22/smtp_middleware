using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Span.Notification.SMTP.Core.Extensions;
using Span.Notification.SMTP.Service.DataContext;
using Span.Notification.SMTP.Service.Model;
using Span.Notification.SMTP.Service.Repository.IContract;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Span.Notification.SMTP.Service.Repository
{
    public class MailerActivityRepository : IMailerActivityRepository
    {
        private readonly IDapperConnection _connection;
        public MailerActivityRepository(IDapperConnection connection)
        {
            _connection = connection;
        }

        public int GenerateId()
        {
            using (var db = _connection.Database.TryOpen())
            {
                var result = db.Query<MailerActivity>("sp_GetMailerActivityId",
                                commandType: CommandType.StoredProcedure).FirstOrDefault();

                return result == null ? 0 : result.id;
            }
        }

        public void Log(object param)
        {
            using (var db = _connection.Database.TryOpen())
            {
                db.Execute("sp_logEvent", param, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
