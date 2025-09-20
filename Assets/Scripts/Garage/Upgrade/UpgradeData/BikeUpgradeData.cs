using System;

[Serializable]
public class BikeUpgradeData : UpgradeData
{
    public int staminaLevel = 1;

    public BikeUpgradeData(TransportType transportType) : base(transportType) { }
}
