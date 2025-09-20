using System.Collections;
using TMPro;
using UnityEngine;

public class PromptManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] GameObject coin;

    public static PromptManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NotEnoughCoins()
    {
        ShowPrompt("Недостаточно казах-коинов", 3f, true);
    }

    public void ShowPrompt(string content, float displayDuration = 3f, bool coinVisible = false)
    {
        promptText.text = content;
        if (coinVisible) coin.SetActive(true);
        else coin.SetActive(false);

        StartCoroutine(PromptControl(displayDuration));
    }

    IEnumerator PromptControl(float duration)
    {
        UIManager.Instance.ShowUI("Prompt", false, true);
        yield return new WaitForSeconds(duration);
        UIManager.Instance.HideUI("Prompt", false, true);
    }

}
