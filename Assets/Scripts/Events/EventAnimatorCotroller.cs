using System.Collections;
using UnityEngine;

public class EventAnimatorCotroller : MonoBehaviour
{
    [SerializeField] Animator animator;

    const string randomParamName = "Random";

    void Start()
    {
        StartCoroutine(RandomChangeAnimations());
    }

    IEnumerator RandomChangeAnimations()
    {
        while (true)
        {
            animator.SetInteger(randomParamName, Random.Range(0, animator.runtimeAnimatorController.animationClips.Length));
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }
}
