using System.Collections.Generic;
using UnityEngine;
using YG;

public class Test : MonoBehaviour
{
    //public List<Upgrades> test = new();
    //private void Awake()
    //{
    //    if (YG2.saves.test.Count == 0)
    //    {
    //        foreach (Upgrades transport in FindObjectsByType<Upgrades>(sortMode: FindObjectsSortMode.None))
    //        {
    //            if (transport.TransportType == TransportType.Mustang)
    //            {
    //                transport.powerLevel = 5;
    //                test.Add(transport);
    //                Debug.Log("Load");
    //            }
    //        }
    //        YG2.saves.test = test;
    //    }
    //    else
    //    {
    //        Debug.Log(YG2.saves.test[0].TransportType);
    //        Debug.Log(YG2.saves.test[0].powerLevel);
    //    }
    //}

    //private void Start()
    //{
    //    YG2.SaveProgress();
    //}

    public void Reset()
    {
        YG2.saves.upgrades = new();
    }
}
