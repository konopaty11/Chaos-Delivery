using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] Renderer greenLight;
    [SerializeField] Renderer yellowLight;
    [SerializeField] Renderer redLight;

    Coroutine changeLightCoroutine;

    public static UnityEvent IsStartGreen;

    public bool IsGreen { get; private set; }

    public void GreenLight()
    {
        IsGreen = true;
        IsStartGreen?.Invoke();

        if (changeLightCoroutine != null)
            StopCoroutine(changeLightCoroutine);

        changeLightCoroutine = StartCoroutine(ChangeLight(redLight, LightMaterials.Materials.RedMaterial,
            greenLight, LightMaterials.Materials.GreenLightMaterial));
    }

    public void RedLight()
    {
        IsGreen = false;

        if (changeLightCoroutine != null)
            StopCoroutine(changeLightCoroutine);

        changeLightCoroutine = StartCoroutine(ChangeLight(greenLight, LightMaterials.Materials.GreenMaterial,
            redLight, LightMaterials.Materials.RedLightMaterial));
    }

    IEnumerator ChangeLight(Renderer start, Material startMaterial, Renderer finish, Material finishMaterial)
    {
        const float _delayChangeLigth = 1f;

        if (startMaterial != null)
            start.material = startMaterial;

        yellowLight.material = LightMaterials.Materials.YellowLightMaterial;
        yield return new WaitForSeconds(_delayChangeLigth);
        yellowLight.material = LightMaterials.Materials.YellowMaterial;
        finish.material = finishMaterial;
    }
}
