using Microsoft.Extensions.Configuration;

namespace Span.Notification.SMTP.Core.ConfigurationProviders
{
    public class DotNetEnvironmentVariableConfigurationProvider : ConfigurationProvider
    {
        private readonly string _envVarPrefix;

        public DotNetEnvironmentVariableConfigurationProvider(string prefix = "Span_Notification")
        { 
            _envVarPrefix = prefix;
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public override void Load()
        {
            var envVariables = Environment.GetEnvironmentVariables();
            var tempData = envVariables.Keys.Cast<string>().ToDictionary(k => k, k => (string?)envVariables[k]);

            if (!string.IsNullOrEmpty(_envVarPrefix)) 
            {
               tempData = envVariables.Keys.Cast<string>().Where(x => x.StartsWith(_envVarPrefix)).ToDictionary(k => k, k => (string?)envVariables[k]);
            }

            foreach (var data in tempData) 
            {
                var key = SanitizeKey(data.Key);

                Data.Add(key, data.Value);
            }
        }

        private string SanitizeKey(string key)
        { 
            return key.Replace("__", ":");
        }
    }
}
