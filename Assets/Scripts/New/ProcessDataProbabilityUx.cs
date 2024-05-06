using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessDataProbabilityUx
{
    public Dictionary<int, int> densityDict;
    
    public float maxValue;


    public ProcessDataProbabilityUx()
    {
        densityDict = new Dictionary<int, int>();
        for (int i = 0; i < 100; i++)
        {
            densityDict.Add(i, 0);
        }
    }


    public void ValidateMaxValue(List<NDT.TaskData> taskDataList)
    {
        foreach (var taskData in taskDataList)
        {
            if (taskData.finishTime == -1f)
                continue;
            float currentValue = taskData.finishTime - taskData.arrivalTime;
            maxValue = currentValue > maxValue ? currentValue : maxValue;
        }
    }

    public NDT.ViewData GetViewData(List<NDT.ServerLog> serverLogList)
    {
        foreach (var serverLog in serverLogList)
        {
            ValidateMaxValue(serverLog.alltimeTaskList);
        }

        int maxCount = 0;
        
        foreach (var serverLog in serverLogList)
        {
            foreach (var taskData in serverLog.alltimeTaskList)
            {
                float timeToFinishValue = taskData.finishTime - taskData.arrivalTime;
                densityDict[(int) (Mathf.Lerp(0, 99.9f,timeToFinishValue / maxValue))]++;
                maxCount++;
            }
        }
        
        var sumDict = new Dictionary<float, float>();
        float sumValue = 0;
        
        // Debug.Log($"MaxCount: {maxCount}");
        
        foreach (var density in densityDict)
        {
            sumValue += (float) density.Value / maxCount;
            // Debug.Log($"Sum value: {sumValue}");
            sumDict.Add(density.Key, sumValue);
            // viewData.pointsList.Add(new NDT.ViewData.Points(density.Key, density.Value));
        }
        
        

        var viewData = FormViewDataInstance();

        foreach (var density in sumDict)
        {
            viewData.pointsList.Add(new NDT.ViewData.Points(density.Key, density.Value));
        }

        return viewData;
    }



    private NDT.ViewData FormViewDataInstance()
    {
        return new NDT.ViewData(NDT.ViewData.ViewType.Chart, NDT.TargetView.Ux);
    }

}
