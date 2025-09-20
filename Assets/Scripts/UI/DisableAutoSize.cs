using TMPro;
using UnityEngine;

public class DisableAutoSize : MonoBehaviour
{
    void Start()
    {
        GetComponent<TextMeshProUGUI>().enableAutoSizing = false;
    }
}
