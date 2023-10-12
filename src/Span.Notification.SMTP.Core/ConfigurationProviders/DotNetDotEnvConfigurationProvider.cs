using Microsoft.Extensions.Configuration;

namespace Span.Notification.SMTP.Core.ConfigurationProviders
{
    public class DotNetDotEnvConfigurationProvider : ConfigurationProvider
    {
        private readonly FileInfo _file;
        private readonly bool _optional;

        public DotNetDotEnvConfigurationProvider(FileInfo file, bool optional = false)
        { 
            _file = file;
            _optional = optional;

            if (!file.Exists && optional)
            { 
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            else if (!file.Exists)
            {
                throw new FileNotFoundException("File not found", file.FullName);
            }
            else if (!file.Extension.Equals(".env", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid file", nameof(Path));
            }
        }

        public override void Load()
        {
            Load(_file);
        }

        internal void Load(FileInfo file)
        {
            if (_optional && !file.Exists) return;

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in File.ReadAllLines(file.FullName))
            { 
                var split = line.Split(new char[] { '=' }, 2);

                if (split.Length != 2) continue;

                var key = SanitizeKey(split[0]);

                if (string.IsNullOrEmpty(key)) continue;

                result[key.Trim()] = split[1].Trim();
            }

            Data = result;

        }

        private string SanitizeKey(string key)
        {
            return key.Replace("__", ":");
        }
    }
}
