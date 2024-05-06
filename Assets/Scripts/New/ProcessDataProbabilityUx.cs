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
        
        foreach (var serverLog in serverLogList)
        {
            foreach (var taskData in serverLog.alltimeTaskList)
            {
                float timeToFinishValue = taskData.finishTime - taskData.arrivalTime;
                densityDict[(int) (Mathf.Lerp(0, 99.9f,timeToFinishValue / maxValue))]++;
            }
        }

        var viewData = FormViewDataInstance();

        foreach (var density in densityDict)
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
