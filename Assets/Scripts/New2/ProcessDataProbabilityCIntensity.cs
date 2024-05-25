using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessDataProbabilityCIntensity
{

    public string result;
    
    public float maxValue;


    public ProcessDataProbabilityCIntensity()
    {
        result = "";
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

    public void GetViewData(List<NDT.ServerLog> serverLogList, Lab1DataSO.Data lab1Data, int NodeId, float TotalNetworkTime)
    {
        
        
        var resultValue = 0f;
        

        foreach (var serverLog in serverLogList)
        {
            resultValue += serverLog.allTimeIncomeTasksCount / TotalNetworkTime;
            
            
        }

        resultValue /= serverLogList.Count;
        

        result = "(I) SERVER NODE ID: " + NodeId +", lambda: " + lab1Data.lambda + ", " + "D: " + lab1Data.D + ", " + "mean: " + lab1Data.mean + ", " + "std_dev: " + lab1Data.std_dev + ", Nodes Count: " +
                 lab1Data.nodesCount + ", P peredachi: " + lab1Data.pToPassTask + ", CIntensity: " + resultValue;
        
        Debug.Log($"Result: {result}");


    }



    private NDT.ViewData FormViewDataInstance()
    {
        return new NDT.ViewData(NDT.ViewData.ViewType.Chart, NDT.TargetView.Ux);
    }

}
