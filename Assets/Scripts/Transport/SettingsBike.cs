using UnityEngine;
using UnityEngine.UIElements;

public class SettingsBike
{
    float depletion;
    float depletionPercent = 0.2f;

    public SettingsBike(float depletion)
    {
        this.depletion = depletion;
    }

    public float GetDeplationRate(int level)
    {
        level--;
        float resDepletion = depletion;
        for (int i = 0; i < level; i++)
        {
            resDepletion -= resDepletion * depletionPercent;
        }
        return resDepletion;
    }
}
