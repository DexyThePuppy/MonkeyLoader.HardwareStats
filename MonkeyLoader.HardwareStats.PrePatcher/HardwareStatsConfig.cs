using MonkeyLoader.Configuration;
using System;

namespace MonkeyLoader.HardwareStats
{
    internal sealed class HardwareStatsConfig : ConfigSection
    {
        private readonly DefiningConfigKey<int> _updateIntervalKey = new("UpdateInterval", "Update interval in milliseconds.", () => 1000);

        public override string Description => "Configuration for hardware stats monitoring.";

        public override string Id => "HardwareStats";

        public int UpdateInterval
        {
            get => _updateIntervalKey.GetValue();
            set => _updateIntervalKey.SetValue(value);
        }

        public override Version Version { get; } = new Version(1, 0, 0);
    }
}