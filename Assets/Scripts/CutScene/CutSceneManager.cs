using System.Collections.Generic;
using UnityEngine;
using YG;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] List<CutScene> cutScenes;

    public static CutSceneManager Instance { get; private set; }

    Plot currentPlotProgress; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentPlotProgress = YG2.saves.currentPlot;
        StartCutScene(Plot.Start);
    }

    public void StartCutScene(Plot plotProgress)
    {
        foreach (CutScene cutScene in cutScenes)
        {
            if (cutScene.PlotProgress == plotProgress && currentPlotProgress < plotProgress)
            {
                cutScene.StartCutScene();
                currentPlotProgress = plotProgress;
                YG2.saves.currentPlot = currentPlotProgress;
                YG2.SaveProgress();
            }
            else if (currentPlotProgress >= plotProgress)
                cutScene.SkipCutScene();
        }
    }
}
