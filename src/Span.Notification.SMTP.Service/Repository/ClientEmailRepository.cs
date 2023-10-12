using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Span.Notification.SMTP.Core.Extensions;
using Span.Notification.SMTP.Service.DataContext;
using Span.Notification.SMTP.Service.Model;
using Span.Notification.SMTP.Service.Repository.IContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Notification.SMTP.Service.Repository
{
    public class ClientEmailRepository : IClientEmailRepository
    {
        private readonly IDapperConnection _connection;
        public ClientEmailRepository(IDapperConnection connection)
        {
            _connection = connection; 
        }

        public async Task<List<ClientEmail>> GetByCustomerId(object param)
        {
            using (var db = _connection.Database.TryOpen())
            {
                var result = await db.QueryAsync<ClientEmail>("sp_GetClientsEmailByCustomerId", param, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }
    }
}
