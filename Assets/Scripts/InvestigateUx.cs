using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateUx : InvestigatePieceAbstract
{
    public override void InvestigateOne(ServerModel.ServerLog serverLog)
    {
        chartData.targetChart = TargetChart.Ux;
        
        // Queue<ServerModel.QBuffer.QContent> myQueue = new Queue<string>();

        
        
        
        float sumInProgressTime = 0f;
        int correctTasksIterCount = 0;

        
        foreach (var task in serverLog.tasksList)
        {
            if(task.finishTime == -1)
                continue;
            
            sumInProgressTime += task.finishTime - task.arrivalTime;
            correctTasksIterCount++;
            
        }

        investigatedValueTempSum += sumInProgressTime / correctTasksIterCount;
        
    }

}
