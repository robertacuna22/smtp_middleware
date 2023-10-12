
using Microsoft.EntityFrameworkCore;

namespace Span.Notification.SMTP.Service.DataContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
        }


    }
}
