using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InvestigatePieceAbstract
{

    public ChartData chartData = new ChartData();
    public float investigatedValueTempSum = 0f;
    
    public abstract void InvestigateOne(ServerModel.ServerLog serverLog);

    public virtual void InvestigateTwo(int itersCount, float x)
    {
        investigatedValueTempSum /= itersCount + 1;
        
        // if(chartData.targetChart == TargetChart.P_prostoi)
        //     Debug.Log("INVESTIGATEEEEEE: " + investigatedValueTempSum);
        
        chartData.pointsList.Add(new ChartData.Points(x, investigatedValueTempSum));
    }

    public virtual ChartData InvestigateFinal(Lab1DataSO.DependencyValue xAxisName)
    {
        chartData.xAxisName = xAxisName;
        
        return chartData;
    }
    
}
