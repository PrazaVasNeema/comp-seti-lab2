using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InvestigatePieceAbstract : MonoBehaviour
{
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

        public List<Points> pointsList;
        public Lab1DataSO.DependencyValue xAxisName;
    }

    public abstract ChartData Investigate(List<ServerModel.ServerLog> serverLogList, float x);
}
