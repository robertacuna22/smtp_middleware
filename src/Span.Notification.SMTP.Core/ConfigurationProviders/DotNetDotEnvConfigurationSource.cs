using Microsoft.Extensions.Configuration;

namespace Span.Notification.SMTP.Core.ConfigurationProviders
{
    public class DotNetDotEnvConfigurationSource : IConfigurationSource
    {
        private readonly FileInfo _envFile;
        private readonly bool _optional;

        public DotNetDotEnvConfigurationSource(FileInfo envFile, bool optional = false)
        {
            _envFile = envFile;
            _optional = optional;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DotNetDotEnvConfigurationProvider(_envFile, _optional);
        }
    }
}
