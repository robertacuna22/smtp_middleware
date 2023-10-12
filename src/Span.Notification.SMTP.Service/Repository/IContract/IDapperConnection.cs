using System.Data;

namespace Span.Notification.SMTP.Service.Repository.IContract
{
    public interface IDapperConnection
    {
        IDbConnection Database { get; }
        IDbConnection DatabaseContent { get; }

    }
}
