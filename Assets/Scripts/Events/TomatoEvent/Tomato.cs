using System.Collections;
using UnityEngine;

public class Tomato : MonoBehaviour
{
    const float lifeTime = 10f;

    string playerTag = "Player";
    static Coroutine hitTomatoCoroutine;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerTag) || hitTomatoCoroutine != null) return;

        hitTomatoCoroutine = StartCoroutine(HitTomato());
    }

    static IEnumerator HitTomato()
    {
        UIManager.Instance.SetGroupDuration("Tomato", 0.1f);
        UIManager.Instance.HideAllUI();
        UIManager.Instance.ShowUI("Tomato");

        yield return new WaitForSeconds(2f);

        UIManager.Instance.SetGroupDuration("Tomato", 1.5f);
        UIManager.Instance.HideUI("Tomato");
        UIManager.Instance.ShowHidenUI();

        yield return new WaitForSeconds(1.5f);

        hitTomatoCoroutine = null;
    }
}