using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class SettingsTransport
{
    WheelFrictionCurve forwardFricF;
    WheelFrictionCurve forwardFricR;
    WheelFrictionCurve sidewaysFricF;   
    WheelFrictionCurve sidewaysFricR;
    float power;
    float strenght;

    float stiffnessStep = 0.2f;
    float powerPercent = 0.1f;
    float strenghtPercent = 0.2f;

    public SettingsTransport(
        WheelFrictionCurve forwardFricF, WheelFrictionCurve forwardFricR, 
        WheelFrictionCurve sidewaysFricF, WheelFrictionCurve sidewaysFricR, 
        float power, float strenght
        )
    {
        this.forwardFricF = forwardFricF;
        this.forwardFricR = forwardFricR;
        this.sidewaysFricF = sidewaysFricF;
        this.sidewaysFricR = sidewaysFricR;
        this.power = power;
        this.strenght = strenght;
    }

    public WheelFrictionCurve GetForwardFrictionF(int level)
    {
        level--;
        WheelFrictionCurve newForwardFricF = forwardFricF;
        newForwardFricF.stiffness += stiffnessStep * level;
        return newForwardFricF;
    }

    public WheelFrictionCurve GetForwardFrictionR(int level)
    {
        level--;
        WheelFrictionCurve newForwardFricR = forwardFricR;
        newForwardFricR.stiffness += stiffnessStep * level;
        return newForwardFricR;
    }

    public WheelFrictionCurve GetSidewaysFrictionF(int level)
    {
        level--;
        WheelFrictionCurve newSidewaysFricF = sidewaysFricF;
        newSidewaysFricF.stiffness += stiffnessStep * level;
        return newSidewaysFricF;
    }

    public WheelFrictionCurve GetSidewaysFrictionR(int level)
    {
        level--;
        WheelFrictionCurve newSidewaysFricR = sidewaysFricR;
        newSidewaysFricR.stiffness += stiffnessStep * level;
        return newSidewaysFricR;
    }

    public float GetPower(int level)
    {
        level--;
        float resPower = power;
        for (int i = 0; i < level; i++)
        {
            resPower += resPower * powerPercent;
        }
        return resPower;
    }

    public float GetStrenght(int level)
    {
        level--;
        float resStrenght = strenght;
        for (int i = 0; i < level; i++)
        {
            resStrenght += resStrenght * strenghtPercent;
        }
        return resStrenght;
    }
}
