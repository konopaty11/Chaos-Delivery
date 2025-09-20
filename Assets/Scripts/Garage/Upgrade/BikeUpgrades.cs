using System;
using UnityEngine;
using UnityEngine.UI;

public class BikeUpgrades : Upgrades
{
    [Header("Bike Upgrades")]
    [SerializeField] RectTransform staminaScale;

    [Header("Visual Bike Upgrades")]
    [SerializeField] RectTransform visualStaminaScale;

    BikeUpgradeData bikeUpgradeData;
    public override UpgradeData UpgradeData
    {
        get => bikeUpgradeData;
        set { 
            if (value != null && value is BikeUpgradeData bikeData) 
                bikeUpgradeData = bikeData; 
        }
    }

    protected override void Awake()
    {
        bikeUpgradeData = new(transportType);
        SetStaminaScale();
    }

    public void StaminaUpgrade()
    {
        if (bikeUpgradeData.staminaLevel >= maxLevel || 
            !BuyUpgrade(GetPriceStamina(GarageManager.Instance.CurrentTransportType))) return;

        bikeUpgradeData.staminaLevel++;
        SetStaminaScale();

        GarageManager.Instance.DecidePriceUpgrades();
    }

    public int GetPriceStamina(TransportType type)
    {
        return Mathf.RoundToInt(TransportManager.Instance.TransportPrices[type] * (0.035f + bikeUpgradeData.staminaLevel * 0.015f));
    }

    public sealed override void UpgradesUpdate()
    {
        base.UpgradesUpdate();
        SetStaminaScale();
    }

    void SetStaminaScale()
    {
        for (int i = 0; i < bikeUpgradeData.staminaLevel; i++)
            staminaScale.GetChild(i).GetComponent<Image>().color = upgradeColor;

        for (int i = 0; i < bikeUpgradeData.staminaLevel; i++)
            visualStaminaScale.GetChild(i).GetComponent<Image>().color = upgradeColor;
    }
}
