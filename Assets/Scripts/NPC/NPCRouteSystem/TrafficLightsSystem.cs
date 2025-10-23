using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class TrafficLightsSystem : MonoBehaviour
{
    [SerializeField] List<TrafficLightGroup> groups;

    [Serializable]
    class TrafficLightGroup
    {
        public List<TrafficLight> trafficLights;
        public float greenLightDuration;
    }

    void Start()
    {
        StartCoroutine(LightControl());
    }

    IEnumerator LightControl()
    {
        foreach (TrafficLightGroup group in groups)
        {
            foreach (TrafficLight trafficLight in group.trafficLights)
                trafficLight.RedLight();
        }

        const float _swithcingDelay = 1f;
        while (true)
        {
            foreach (TrafficLightGroup group in groups)
            {
                foreach (TrafficLight trafficLight in group.trafficLights)
                    trafficLight.GreenLight();

                yield return new WaitForSeconds(group.greenLightDuration);

                foreach (TrafficLight trafficLight in group.trafficLights)
                    trafficLight.RedLight();

                yield return new WaitForSeconds(_swithcingDelay);
            }
        }
    }
}
