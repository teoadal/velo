using Velo.Settings;

namespace Velo.TestsModels.Settings
{
    [Settings("Logging.LogLevel")]
    public sealed class LogLevelSettings
    {
        public string Default { get; set; }

        public string Microsoft { get; set; }

        public string System { get; set; }
    }
}