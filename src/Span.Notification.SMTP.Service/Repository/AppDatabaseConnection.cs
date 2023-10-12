using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Span.Notification.SMTP.Service.DataContext;
using Span.Notification.SMTP.Service.Repository.IContract;
using System.Data;

namespace Span.Notification.SMTP.Service.Repository
{
    public class AppDatabaseConnection : IDapperConnection
    {
        private string _databaseConnectionString;
        private string _databaseContentConnectionString;
        public IDbConnection Database { get
            {
                return new SqlConnection(_databaseConnectionString);
            }
        }
        public IDbConnection DatabaseContent { get
            {
                return new SqlConnection(_databaseContentConnectionString);
            }
        }

        public AppDatabaseConnection(ApplicationDBContext dbContext) 
        {
            _databaseConnectionString = dbContext.Database.GetConnectionString();
        }
    }
}