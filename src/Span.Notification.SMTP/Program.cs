using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Span.Notification.SMTP.Core;
using Span.Notification.SMTP.Service.IContract;
using Span.Notification.SMTP.Service;
using Span.Notification.SMTP.Core.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Span.Notification.SMTP.Service.DataContext;
using Span.Notification.SMTP.Service.Services.IContract;
using Span.Notification.SMTP.Service.Services;
using Span.Notification.SMTP.Service.Repository.IContract;
using Span.Notification.SMTP.Service.Repository;

namespace Span.Notification.SMTP
{

    class Program
    {
        private static readonly IServiceProvider serviceProvider;
        private static IConfigurationRoot configuration;
        private static AppSettings appSetting = null;

        static Program()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureService(serviceCollection);
            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start the process.");
            var emailReportService = serviceProvider.GetService<IEmailSTReportService>();

            await emailReportService.Send();

            Console.WriteLine("End process.");
            Console.ReadKey();
        }

        private static void ConfigureService(IServiceCollection services)
        {
            var configuratioBuilder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", true)
              .AddDotNetDotEnvVariables(optional: true) //<-- Read a .env file and load it as a config eg Randem__ConnectionString=<connectionString>
              .AddDotNetEnvironmentVariables("Span_Notification__");

            configuration = configuratioBuilder.Build();

            services.AddOptions();
            services.Configure<AppSettings>(configuration.GetSection("Span_Notification"));

            var serviceProvider = services.BuildServiceProvider();
            appSetting = serviceProvider.GetService<IOptions<AppSettings>>().Value;

            //db connection
            services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(appSetting.ConnectionString));

            //middleware services
            services.AddTransient<IDapperConnection, AppDatabaseConnection>()
                .AddTransient<IEmailService, EmailService>()
                .AddTransient<IEmailSTReportService, EmailSTReportService>()
                .AddTransient<ILogFileService, LogFileService>();

            //repository
            services.AddTransient<IClientEmailRepository, ClientEmailRepository>()
                .AddTransient<IMailerActivityRepository, MailerActivityRepository>();
        }

    }

}