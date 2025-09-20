using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using YG;
using System;
using UnityEngine.UI;
using TMPro;
using static Cinemachine.DocumentationSortingAttribute;
using static System.Net.Mime.MediaTypeNames;

public class GarageManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] List<GameObject> garageUI;
    [SerializeField] Button BuySelectBtn;
    [SerializeField] TextMeshProUGUI BuySelectText;
    [SerializeField] List<TextMeshProUGUI> powerUpgradeTexts;
    [SerializeField] List<TextMeshProUGUI> strenghtUpgradeTexts;
    [SerializeField] List<TextMeshProUGUI> stiffnessUpgradeTexts;
    [SerializeField] TextMeshProUGUI staminaUpgradeText;

    [Header("Portal")]
    [SerializeField] PortalController portalController;

    [Header("Transport")]
    [SerializeField] List<GameObject> transports;
    [SerializeField] List<GameObject> bikes;
    [SerializeField] Transform spawn;

    [Header("Upgrades")]
    [SerializeField] GameObject bikeUpgrades;
    [SerializeField] GameObject upgrades;
    [SerializeField] RectTransform parentUpgrades;

    [Header("Visual Upgrades")]
    [SerializeField] GameObject bikeVisualUpgrades;
    [SerializeField] GameObject visualUpgrades;
    [SerializeField] RectTransform parentVisualUpgrades;

    public static GarageManager Instance { get; private set; }
    public List<Upgrades> UpgradesTransport { get; private set; } = new();
    public List<UpgradeData> upgradeDatas = new();
    public Transform Spawn => spawn;
    public TransportType CurrentTransportType => currentTransportType;

    const float visibleY = 0f;
    const float invisibleY = -10f;
    int currentIndex;
    TransportType currentTransportType;
    List<GameObject> allTransports = new();
    bool isWorkStarted = false;

    public static event Action OnInitialized;

    string transportUnlock = "Выбрать и выйти";
    string transportLock = "Купить за ";

    void Awake()
    {
        portalController.Listener = Proceed;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        foreach (GameObject elem in bikes)
            allTransports.Add(elem);

        foreach (GameObject elem in transports)
            allTransports.Add(elem);

        InitilizadeUpgrades();
    }

    public void InvokeSubscribers() => OnInitialized?.Invoke();

    void InitilizadeUpgrades()
    {
        //if (YG2.saves.upgrades != null && YG2.saves.upgrades.Count > 0 &&
        //    YG2.saves.bikeUpgrades != null && YG2.saves.bikeUpgrades.Count > 0)
        //{
        //    upgradeDatas.AddRange(YG2.saves.bikeUpgrades);
        //    for (int i = 0; i < upgradeDatas.Count; i++)
        //    {
        //        BikeUpgrades bikeUpgrade = bikes[i].GetComponent<BikeUpgrades>();
        //        bikeUpgrade.UpgradeData = upgradeDatas[i];
        //        UpgradesTransport.Add(bikeUpgrade);
        //    }

        //    for (int i = 0; i < YG2.saves.upgrades.Count; i++)
        //    {
        //        Upgrades upgrade = transports[i].GetComponent<Upgrades>();
        //        upgrade.UpgradeData = YG2.saves.upgrades[i];
        //        UpgradesTransport.Add(upgrade);
        //    }

        //    upgradeDatas.AddRange(YG2.saves.upgrades);
        //}
        //else
        //{
            foreach (GameObject transport in allTransports)
            {
                Upgrades upgrade = transport.GetComponent<BikeUpgrades>();
                if (upgrade == null)
                    upgrade = transport.GetComponent<Upgrades>();
                UpgradesTransport.Add(upgrade);

                upgradeDatas.Add(upgrade.UpgradeData);
            }
        //}
    }

    public void Proceed()
    {
        if (OrderManager.Instance.IsWorking == true)
        {
            OrderManager.Instance.StopGenerate();
            isWorkStarted = true;
        }
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        UIManager.Instance.HideUI("Main");
        UIManager.Instance.ShowUI("LoadScreen");
        if (!OrderManager.Instance.IsWorking) UIManager.Instance.HideUI("Start work");
        if (OrderManager.Instance.CurrentOrderType == OrderType.TimeLimited) UIManager.Instance.HideUI("Timer");

        foreach (GameObject ui in garageUI) ui.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        TransportManager.Instance.CurrentTransport.SetActive(false);
        CameraManager.Instance.TransitionCamera(CameraType.GarageCamera);
        UIManager.Instance.ShowUI("Garage");
        UIManager.Instance.ShowUI("Coins");
        SetCurrentTransport();

        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.HideUI("LoadScreen");
    }

    void SetCurrentTransport()
    {
        currentTransportType = TransportManager.Instance.CurrentTransportController.TransportType;
        DecidePriceUpgrades();

        for (int i = 0; i < upgradeDatas.Count; i++)
        {
            if (upgradeDatas[i].transportType == currentTransportType)
            {
                allTransports[i].transform.localPosition = new Vector3(0, visibleY, 0);
                currentIndex = i;
                ArrowTransperency(currentIndex);
                ShowUpgrades(currentTransportType);
                UpgradesTransport[i].UpgradesUpdate();

                BuySelectText.text = transportUnlock;
                BuySelectBtn.onClick.AddListener(SelectAndExitGarage);
            }
        }
    }

    public void DecidePriceUpgrades()
    {
        int priceTransport = TransportManager.Instance.TransportPrices[currentTransportType];

        foreach (TextMeshProUGUI text in powerUpgradeTexts)
            text.text = UpgradesTransport[currentIndex].GetPricePower(currentTransportType).ToString();

        foreach (TextMeshProUGUI text in strenghtUpgradeTexts)
            text.text = UpgradesTransport[currentIndex].GetPriceStrenght(currentTransportType).ToString();

        foreach (TextMeshProUGUI text in stiffnessUpgradeTexts)
            text.text = UpgradesTransport[currentIndex].GetPriceStiffness(currentTransportType).ToString();

        if (UpgradesTransport[currentIndex] is BikeUpgrades bikeUpgrades)
            staminaUpgradeText.text = UpgradesTransport[currentIndex].GetPriceStrenght(currentTransportType).ToString();
    }

    public void ScrollTransportLeft()
    {
        ScrollTransport(-1);
    }

    public void ScrollTransportRight()
    {
        ScrollTransport(1);
    }

    void ScrollTransport(int scroll)
    {
        int futureIndex = currentIndex + scroll;
        if (futureIndex < 0 || futureIndex >= allTransports.Count) return;

        ArrowTransperency(futureIndex);

        allTransports[currentIndex].transform.localPosition = new Vector3(0, invisibleY, 0);
        allTransports[futureIndex].transform.localPosition = new Vector3(0, visibleY, 0);

        currentIndex = futureIndex;
        currentTransportType = upgradeDatas[currentIndex].transportType;
        ShowUpgrades(currentTransportType);
        UpgradesTransport[currentIndex].UpgradesUpdate();

        BuySelectBtn.onClick.RemoveAllListeners();
        if (TransportManager.Instance.TransportUnlock[currentTransportType])
        {
            BuySelectText.text = transportUnlock;
            BuySelectBtn.onClick.AddListener(SelectAndExitGarage);
        }
        else
        {
            BuySelectText.text = transportLock + TransportManager.Instance.TransportPrices[currentTransportType];
            BuySelectBtn.onClick.AddListener(BuyTransport);
        }

        DecidePriceUpgrades();
    }

    void ArrowTransperency(int index)
    {
        if (index == 0)
        {
            UIManager.Instance.HideUI("Arrow L");
            UIManager.Instance.ShowUI("Arrow R");
        }
        else if (index == allTransports.Count - 1)
        {
            UIManager.Instance.HideUI("Arrow R");
            UIManager.Instance.ShowUI("Arrow L");
        }
        else
        {
            UIManager.Instance.ShowUI("Arrow L");
            UIManager.Instance.ShowUI("Arrow R");
        }
    }


    void ShowUpgrades(TransportType transportType)
    {
        if (transportType == TransportType.Bike)
        {
            bikeUpgrades.SetActive(true);
            bikeVisualUpgrades.SetActive(true);
            upgrades.SetActive(false);
            visualUpgrades.SetActive(false);
        }
        else
        {
            bikeUpgrades.SetActive(false);
            bikeVisualUpgrades.SetActive(false);
            upgrades.SetActive(true);
            visualUpgrades.SetActive(true);
        }
    }

    public void PowerUpgrade() => UpgradesTransport[currentIndex].PowerUpgrade();

    public void StiffnessUpgrade() => UpgradesTransport[currentIndex].StiffnessUpgrade();

    public void StrenghtUpgrade() => UpgradesTransport[currentIndex].StrenghtUpgrade();

    public void StaminaUpgrade()
    {
        if (UpgradesTransport[currentIndex] is BikeUpgrades bikeUpgrades)
            bikeUpgrades.StaminaUpgrade();
    }

    

    public void UseUpgrades()
    {
        List<UpgradeData> upgrades = new();
        List<BikeUpgradeData> bikeUpgrades = new();

        foreach (UpgradeData upgrade in upgradeDatas)
        {
            if (upgrade is BikeUpgradeData bikeUpgrade)
                bikeUpgrades.Add(bikeUpgrade);
            else
                upgrades.Add(upgrade);
        }
        YG2.saves.upgrades = upgrades;
        YG2.saves.bikeUpgrades = bikeUpgrades;

        YG2.SaveProgress();
        UIManager.Instance.HideUI("Upgrade");
        UIManager.Instance.ShowUI("Virtual Upgrade");
    }

    public void ExitGarage()
    {
        if (isWorkStarted == true)
        {
            OrderManager.Instance.StartGenerate();
            isWorkStarted = false;
        }
        StartCoroutine(Exit());
    }

    void SelectAndExitGarage()
    {
        List<TransportController> controllers = TransportManager.Instance.TransportControllers;
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i].TransportType == currentTransportType)
                TransportManager.Instance.CurrentTransport = TransportManager.Instance.Transports[i];
        }

        ExitGarage();
    }

    IEnumerator Exit()
    {
        UIManager.Instance.ShowUI("LoadScreen");
        UIManager.Instance.HideUI("Coins");
        
        yield return new WaitForSeconds(0.5f);

        allTransports[currentIndex].transform.localPosition = new Vector3(0, invisibleY, 0);
        UIManager.Instance.HideUI("Garage");
       
        TransportManager.Instance.CurrentTransport.SetActive(true);
        TransportManager.Instance.CurrentTransport.GetComponent<Rigidbody>().Move(spawn.position, spawn.rotation);
        CameraManager.Instance.TransitionCamera(CameraType.PlayerCamera);

        foreach (GameObject ui in garageUI) ui.SetActive(false);

        if (!OrderManager.Instance.IsWorking) UIManager.Instance.ShowUI("Start work");
        if (OrderManager.Instance.CurrentOrderType == OrderType.TimeLimited) UIManager.Instance.ShowUI("Timer");
        UIManager.Instance.ShowUI("Main");

        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.HideUI("LoadScreen");
        OnInitialized?.Invoke();

        portalController.BlockControl(false);
    }

    void BuyTransport()
    {
        int price = TransportManager.Instance.TransportPrices[currentTransportType];
        if (CoinsManager.Instance.Coins - price < 0)
            PromptManager.Instance.NotEnoughCoins();
        else
        {
            TransportManager.Instance.TransportUnlock[currentTransportType] = true;
            CoinsManager.Instance.Coins -= price;
            PromptManager.Instance.ShowPrompt("Вы купили новый транспорт");

            TransportManager.Instance.SaveUnlockTransport();
        }
    }

    public void ShowUpgrades()
    {
        if (TransportManager.Instance.TransportUnlock[currentTransportType])
        {
            UIManager.Instance.ShowUI("Upgrade");
            UIManager.Instance.ShowUI("Visual Upgrade");
        }
        else
            PromptManager.Instance.ShowPrompt("Приобретите транспорт, прежде чем улучшать его");
    }

    public void TransportEnableControl(bool enable, TransportType targetType)
    {
        for (int i = 0; i < upgradeDatas.Count; i++)
            if (upgradeDatas[i].transportType == targetType)
            {
                allTransports[i].transform.localPosition = new Vector3(0, enable ? visibleY : invisibleY, 0);
            }
    }
}
