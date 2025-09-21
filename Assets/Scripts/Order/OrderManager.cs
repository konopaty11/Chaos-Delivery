using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using YG;

public class OrderManager : MonoBehaviour
{
    [Header("Point")]
    [SerializeField] List<Transform> pickUpPoints;
    [SerializeField] List<Transform> issuePoints;

    [Header("Prefab")]
    [SerializeField] GameObject pickUpPrefab;
    [SerializeField] GameObject issuePrefab;

    [Header("Params")]
    [SerializeField] List<GameObject> timeLimiteParams;
    [SerializeField] GameObject fineParam;
    [SerializeField] TextMeshProUGUI type;
    [SerializeField] TextMeshProUGUI distance;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] TextMeshProUGUI tips;
    [SerializeField] TextMeshProUGUI timeOrder;
    [SerializeField] TextMeshProUGUI timeRemaining;
    [SerializeField] TextMeshProUGUI total;
    [SerializeField] TextMeshProUGUI timer;

    [Header("PickUp Params")]
    [SerializeField] TextMeshProUGUI typePickUp;
    [SerializeField] TextMeshProUGUI pricePickUp;
    [SerializeField] TextMeshProUGUI fine;

    [Header("Other UI")]
    [SerializeField] CanvasGroup _getTotalButtonCanvasGroup;
    [SerializeField] RectTransform directionArrow;

    [Header("AudioSource")]
    [SerializeField] AudioSource audioSource;

    public static OrderManager Instance { get; private set; }

    public OrderType UnlockTypeOrder { get; set; } = OrderType.Default;
    public bool IsWorking{ get; private set; } = false;
    public OrderType CurrentOrderType { get; private set; } = OrderType.None;

    List<Order> orders = new();
    Order newOrder;

    PortalController pickUpPortal;
    PortalController issuePortal;

    Coroutine timerCoroutine;
    Coroutine orderGenerate;
    Coroutine directArrowCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        CoinsManager.AnimationEnd += CloseOrder;
        LevelManager.AnimationEnd += GetCoins;
        TransportManager.DestroyTransport += RestoreBox;
    }

    void OnDisable()
    {
        CoinsManager.AnimationEnd -= CloseOrder;
        LevelManager.AnimationEnd -= GetCoins;
        TransportManager.DestroyTransport -= RestoreBox;
    }

    void Start()
    {
        UIManager.Instance.HideUI("Result Order Window");
        pickUpPortal = pickUpPrefab.GetComponent<PortalController>();
        issuePortal = issuePrefab.GetComponent<PortalController>();

        pickUpPortal.Listener = PickUp;
        issuePortal.Listener = Issue;

        UnlockTypeOrder = YG2.saves.unlockTypeOrder;
    }

    IEnumerator GenerateOrders()
    {
        float minTime = 5f;
        while (true)
        {
            if (newOrder != null)
            {
                yield return null;
                continue;
            }

            if (orders.Count > 0)
            {
                float time = orders[0].Time.Seconds > minTime ? 
                    UnityEngine.Random.Range(orders[0].Time.Seconds / 1.5f, orders[0].Time.Seconds) :
                    minTime;
                yield return new WaitForSeconds(time);
                AddOrder();
            }
            else
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(3, 7));
                AddOrder();
            }
        }
    }

    void HideOrderWindow() => UIManager.Instance.HideUI("Order", false, true);

    public void Accept()
    {
        orders.Add(newOrder);
        if (orders.Count == 1) CreateOrder();

        HideOrderWindow();
    }

    public void Reject()
    {
        newOrder = null;
        HideOrderWindow();
    }

    public void AddOrder()
    {
        Vector3 pickUpPosition = pickUpPoints[UnityEngine.Random.Range(0, pickUpPoints.Count)].position;
        Vector3 issuePosition = issuePoints[UnityEngine.Random.Range(0, issuePoints.Count)].position;
        float distance = Vector3.Distance(pickUpPosition, issuePosition);

        //if (TransportManager.Instance.CurrentTransportController.TransportType <= TransportType.ElectricScooter)
        //{

        //}

        OrderType type;
        if (UnityEngine.Random.Range(0f, 1f) < 0.6f) type = UnlockTypeOrder;
        else
        {
            OrderType t = (OrderType)UnityEngine.Random.Range(0, (int)UnlockTypeOrder);
            type = t;
        }

        float coefficient = 1;
        switch (type)
        {
            case OrderType.TimeLimited:
                coefficient = 1.5f;
                break;
            case OrderType.Smuggling:
                coefficient = 2.5f;
                break;
        }

        float timeForMetr = 0.5f;
        switch (TransportManager.Instance.CurrentTransportController.TransportType)
        {
            case TransportType.Bike:
                timeForMetr = 0.28f;
                break;

            case TransportType.ElectricScooter:
            case TransportType.Scooter:
                timeForMetr = 0.2f;
                break;

            case TransportType.Pirojok:
            case TransportType.Mustang:
                timeForMetr = 0.16f;
                break;
        }

        float price = distance * coefficient;
        float tips = 0f;
        if (type != OrderType.Smuggling && UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
            tips = UnityEngine.Random.Range(0f, 1f) < 0.9f ?
                    UnityEngine.Random.Range(0, price / 10) :
                    UnityEngine.Random.Range(0, price / 2);
        }

        newOrder = new Order(pickUpPosition, issuePosition, distance, price, tips, type, TimeSpan.FromSeconds(distance * timeForMetr));

        SetPickUpParams(newOrder);
        UIManager.Instance.ShowUI("Order", false, true);

        audioSource.Play();
    }

    void CreateOrder()
    {
        pickUpPrefab.SetActive(true);
        pickUpPrefab.transform.position = orders[0].PickUp;

        directionArrow.gameObject.SetActive(true);
        if (directArrowCoroutine != null)
            StopCoroutine(directArrowCoroutine);
        directArrowCoroutine = StartCoroutine(DirectArrow(pickUpPrefab.transform.position));

        pickUpPortal.SetPlace(GetStringOfOrderType(newOrder.Type));
        issuePortal.SetPlace(GetStringOfOrderType(newOrder.Type));

        CurrentOrderType = newOrder.Type;
        newOrder = null;
    }

    IEnumerator DirectArrow(Vector3 target)
    {
        while (true)
        {
            Transform currentTransportTransform = TransportManager.Instance.CurrentTransport.transform;

            Vector3 direction = (target - currentTransportTransform.position).normalized;
            direction.y = 0f;

            float targetAngle = Vector3.SignedAngle(currentTransportTransform.forward, direction, Vector3.down);

            float newAngle = Mathf.MoveTowardsAngle(directionArrow.eulerAngles.z, targetAngle, Time.deltaTime * 150f);
            directionArrow.eulerAngles = new(0, 0, newAngle);

            yield return null;
        }
    }

    void SetOrderParams(Order order)
    {
        type.text = GetStringOfOrderType(order.Type);
        distance.text = order.Distance.ToString();
        price.text = order.Price.ToString();
        tips.text = order.Tips.ToString();

        if (order.Type == OrderType.TimeLimited)
        {
            foreach (GameObject param in timeLimiteParams)
                param.SetActive(true);

            timeOrder.text = order.Time.Subtract(order.TimeRemaining).ToString(@"mm\:ss\.f");
            timeRemaining.text = order.TimeRemaining.ToString(@"mm\:ss\.f");
        }

        total.text = order.Total.ToString();
    }

    IEnumerator Timer(Order order)
    {
        timer.text = order.TimeRemaining.ToString(@"mm\:ss\.f");

        while (true)
        {
            if (order.TimeRemaining.TotalSeconds > 0)
            {
                order.TimeRemaining = order.TimeRemaining.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
                timer.text = order.TimeRemaining.ToString(@"mm\:ss\.f");
            }
            else if(!order.IsLate)
            {
                fineParam.SetActive(true);
                fine.text = order.Fine.ToString();
                timer.color = Color.red;
                order.IsLate = true;
            }

            yield return null;
        }
    }
    
    void SetPickUpParams(Order order)
    {
        typePickUp.text = GetStringOfOrderType(order.Type);
        pricePickUp.text = order.Price.ToString();
    }

    public void PickUp()
    {
        pickUpPrefab.SetActive(false);
        issuePrefab.SetActive(true);
        issuePrefab.transform.position = orders[0].Issue;
        _getTotalButtonCanvasGroup.interactable = true;

        if (directArrowCoroutine != null)
            StopCoroutine(directArrowCoroutine);
        directArrowCoroutine = StartCoroutine(DirectArrow(issuePrefab.transform.position));

        if (orders[0].Type == OrderType.TimeLimited)
        {
            UIManager.Instance.ShowUI("Timer");
            timerCoroutine = StartCoroutine(Timer(orders[0]));
        }

        pickUpPortal.BlockControl(false);

        TransportManager.Instance.CurrentTransportController.Box?.SetActive(true);
    }

    public void Issue()
    {
        SetOrderParams(orders[0]);

        UIManager.Instance.ShowUI("Result Order Window");
        UIManager.Instance.ShowUI("Result Order", false, true);
        UIManager.Instance.ShowUI("Coins");

        issuePrefab.SetActive(false);
        UIManager.Instance.HideUI("Timer");
        if (orders[0].Type == OrderType.TimeLimited)
        {
            timer.color = Color.white;
            StopCoroutine(timerCoroutine);
        }
    }

    public string GetStringOfOrderType(OrderType type)
    {
        return type switch
        {
            OrderType.Default => "Заказ",
            OrderType.TimeLimited => "Заказ на время",
            OrderType.Smuggling => "Контрабанда",
            _ => "Заказ",
        };
    }

    public void GetTotal()
    {
        _getTotalButtonCanvasGroup.interactable = false;
        TransportManager.Instance.CurrentTransportController.Box?.SetActive(false);

        if (orders.Count == 1) directionArrow.gameObject.SetActive(false);

        GetLevel();
    }

    void GetLevel() => LevelManager.Instance.Level += (float)Math.Round(1f / LevelManager.Instance.CurrentLevelInt, 3);
    void GetCoins() => CoinsManager.Instance.Coins += orders[0].Total;

    void CloseOrder()
    {
        issuePortal.BlockControl(false);

        orders.RemoveAt(0);
        UIManager.Instance.HideUI("Result Order", false, true);
        UIManager.Instance.HideUI("Result Order Window");
        UIManager.Instance.HideUI("Coins");

        foreach (GameObject param in timeLimiteParams)
            param.SetActive(false);
        fineParam.SetActive(false);

        CurrentOrderType = OrderType.None;
        if (orders.Count > 0) CreateOrder();

        YG2.SaveProgress();
    }

    public void StartGenerate()
    {
        orderGenerate = StartCoroutine(GenerateOrders());
        UIManager.Instance.HideUI("Start work");
        IsWorking = true;
    }

    public void StopGenerate()
    {
        StopCoroutine(orderGenerate);
        UIManager.Instance.ShowUI("Start work");
        IsWorking = false;
    }

    void RestoreBox()
    {
        if (issuePrefab.activeSelf)
            TransportManager.Instance.CurrentTransportController.Box?.SetActive(true);
    }

}