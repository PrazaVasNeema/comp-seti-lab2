using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerModel : MonoBehaviour
{
    public class ServerLog
    {
        public enum EventType
        {
            T1,
            T2
        }
        
        public EventType TIP;
        public float serverTime;
        public bool serverIsBusy;
        public int QSize;
        public int curTask;
        public float AxValue;
        public float BxValue;

        public void SetValues(EventType TTP, float serverTime, bool serverIsBusy, int QSize, int curTask, float AxValue,
            float BxValue)
        {
            this.TIP = TIP;
            this.serverTime = serverTime;
            this.serverIsBusy = serverIsBusy;
            this.QSize = QSize;
            this.curTask = this.curTask;
            this.AxValue = AxValue;
            this.BxValue = BxValue;
        }
    }

    [SerializeField] private ChartView m_ChartView;
    
    [SerializeField] private Lab1DataSO m_labDataSO;
    private Lab1DataSO.Data m_labData => m_labDataSO.data;
    // [SerializeField] private int m_serverCores;
    [SerializeField] private int m_qMaxSize;
    
    // public void Calculate()
    // {
    //     float Ax;
    //     float Bx;
    //     float dependencyParamDefValue = GetDependencyValue(m_labData.dependencyValue);
    //
    //     int totalIters = (int)((m_labData.to - m_labData.from) / m_labData.step) + 2;
    //     List<ChartData> chartDataList = new List<ChartData>();
    //     // Debug.Log($"valuesLength: {(int) ((m_labData.to - m_labData.from) / m_labData.step)}");
    //
    //     int arrayIter = -1;
    //     for (float i = m_labData.from; i <= m_labData.to; i += m_labData.step)
    //     {
    //         arrayIter++;
    //         SetDependencyValue(m_labData.dependencyValue, i);
    //
    //         float sumIters = 0;
    //         for (int j = 0; j <= m_labData.iterAmount; j++)
    //         {
    //             float sumTasks = 0;
    //             
    //
    //             float P_Prostoi = 0;
    //
    //             //////
    //             
    //
    //             // sumTasks /= m_labData.k;
    //
    //             sumTasks = P_Prostoi / serverTimer;
    //             sumIters += sumTasks;
    //
    //         }
    //
    //         sumIters /= m_labData.iterAmount;
    //
    //         ChartData newChartData = new ChartData();
    //         newChartData.x = i;
    //         newChartData.y = sumIters;
    //         chartDataList.Add(newChartData);
    //
    //     }
    //
    //     SetDependencyValue(m_labData.dependencyValue, dependencyParamDefValue);
    //
    //     // foreach (var value in valuesArray)
    //     // {
    //     //     Debug.Log($"P_Prostoi: {value}");
    //     // }
    //     
    //     m_ChartView.UpdateChart(chartDataList);
    //     
    //     Debug.Log(chartDataList.Count);
    //     
    // }

    public void ImitateServerActivity()
    {
        var serverLogList = ImitateServer();
        var chartData = new InvestigatePieceAbstract.ChartData();
        chartData.xAxisName = Lab1DataSO.DependencyValue.justTime;

        foreach (var log in serverLogList)
        {
            if (Equals(log.TIP, ServerLog.EventType.T1))
            {
                var point = new InvestigatePieceAbstract.ChartData.Points(log.serverTime, log.QSize);
                chartData.pointsList.Add(point);
            }
        }
        
        GameEvents.OnBuildChart?.Invoke(chartData);
    }

    private List<ServerLog> ImitateServer()
    {
        float serverTime = 0;
        float T1 = 0;
        float T2 = 0;
        bool serverIsBusy = false;
        int QSize = 0;
        int curTask = 0;

        System.Random rng = new System.Random();
        float Ax;
        float Bx;
        ServerLog.EventType TIP = ServerLog.EventType.T1;

        var serverLogList = new List<ServerLog>();
        var curServerLog = new ServerLog();
        
        while (curTask < m_labData.k)
        {
            Ax = 0f;
            Bx = 0f;
            if (T1 < T2 || T1 == 0)
            {
                serverTime = T1;
                Ax = (float) ProbDistFuncModel.GenerateErlang(rng, 5, m_labData.lambda);
                T1 += Ax;

                if (!serverIsBusy)
                {
                    serverIsBusy = true;
                    Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);
                    T2 = serverTime + Bx;
                }
                else
                {
                    QSize += Math.Min(QSize+1, m_qMaxSize);
                }

                TIP = ServerLog.EventType.T1;
            }
            else if (T1 > T2)
            {
                serverTime = T2;
                Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);
                
                if (QSize == 0)
                {
                    serverIsBusy = false;
                    T2 = T1 + Bx;
                }
                else if (QSize > 0)
                {
                    QSize--;
                    T2 = serverTime + Bx;
                }
                
                TIP = ServerLog.EventType.T2;
            }

            curServerLog.SetValues(TIP, serverTime, serverIsBusy, QSize, curTask, Ax, Bx);
            serverLogList.Add(curServerLog);
        }

        return serverLogList;
    }

    private void SetDependencyValue(Lab1DataSO.DependencyValue lab1DDependencyValue, float value)
    {
        switch (lab1DDependencyValue)
        {
            case Lab1DataSO.DependencyValue.lambda:
                m_labData.lambda = value;
                break;
            case Lab1DataSO.DependencyValue.D:
                m_labData.D = value;
                break;
            case Lab1DataSO.DependencyValue.mean:
                m_labData.mean = value;
                break;
            case Lab1DataSO.DependencyValue.std_dev:
                m_labData.std_dev = value;
                break;
        }
    }
    
    private float GetDependencyValue(Lab1DataSO.DependencyValue lab1DDependencyValue)
    {
        switch (lab1DDependencyValue)
        {
            case Lab1DataSO.DependencyValue.lambda:
                return m_labData.lambda;
            case Lab1DataSO.DependencyValue.D:
                return m_labData.D;
            case Lab1DataSO.DependencyValue.mean:
                return m_labData.mean;
            case Lab1DataSO.DependencyValue.std_dev:
                return m_labData.std_dev;
        }

        return -1;
    }
}
