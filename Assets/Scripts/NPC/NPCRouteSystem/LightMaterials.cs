using UnityEngine;

public class LightMaterials : MonoBehaviour
{
    [SerializeField] Material greenLightMaterial;
    [SerializeField] Material yellowLightMaterial;
    [SerializeField] Material redLightMaterial;
    [SerializeField] Material greenMaterial;
    [SerializeField] Material yellowMaterial;
    [SerializeField] Material redMaterial;

    public Material GreenMaterial => greenMaterial;
    public Material YellowMaterial => yellowMaterial;
    public Material RedMaterial => redMaterial;
    public Material GreenLightMaterial => greenLightMaterial;
    public Material YellowLightMaterial => yellowLightMaterial;
    public Material RedLightMaterial => redLightMaterial;

    public static LightMaterials Materials { get; private set; }

    void Awake()
    {
        Materials = this;
    }
}
