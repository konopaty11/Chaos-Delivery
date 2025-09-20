using UnityEngine;

public abstract class CutScene : MonoBehaviour
{
    public abstract Plot PlotProgress { get; }
    public abstract string Name { get; }

    public abstract void StartCutScene();
    public abstract void SkipCutScene();
}
