using Microsoft.Extensions.Configuration;

namespace Span.Notification.SMTP.Core.ConfigurationProviders
{
    public class DotNetEnvironmentVariableConfigurationSource : IConfigurationSource
    {
        private readonly string _environmentVariablePrefix;

        public DotNetEnvironmentVariableConfigurationSource(string prefix = "Span_Notification")
        { 
            _environmentVariablePrefix = prefix;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DotNetEnvironmentVariableConfigurationProvider(_environmentVariablePrefix);
        }
    }
}
