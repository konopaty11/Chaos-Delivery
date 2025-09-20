using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OrderPortalController : PortalController
{
    [SerializeField] Image loadScale;

    float duration = 2f;
    float startScale = 0f;
    float finishScale = 1f;

    Coroutine loadOrder;

    public override void ShowPortalUI()
    {
        loadOrder = StartCoroutine(LoadOrder());
    }

    IEnumerator LoadOrder()
    {
        UIManager.Instance.ShowUI("LoadOrder");

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            loadScale.fillAmount = Mathf.Lerp(startScale, finishScale, t);

            yield return null;
        }

        UIManager.Instance.HideUI("LoadOrder");
        base.ShowPortalUI();
    }

    public override void HidePortalUI()
    {
        base.HidePortalUI();

        if (loadOrder != null) StopCoroutine(loadOrder);
        UIManager.Instance.HideUI("LoadOrder");
        loadScale.fillAmount = 0;
    }
}
