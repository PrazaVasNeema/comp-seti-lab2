using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateErland5D : InvestigatePieceAbstract
{
    public override void InvestigateOne(ServerModel.ServerLog serverLog)
    {
        chartData.targetChart = TargetChart.Erland;

        float sumAx = 0f;
        int iterCount = 0;
        
        foreach (var log in serverLog.generalLogDatasList)
        {
            if (log.AxValue > 0)
            {
                sumAx += log.AxValue;
                iterCount++;
            }
        }
        
        sumAx /= iterCount;
        
        investigatedValueTempSum += sumAx;
        
    }

}
