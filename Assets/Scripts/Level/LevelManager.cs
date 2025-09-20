using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YG;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Image scaleLevel;
    [SerializeField] TextMeshProUGUI levelText;

    public static LevelManager Instance { get; private set; }

    float level = 1f;
    public float Level { get => level; set => StartCoroutine(SetLevel(value)); }
    public int CurrentLevelInt => (int)level;
    public float CurrentLevelFloat => Mathf.Repeat(level, 1f);

    public static UnityAction AnimationEnd;

    Dictionary<OrderType, float> orderTypeUnlockLevel = new()
    {
        {OrderType.TimeLimited, 3f },
        {OrderType.Smuggling, 5f }
    };

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
        LevelInit();
    }

    void LevelInit()
    {
        level = YG2.saves.level;

        levelText.text = CurrentLevelInt.ToString();
        scaleLevel.fillAmount = CurrentLevelFloat;
    }

    IEnumerator SetLevel(float newLevel)
    {
        if (Level >= newLevel) yield break;

        int levelInt = CurrentLevelInt;
        float startLevel = level;

        float elapsed = 0f;
        float duration = newLevel - startLevel;
        while (elapsed < duration && level < newLevel)
        {
            elapsed += Time.deltaTime;
            level = Mathf.Lerp(startLevel, newLevel, elapsed / duration);
            scaleLevel.fillAmount = CurrentLevelFloat;

            if (levelInt < CurrentLevelInt)
            {
                levelInt = CurrentLevelInt;
                levelText.text = CurrentLevelInt.ToString();

                yield return AnchorsTo(levelText.GetComponent<RectTransform>(), 1f / 1.5f, 0.3f);
                yield return AnchorsTo(levelText.GetComponent<RectTransform>(), 1.5f, 0.3f);
            }

            yield return null;
        }

        level = newLevel;
        scaleLevel.fillAmount = CurrentLevelFloat;

        YG2.saves.level = level;
        AnimationEnd?.Invoke();

        LevelUp();
    }

    void LevelUp()
    {
        foreach (var (orderType, level) in orderTypeUnlockLevel)
            if (orderType > OrderManager.Instance.UnlockTypeOrder && this.level >= level)
            {
                OrderManager.Instance.UnlockTypeOrder = orderType;
                YG2.saves.UnlockTypeOrder = orderType;
                PromptManager.Instance.ShowPrompt($"Вау. Вам доступен новый тип заказов: {OrderManager.Instance.GetStringOfOrderType(orderType)}", 5f);
            }
    }

    IEnumerator AnchorsTo(RectTransform uiTransform, float percent, float duration)
    {
        Vector2 targetOffsetMin = uiTransform.offsetMin * percent;
        Vector2 targetOffsetMax = uiTransform.offsetMax * percent;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            uiTransform.offsetMin = Vector2.Lerp(uiTransform.offsetMin, targetOffsetMin, elapsed / duration);
            uiTransform.offsetMax = Vector2.Lerp(uiTransform.offsetMax, targetOffsetMax, elapsed / duration);

            yield return null;
        }

        uiTransform.offsetMin = targetOffsetMin;
        uiTransform.offsetMax = targetOffsetMax;
    }


}
