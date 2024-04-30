using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessDataProbability
{
    public enum FuncEnum
    {
        Gauss,
        Erland5D,
        Ux
    }
    
    public Dictionary<int, int> densityDict;
    
    public float maxValue;

    private FuncEnum chosenFunc;

    public ProcessDataProbability(FuncEnum chosenFunc)
    {
        this.chosenFunc = chosenFunc;

        for (int i = 0; i < 100; i++)
        {
            densityDict.Add(i, 0);
        }
    }


    public void ValidateMaxValue(NDT.ServerLog.ServerStatus serverStatus)
    {
        float currentValue = GetNeededLogData(serverStatus);
        maxValue = currentValue > maxValue ? currentValue : maxValue;
    }

    public NDT.ViewData GetViewData(List<NDT.ServerLog> serverLogList)
    {
        foreach (var serverLog in serverLogList)
        {
            foreach (var serverStatus in serverLog.serverStatusList)
            {
                float value = GetNeededLogData(serverStatus);
                densityDict[(int)(value / maxValue)]++;
            }
        }

        var viewData = FormViewDataInstance();

        foreach (var density in densityDict)
        {
            viewData.pointsList.Add(new NDT.ViewData.Points(density.Key, density.Value));
        }

        return viewData;
    }

    private float GetNeededLogData(NDT.ServerLog.ServerStatus serverStatus)
    {
        return chosenFunc switch
        {
            FuncEnum.Gauss => serverStatus.BxValue,
            FuncEnum.Erland5D => serverStatus.AxValue,
            _ => -1
        };
    }

    private NDT.ViewData FormViewDataInstance()
    {
        return chosenFunc switch
        {
            FuncEnum.Gauss => new NDT.ViewData(NDT.ViewData.ViewType.Chart, NDT.TargetView.Gauss),
            FuncEnum.Erland5D => new NDT.ViewData(NDT.ViewData.ViewType.Chart, NDT.TargetView.Erland),
            _ => null
        };
    }

}
