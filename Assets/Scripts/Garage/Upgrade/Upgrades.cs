using System;
using UnityEngine;
using UnityEngine.UI;

public class Upgrades : MonoBehaviour
{
    [SerializeField] protected TransportType transportType;

    [Header("Upgrade Color")]
    [SerializeField] protected Color upgradeColor;

    [Header("Upgrades")]
    [SerializeField] RectTransform powerScale;
    [SerializeField] RectTransform stiffnessScale;
    [SerializeField] RectTransform strenghtScale;
    
    [Header("Visual Upgrades")]
    [SerializeField] RectTransform visualPowerScale;
    [SerializeField] RectTransform visualStiffnessScale;
    [SerializeField] RectTransform visualStrenghtScale;

    UpgradeData upgradeData;
    public virtual UpgradeData UpgradeData 
    { 
        get => upgradeData;
        set { if (value != null) upgradeData = value; }
    }

    protected const int maxLevel = 5;
    protected readonly Color baseColor = Color.white;

    protected virtual void Awake()
    {
        upgradeData = new(transportType);
        SetPowerScale();
        SetStiffnessScale();
        SetStrenghtScale();
    }

    public virtual void UpgradesUpdate()
    {
        SetPowerScale();
        SetStiffnessScale();
        SetStrenghtScale();
    }

    public void PowerUpgrade()
    {
        if (UpgradeData.powerLevel >= maxLevel || 
            !BuyUpgrade(GetPricePower(GarageManager.Instance.CurrentTransportType))) return;

        UpgradeData.powerLevel++;
        SetPowerScale();

        GarageManager.Instance.DecidePriceUpgrades();
    }

    public void StiffnessUpgrade()
    {
        if (UpgradeData.stiffnessLevel >= maxLevel || 
            !BuyUpgrade(GetPriceStiffness(GarageManager.Instance.CurrentTransportType))) return;

        UpgradeData.stiffnessLevel++;
        SetStiffnessScale();

        GarageManager.Instance.DecidePriceUpgrades();
    }

    public void StrenghtUpgrade()
    {
        if (UpgradeData.strengthLevel >= maxLevel || 
            !BuyUpgrade(GetPriceStrenght(GarageManager.Instance.CurrentTransportType))) return;

        UpgradeData.strengthLevel++;
        SetStrenghtScale();

        GarageManager.Instance.DecidePriceUpgrades();
    }

    void SetPowerScale()
    {
        SetScale(powerScale, UpgradeData.powerLevel);
        SetScale(visualPowerScale, UpgradeData.powerLevel);
    }

    void SetStiffnessScale()
    {
        SetScale(stiffnessScale, UpgradeData.stiffnessLevel);
        SetScale(visualStiffnessScale, UpgradeData.stiffnessLevel);
    }

    void SetStrenghtScale()
    {
        SetScale(strenghtScale, UpgradeData.strengthLevel);
        SetScale(visualStrenghtScale, UpgradeData.strengthLevel);
    }

    public int GetPricePower(TransportType type)
    {
        return Mathf.RoundToInt(TransportManager.Instance.TransportPrices[type] * (0.035f + UpgradeData.powerLevel * 0.015f));
    }

    public int GetPriceStiffness(TransportType type)
    {
        return Mathf.RoundToInt(TransportManager.Instance.TransportPrices[type] * (0.035f + UpgradeData.stiffnessLevel * 0.015f));
    }

    public int GetPriceStrenght(TransportType type)
    {
        return Mathf.RoundToInt(TransportManager.Instance.TransportPrices[type] * (0.035f + UpgradeData.strengthLevel * 0.015f));
    }

    protected bool BuyUpgrade(int _price)
    {
        if (CoinsManager.Instance.Coins - _price < 0)
        {
            PromptManager.Instance.NotEnoughCoins();
            return false;
        }

        CoinsManager.Instance.Coins -= _price;
        return true;
    }

    protected void SetScale(RectTransform scale, int level)
    {
        for (int i = 0; i < maxLevel; i++)
        {
            Image cell = scale.GetChild(i).GetComponent<Image>();
            if (i < level)
                cell.color = upgradeColor;
            else
                cell.color = baseColor;
        }
    }
}
