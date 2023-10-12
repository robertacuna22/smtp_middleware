using System.Data;

namespace Span.Notification.SMTP.Core.Extensions
{ 
    public static class DatabaseExtension
    {
        public static IDbConnection TryOpen(this IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
            
            return connection;
        }
    }
}
