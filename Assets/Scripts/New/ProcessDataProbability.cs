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

        densityDict = new Dictionary<int, int>();
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
        int valueCounter = -1;

        foreach (var serverLog in serverLogList)
        {
            foreach (var serverStatus in serverLog.serverStatusList)
            {
                ValidateMaxValue(serverStatus);
            }
        }

        
        foreach (var serverLog in serverLogList)
        {
            foreach (var serverStatus in serverLog.serverStatusList)
            {
                float value = GetNeededLogData(serverStatus);
                if (value == 0)
                    continue;
                if (chosenFunc == FuncEnum.Erland5D)
                {
                    // Debug.Log($"value: {value}");
                    // Debug.Log((value / maxValue));
                    // Debug.Log((int) (Mathf.Lerp(0, 99.9f,value / maxValue)));
                }
                
                if (value == maxValue)
                    Debug.Log($"value: {value}");

                densityDict[(int) (Mathf.Lerp(0, 99.9f,value / maxValue))]++;
                valueCounter++;
            }
        }

        // for (int i = 0; i < 100; i++)
        // {
        //     densityDict[i] /= serverLogList.Count;
        // }

        var viewData = FormViewDataInstance();

        var finalDict = new Dictionary<float, float>();
        
        for (int i = 0; i < 100; i++)
        {
            finalDict.Add(Mathf.Lerp(0, maxValue, (float) i / 99), densityDict[i]);
        }

        foreach (var density in finalDict)
        {
            viewData.pointsList.Add(new NDT.ViewData.Points(density.Key, density.Value));

            if (chosenFunc == FuncEnum.Erland5D)
            {
                // Debug.Log($"density.Key: {density.Key}, density.Value: {density.Value}");
            }
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
