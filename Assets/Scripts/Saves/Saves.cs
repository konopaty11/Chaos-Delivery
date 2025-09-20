using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public List<UpgradeData> upgrades;
        public List<BikeUpgradeData> bikeUpgrades;

        public Dictionary<TransportType, bool> UnlockTransports;

        public Plot currentPlot = Plot.None;
        public OrderType UnlockTypeOrder = OrderType.Default;

        public float level = 1;
    }
}
