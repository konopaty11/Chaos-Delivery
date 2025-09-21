using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YG;

public class TransportManager : MonoBehaviour
{
    [Header("DestroySettings")]
    [SerializeField] TextMeshProUGUI payText;

    [Header("Transports")]
    [SerializeField] List<TransportController> transportControllers;
    [SerializeField] List<GameObject> transportPrefabs;

    [Header("TransportComponents")]
    [SerializeField] Slider steerSlider;
    [SerializeField] Slider motorSlider;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] GameObject stamina;
    [SerializeField] RectTransform stage1Stamina;
    [SerializeField] RectTransform stage2Stamina;

    [Header("Auto Repair Shop")]
    [SerializeField] List<PortalController> autoRepairPortals;

    public static event UnityAction DestroyTransport;

    public Slider SteerSlider => steerSlider;
    public Slider MotorSlider => motorSlider;
    public CinemachineVirtualCamera VirtualCamera => virtualCamera;
    public GameObject Stamina => stamina;
    public RectTransform Stage1Stamina => stage1Stamina;
    public RectTransform Stage2Stamina => stage2Stamina;
    public List<PortalController> AutoRepairPortals => autoRepairPortals;

    public static TransportManager Instance { get; private set; }

    public List<GameObject> Transports => transports;
    public List<TransportController> TransportControllers => transportControllers;

    public GameObject CurrentTransport
    {
        get => currentTransport;
        set { if (value != null) currentTransport = value; }
    }
    GameObject currentTransport;

    public TransportController CurrentTransportController
    {
        get => currentTransportController;
        set { if (value != null) currentTransportController = value; }
    }
    TransportController currentTransportController;

    float forceWheelFactor = 300f;
    float forceTransportFactor = 100f;

    List<GameObject> transports = new();
    public readonly Dictionary<TransportType, int> TransportPrices = new()
    {
        {TransportType.Bike, 500 },
        {TransportType.ElectricScooter, 600 },
        {TransportType.Scooter, 1000 },
        {TransportType.Pirojok, 2000 },
        {TransportType.Mustang, 3500 }
    };

    public Dictionary<TransportType, bool> TransportUnlock { get; private set; } = new()
    {
        {TransportType.Bike, true },
        {TransportType.ElectricScooter, false },
        {TransportType.Scooter, false },
        {TransportType.Pirojok, false },
        {TransportType.Mustang, false }
    };

    string rewardID = "transport";
    string patterPay = "Починить за ";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Dictionary<TransportType, bool> savesDict = UnlockTransportSerialize.GetDict();
        if (savesDict.Count > 0)
        {
            TransportUnlock = savesDict;
        }

        foreach (TransportController controller in transportControllers)
            transports.Add(controller.gameObject);
    }

    public void SaveUnlockTransport()
    {
        UnlockTransportSerialize.SaveDict(TransportUnlock);
    }

    public void Terminate(List<GameObject> fallAwayObjects, List<WheelCollider> wheelColidersF, List<WheelCollider> wheelColidersR, Vector3 point)
    {
        Vector3 centerOfMass = CalculateCenterOfMass(fallAwayObjects);

        foreach (GameObject wheel in fallAwayObjects)
        {
            Destroy(wheel.GetComponent<WheelVisual>());

            BoxCollider collider = wheel.AddComponent<BoxCollider>();

            Rigidbody rg = wheel.AddComponent<Rigidbody>();
            rg.mass = 15f;

            Vector3 direction = (centerOfMass - wheel.transform.localPosition).normalized;
            rg.AddForce(direction * forceWheelFactor + Vector3.up * forceWheelFactor * rg.mass);
        }

        foreach (WheelCollider wheel in wheelColidersF)
            Destroy(wheel);
        foreach (WheelCollider wheel in wheelColidersR)
            Destroy(wheel);

        CurrentTransportController.Rg.AddForceAtPosition(Vector3.up * forceTransportFactor * CurrentTransportController.Rg.mass, point);

        StartCoroutine(ShowDestroyWindow());
    }

    IEnumerator ShowDestroyWindow()
    {
        payText.text = patterPay + Mathf.RoundToInt(TransportPrices[CurrentTransportController.TransportType] / 2);
        yield return new WaitForSeconds(1.2f);
        UIManager.Instance.ShowUI("Coins");
        UIManager.Instance.ShowUI("Destroy Transport Window");
        UIManager.Instance.ShowUI("Destroy Transport", false, true);
    }

    Vector3 CalculateCenterOfMass(List<GameObject> objects)
    {
        Vector3 center = Vector3.zero;
        foreach (GameObject wheel in objects)
        {
            center += wheel.transform.localPosition;
        }
        return center / objects.Count;
    }

    IEnumerator HideDestroyWindow(TransportType targetTransport)
    {
        UIManager.Instance.ShowUI("LoadScreen");

        UIManager.Instance.HideUI("Destroy Transport", false, true);
        UIManager.Instance.HideUI("Destroy Transport Window");
        UIManager.Instance.HideUI("Coins");
        yield return new WaitForSeconds(0.5f);
        ChangeTransport(targetTransport);
        yield return new WaitForSeconds(0.1f);

        UIManager.Instance.HideUI("LoadScreen");
    }

    public void RewardForTransport()
    {
        YG2.RewardedAdvShow(
            rewardID, 
            () => StartCoroutine(HideDestroyWindow(CurrentTransportController.TransportType))
            );
    }

    public void Pay()
    {
        int price = Mathf.RoundToInt(TransportPrices[CurrentTransportController.TransportType] / 2);
        if (CoinsManager.Instance.Coins - price > 0)
        {
            CoinsManager.Instance.Coins -= price;
            StartCoroutine(HideDestroyWindow(CurrentTransportController.TransportType));
            PromptManager.Instance.ShowPrompt($"Списание: {price}", 3f, true);
        }
        else
            PromptManager.Instance.NotEnoughCoins();
    }

    public void TakeSanctionedBike()
    {
        StartCoroutine(HideDestroyWindow(TransportType.SanctionedBike));
    }

    void ChangeTransport(TransportType type)
    {
        if (CurrentTransport != null)
            Destroy(CurrentTransport);

        foreach (GameObject prefab in transportPrefabs)
        {
            TransportController transport = prefab.GetComponent<TransportController>();
            if (transport == null) transport = prefab.GetComponent<BikeController>();

            if (transport.TransportType == type)
            {
                GameObject bike = Instantiate(prefab, GarageManager.Instance.Spawn);
                bike.SetActive(true);
                bike.transform.SetParent(null);

                CurrentTransport = bike;
            }
        }

        DestroyTransport?.Invoke();
    }

    public void TransportEnableControl(bool enable, TransportType targetType)
    {
        for (int i = 0; i < transportControllers.Count; i++)
            if (transportControllers[i].TransportType == targetType)
            {
                transportControllers[i].gameObject.SetActive(true);
            }
    }
}