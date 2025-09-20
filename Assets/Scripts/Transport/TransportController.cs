using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections;

public class TransportController : MonoBehaviour
{
    [Header("TransportType")]
    [SerializeField] TransportType transportType;

    [Header("Wheels")]
    [SerializeField] protected List<WheelCollider> wheelsF;
    [SerializeField] protected List<WheelCollider> wheelsR;
    [SerializeField] List<GameObject> fallAwayObjects;

    [Header("Metrics")]
    [SerializeField] float motorToque;
    [SerializeField] float brakeToque;
    [SerializeField] float steerAngle;
    [SerializeField] float downforce;
    [SerializeField] float strenght;

    [Header("InputAction")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Lights")]
    [SerializeField] Material brakeLight;
    [SerializeField] List<GameObject> reversLights;

    [Header("Box")]
    [SerializeField] GameObject box;

    public Rigidbody Rg { get; private set; }
    public TransportType TransportType => transportType;
    public bool BlockControl { get; set; }
    public GameObject Box => box;

    PortalController autoRepairPortal;
    InputAction moveAction;
    Animator animator; 
    string actionMapName = "Player";
    string moveActionName = "Move";
    float valueMotor;
    float currentStrenght;
    bool isStoping = false;

    float baseIntensity = 40f;
    float brakeIntensity = 135f;
    float factorBrake = -2.5f;
    protected float powerFactor = 1f;

    float delay = 0.5f;
    float springFactor = 0.3f;

    protected SettingsTransport settings;
    protected UpgradeData currentUpgrade = null;

    Slider steerSlider;
    protected Slider motorSlider;
    protected CinemachineVirtualCamera virtualCamera;

    protected virtual void Awake()
    {
        steerSlider = TransportManager.Instance.SteerSlider;
        motorSlider = TransportManager.Instance.MotorSlider;
        virtualCamera = TransportManager.Instance.VirtualCamera;

        foreach (PortalController autoRepairPortal in TransportManager.Instance.AutoRepairPortals)
            autoRepairPortal.Listener = RestoreStrenght;

        steerSlider.onValueChanged.AddListener(Steer);
        motorSlider.onValueChanged.AddListener(MotorSlider);

        InputActionMap actionMap = inputActions.FindActionMap(actionMapName);
        moveAction = actionMap.FindAction(moveActionName);

        settings = new(
            wheelsF[0].forwardFriction, wheelsR[0].forwardFriction,
            wheelsF[0].sidewaysFriction, wheelsR[0].sidewaysFriction,
            motorToque, strenght
            );
    }

    void Start()
    {
        Rg = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        GarageManager.Instance.InvokeSubscribers();
    }

    protected virtual void OnEnable()
    {
        CameraManager.Instance.TransitionCamera(CameraType.PlayerCamera);
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;

        moveAction.started += OnMoveStarted;
        moveAction.performed += OnMovePerfomed;
        moveAction.canceled += OnMoveCanceled;
        moveAction.Enable();

        StartCoroutine(InitializeTransportManagerReference());

        GarageManager.OnInitialized += SetUpgrates;
    }

    protected virtual void OnDisable()
    {
        moveAction.started -= OnMoveStarted;
        moveAction.performed -= OnMovePerfomed;
        moveAction.canceled -= OnMoveCanceled;
        moveAction.Disable();

        GarageManager.OnInitialized -= SetUpgrates;
    }

    void Update()
    {
        AnimationControl();
        Drive();
    }

    protected virtual void FixedUpdate()
    {
        Aerodynamics();
    }

    IEnumerator InitializeTransportManagerReference()
    {
        if (TransportManager.Instance == null)
            yield return null;

        TransportManager.Instance.CurrentTransport = gameObject;
        TransportManager.Instance.CurrentTransportController = this;
    }

    void OnMoveStarted(InputAction.CallbackContext context) => SliderConttroller.IsHeld = true;
    void OnMovePerfomed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        motorSlider.value = input.y;
        steerSlider.value = input.x;
    }
    void OnMoveCanceled(InputAction.CallbackContext context) => SliderConttroller.IsHeld = false;

    public void Steer(float value)
    {
        if (BlockControl) return;

        foreach (WheelCollider wheel in wheelsF)
        {
            wheel.steerAngle = steerAngle * value;
        }
    }

    public void MotorSlider(float value) => valueMotor = value;
    
    void Drive()
    {
        if ((valueMotor < 0 && Vector3.Dot(Rg.linearVelocity, transform.forward) > 0.1f)
            || isStoping)
            Brake();
        else
            Accelerate();
    }

    void Accelerate()
    {
        if (BlockControl) return;

        foreach (WheelCollider wheel in wheelsF)
        {
            wheel.brakeTorque = 0;
        }

        foreach (WheelCollider wheel in wheelsR)
        {
            wheel.brakeTorque = 0;
            wheel.motorTorque = motorToque * valueMotor * powerFactor;
        }

        if (valueMotor < 0)
            ReversLights();
        else
            SetDefaultLights();
    }

