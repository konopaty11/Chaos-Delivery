using System;
using UnityEngine;

[Serializable]
public class UpgradeData 
{
    public TransportType transportType;
    public int powerLevel = 1;
    public int stiffnessLevel = 1;
    public int strengthLevel = 1;

    public UpgradeData(TransportType transportType)
    {
        this.transportType = transportType;
    }
}
