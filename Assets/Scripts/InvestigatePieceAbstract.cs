using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InvestigatePieceAbstract
{
    public enum TargetChart
    {
        Ux,
        P_prostoi,
        Erland,
        Gauss,
        ServerActivity
    }
    
    public class ChartData
    {
        public class Points
        {
            public float x;
            public float y;
            
            public Points(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public List<Points> pointsList = new List<Points>();
        public Lab1DataSO.DependencyValue xAxisName;
        public TargetChart targetChart;
    }

    public ChartData chartData = new ChartData();
    public float investigatedValueTempSum = 0f;
    
    public abstract void InvestigateOne(List<ServerModel.ServerLog> serverLogList);

    public virtual void InvestigateTwo(int itersCount, float x)
    {
        investigatedValueTempSum /= itersCount;
        
        chartData.pointsList.Add(new ChartData.Points(x, investigatedValueTempSum));
    }

    public virtual ChartData InvestigateFinal(Lab1DataSO.DependencyValue xAxisName)
    {
        chartData.xAxisName = xAxisName;
        
        return chartData;
    }
    
}