    void Brake()
    {
        if (BlockControl) return;

        BrakeLights();

        foreach(WheelCollider wheel in wheelsF)
        {
            wheel.motorTorque = 0;
            wheel.brakeTorque = brakeToque * (valueMotor < 0 ? valueMotor * factorBrake : 1);
        }

        foreach (WheelCollider wheel in wheelsR)
        {
            wheel.motorTorque = 0;
            wheel.brakeTorque = brakeToque * (valueMotor < 0 ? valueMotor * factorBrake : 1);
        }

        if (!isStoping && Rg.linearVelocity.magnitude < 0.5f)
            StartCoroutine(Stopping());
    }

    void Aerodynamics()
    {
        float speed = Rg.linearVelocity.magnitude;
        Rg.AddForce(-transform.up * downforce * speed);
    }

    void AnimationControl()
    {
        if (animator == null) return;

        if (motorSlider.value != 0)
            animator.speed = Rg.linearVelocity.magnitude;
        else
            animator.speed = 0;
    }

    void BrakeLights()
    {
        if (brakeLight != null)
            brakeLight.SetColor("_EmissionColor", Color.red * brakeIntensity);
    }
    void ReversLights()
    {
        if (brakeLight != null)
            brakeLight.SetColor("_EmissionColor", Color.red * baseIntensity);
        foreach (GameObject lightObj in reversLights)
        {
            lightObj.SetActive(true);
        }
    }
    void SetDefaultLights()
    {
        if (brakeLight != null)
            brakeLight.SetColor("_EmissionColor", Color.red * baseIntensity);
        foreach (GameObject lightObj in reversLights)
        {
            lightObj.SetActive(false);
        }
    }

    IEnumerator Stopping()
    {
        isStoping = true;
        yield return new WaitForSeconds(delay);
        isStoping = false;
    }

    protected virtual void SetUpgrates()
    {
        if (transportType == TransportType.SanctionedBike) return;

        List<UpgradeData> upgrades = GarageManager.Instance.upgradeDatas;
        foreach (UpgradeData upgrade in upgrades)
        {
            if (upgrade is BikeUpgradeData bikeUpgrade)
            {
                if (bikeUpgrade.transportType == TransportType)
                    currentUpgrade = bikeUpgrade;
                
            }
            else
            {
                if (upgrade.transportType == TransportType)
                    currentUpgrade = upgrade;
            }
        }

        foreach (WheelCollider wheel in wheelsF)
        {
            wheel.forwardFriction = settings.GetForwardFrictionF(currentUpgrade.stiffnessLevel);
            wheel.sidewaysFriction = settings.GetSidewaysFrictionF(currentUpgrade.stiffnessLevel);
        }

        foreach (WheelCollider wheel in wheelsR)
        {
            wheel.forwardFriction = settings.GetForwardFrictionR(currentUpgrade.stiffnessLevel);
            wheel.sidewaysFriction = settings.GetSidewaysFrictionR(currentUpgrade.stiffnessLevel);
        }

        motorToque = settings.GetPower(currentUpgrade.powerLevel);
        strenght = settings.GetStrenght(currentUpgrade.strengthLevel);
        currentStrenght = strenght;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (TransportType == TransportType.SanctionedBike) return;

        Rigidbody rg2 = collision.collider.GetComponent<Rigidbody>();
        Vector3 impuls2 = rg2 == null ? Vector3.zero : rg2.mass * rg2.linearVelocity;

        float force = collision.impulse.magnitude / Time.fixedDeltaTime;
        currentStrenght -= force;

        float springForce = collision.relativeVelocity.magnitude * Rg.mass * springFactor;
        Rg.AddForceAtPosition(springForce * collision.contacts[0].normal, collision.contacts[0].point);

        if (currentStrenght <= 0 && !BlockControl)
        {
            BlockControl = true;
            TransportManager.Instance.Terminate(fallAwayObjects, wheelsF, wheelsR, collision.contacts[0].point);
        }
    }

    void RestoreStrenght()
    {
        BlockControl = false;
        int price = Mathf.RoundToInt((1 - currentStrenght / strenght) * TransportManager.Instance.TransportPrices[TransportType] / 4);
        if (price == 0)
        {
            PromptManager.Instance.ShowPrompt("Прочность максимальна");
            return;
        }

        if (CoinsManager.Instance.Coins - price >= 0)
        {
            CoinsManager.Instance.Coins -= price;
            StartCoroutine(ShowCoins());
            PromptManager.Instance.ShowPrompt($"Списание: {price}", 3f, true);
            currentStrenght = strenght;
        }
        else
            PromptManager.Instance.ShowPrompt("Недостаточно казах-коинов");
    }

    IEnumerator ShowCoins()
    {
        UIManager.Instance.ShowUI("Coins");
        yield return new WaitForSeconds(2f);
        UIManager.Instance.HideUI("Coins");
    }
}
