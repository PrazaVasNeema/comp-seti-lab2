using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessDataProbabilityPprostoi
{

    public string result;
    
    public float maxValue;


    public ProcessDataProbabilityPprostoi()
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

    public float GetViewData(List<NDT.ServerLog> serverLogList, Lab1DataSO.Data lab1Data, int NodeId)
    {
        
        float timeProstoi = 0f;
        float totalTime = 0f;
        float prevTime = 0f;
        
        bool serverWasBusy = true;

        float Pprostoi = 0;


        foreach (var serverLog in serverLogList)
        {
            timeProstoi = 0f;

            prevTime = 0f;
            
            foreach (var serverStatus in serverLog.serverStatusList)
            {
                 if (!serverWasBusy)
                 {
                     timeProstoi += serverStatus.serverTime - prevTime;
                 }

                 totalTime = serverStatus.serverTime;
                 prevTime = serverStatus.serverTime;
                 serverWasBusy = serverStatus.serverIsBusy;
            }
            
            // Debug.Log($"Within Pprostoi: {timeProstoi / totalTime}");

            Pprostoi += timeProstoi / totalTime;
        }

        Pprostoi /= serverLogList.Count;

        result = "(P) SERVER NODE ID: " + NodeId +", lambda: " + lab1Data.lambda + ", " + "D: " + lab1Data.D + ", " + "mean: " + lab1Data.mean + ", " + "std_dev: " + lab1Data.std_dev + ", Nodes Count: " +
                 lab1Data.nodesCount + ", P peredachi: " + lab1Data.pToPassTask + ", Pprostoi: " + Pprostoi;
        
        Debug.Log($"Result: {result}");

        return Pprostoi;
    }



    private NDT.ViewData FormViewDataInstance()
    {
        return new NDT.ViewData(NDT.ViewData.ViewType.Chart, NDT.TargetView.Ux);
    }

}
