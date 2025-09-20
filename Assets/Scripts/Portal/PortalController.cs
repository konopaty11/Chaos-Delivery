using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PortalController : MonoBehaviour
{
    [SerializeField] string pattern;
    [SerializeField] string place;
    [SerializeField] TextMeshProUGUI portalTMP;
    [SerializeField] Button portalBtn;
    [SerializeField] TextMeshPro placeTMP;

    string playerTag = "Player";
    string warningContent = "Портал вас не распознал вас.\nЗаедте в него на меньшей скорости.";

    float maxSpeed = 7f;
    bool textAdded = false;

    TransportController transportController;
    static UnityAction prevAction;

    public UnityAction Listener { get; set; }

    void Start()
    {
        placeTMP.text = pattern + place;
    }

    void Update()
    {
        RotateTMP();
    }

    void RotateTMP()
    {
        if (TransportManager.Instance.CurrentTransport != null)
            placeTMP.transform.rotation = Quaternion.Euler(0, TransportManager.Instance.CurrentTransport.transform.eulerAngles.y, 0);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        transportController = other.GetComponent<TransportController>();
        if (transportController.Rg.linearVelocity.magnitude > maxSpeed)
        {
            PromptManager.Instance.ShowPrompt(warningContent);
            return;
        }

        if (Listener == null)
        {
            Debug.Log("Слушателя нет " + pattern);
            return;
        }
        SetListenerPortal();
        ShowPortalUI();
    }

    public void SetPlace(string place)
    {
        this.place = place;
        placeTMP.text = pattern + place;
    }

    public virtual void ShowPortalUI()
    {
        UIManager.Instance.ShowUI("Portal");
        portalTMP.text = pattern + place;
        textAdded = true;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        HidePortalUI();
    }

    public virtual void HidePortalUI()
    {
        if (!textAdded) return;

        UIManager.Instance.HideUI("Portal");
        textAdded = false;
    }

    public void BlockControl(bool isBlock) => transportController.BlockControl = isBlock;

    void SetListenerPortal()
    {
        if (prevAction != null)
            portalBtn.onClick.RemoveListener(prevAction);

        portalBtn.onClick.AddListener(HidePortal);
        portalBtn.onClick.AddListener(() => BlockControl(true));
        portalBtn.onClick.AddListener(Listener);
        prevAction = Listener;
    }

    void HidePortal() => UIManager.Instance.HideUI("Portal");
}
