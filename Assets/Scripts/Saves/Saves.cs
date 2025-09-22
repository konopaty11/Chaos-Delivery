using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public List<UpgradeData> upgrades;
        public List<BikeUpgradeData> bikeUpgrades;

        public List<TransportType> types;
        public List<bool> unlock;

        public Plot currentPlot = Plot.None;
        public OrderType unlockTypeOrder = OrderType.Default;
        public TransportType currentTransport = TransportType.Bike;

        public float level = 1;
        public int coins;
    }
}
