using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIManager : MonoBehaviour
{
    [SerializeField] UIGroup[] uiGroups;

    public static UIManager Instance { get; private set; }

    Dictionary<string, Coroutine> fadeCoroutines = new();
    Dictionary<string, Coroutine> moveCoroutines = new();

    List<string> hidenGroups = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void HideAllUI()
    {
        foreach (UIGroup group in uiGroups)
        {
            if (group.isVisible)
            {
                FastHideUI(group);
                hidenGroups.Add(group.groupName);
            }
        }
    }

    public void ShowHidenUI()
    {
        foreach (string groupName in hidenGroups)
        {
            ShowUI(groupName, true, true);
        }
        hidenGroups = new();
    }

    public void ShowUI(string groupName, bool fade = true, bool move = false)
    {
        if (fade)
        {
            if (fadeCoroutines.ContainsKey(groupName))
            {
                Coroutine fadeCoroutine = fadeCoroutines[groupName];
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            }

            fadeCoroutines[groupName] = StartCoroutine(FadeUI(groupName, 1, true));
        }
        if (move)
        {
            if (moveCoroutines.ContainsKey(groupName))
            {
                Coroutine moveCoroutine = moveCoroutines[groupName];
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            }

            moveCoroutines[groupName] = StartCoroutine(MoveUI(groupName, false));
        }
    }

    public void HideUI(string groupName, bool fade = true, bool move = false)
    {
        if (fade)
        {
            if (fadeCoroutines.ContainsKey(groupName))
            {
                Coroutine fadeCoroutine = fadeCoroutines[groupName];
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            }

            fadeCoroutines[groupName] = StartCoroutine(FadeUI(groupName, 0, false));
        }
        if (move)
        {
            if (moveCoroutines.ContainsKey(groupName))
            {
                Coroutine moveCoroutine = moveCoroutines[groupName];
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            }

            moveCoroutines[groupName] = StartCoroutine(MoveUI(groupName, true));
        }
    }

    public void SetGroupDuration(string groupName, float newDuration)
    {
        foreach (UIGroup group in uiGroups)
            if (group.groupName == groupName)
            {
                group.duration = newDuration;
            }
    }

    IEnumerator FadeUI(string groupName, float alpha, bool InteractableRaycasts)
    {
        foreach (UIGroup group in uiGroups)
            if (group.groupName == groupName)
            {
                List<float> startAlpas = new();
                foreach (CanvasGroup elem in group.elements)
                    startAlpas.Add(elem.alpha);

                float elapsed = 0;
                while (elapsed < group.duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float time = Mathf.Clamp01(elapsed / group.duration);

                    for (int i = 0; i < group.elements.Length; i++)
                        group.elements[i].alpha = Mathf.Lerp(startAlpas[i], alpha, time);

                    yield return null;
                }

                foreach (CanvasGroup elem in group.elements)
                {
                    elem.alpha = alpha;
                    elem.interactable = InteractableRaycasts;
                    elem.blocksRaycasts = InteractableRaycasts;
                }
                group.isVisible = InteractableRaycasts;
            }
    }

    IEnumerator MoveUI(string groupName, bool isRevers)
    {
        foreach (UIGroup group in uiGroups)
        {
            Vector2 targetPosition = isRevers ? group.startPosition : group.finishPosition;

            if (group.groupName == groupName)
            {
                List<RectTransform> uiRects = new();
                foreach (CanvasGroup elem in group.elements)
                    uiRects.Add(elem.gameObject.GetComponent<RectTransform>());

                while (Vector2.Distance(uiRects[0].anchoredPosition, targetPosition) > 0.1f)
                {
                    foreach (RectTransform ui in uiRects)
                        ui.anchoredPosition = Vector2.Lerp(ui.anchoredPosition, targetPosition, group.moveSpeed * Time.deltaTime);

                    yield return null;
                }
                foreach (RectTransform ui in uiRects)
                    ui.anchoredPosition = targetPosition;

                group.isVisible = !isRevers;
            }
        }
    }

    void FastHideUI(UIGroup uiGroup)
    {
        foreach (CanvasGroup elem in uiGroup.elements)
        {
            elem.alpha = 0;
            elem.blocksRaycasts = false;
            elem.interactable = false;
        }
    }
}
