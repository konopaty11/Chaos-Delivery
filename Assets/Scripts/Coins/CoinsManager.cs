using System;
using System.Collections;
using TMPro;
using UnityEngine;
using YG;

public class CoinsManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] RectTransform coinsTransform;

    public static CoinsManager Instance;

    int coins;
    public int Coins { get => coins; set => SetCoins(value); }

    public static event Action AnimationEnd;

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
        coins = YG2.saves.coins;
        CoinsInit();
    }

    void CoinsInit() => coinsText.text = coins.ToString();
    

    void SetCoins(int coins)
    {
        if (coins > this.coins) StartCoroutine(AddCoinsAnimation());

        this.coins = coins;
        coinsText.text = this.coins.ToString();

        YG2.saves.coins = coins;
        YG2.SaveProgress();
    }

    IEnumerator AddCoinsAnimation()
    {
        float bounceHeight = 20f;
        float duration = 0.5f;
        Vector2 coinsPosition = coinsTransform.anchoredPosition;

        yield return MoveTo(coinsTransform, coinsPosition + Vector2.up * bounceHeight, duration / 4f);
        yield return MoveTo(coinsTransform, coinsPosition, duration / 4f);
        yield return MoveTo(coinsTransform, coinsPosition + Vector2.up * bounceHeight / 2, duration / 4f);
        yield return MoveTo(coinsTransform, coinsPosition, duration / 4f);
        yield return new WaitForSeconds(0.1f);

        AnimationEnd?.Invoke();
    }

    IEnumerator MoveTo(RectTransform transform, Vector2 targetPosition, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.anchoredPosition = Vector2.Lerp(transform.anchoredPosition, targetPosition, time / duration);
            yield return null;
        }
    }
}
