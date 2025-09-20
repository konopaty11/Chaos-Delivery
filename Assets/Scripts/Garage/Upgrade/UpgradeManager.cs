using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    

    public static UpgradeManager Instance {get; private set;}
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


}
