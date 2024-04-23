using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab1 : MonoBehaviour
{
    public class ChartData
    {
        public float x;
        public float y;
    }

    [SerializeField] private ChartView m_ChartView;
    
    [SerializeField] private Lab1DataSO m_labDataSO;
    private Lab1DataSO.Data m_labData => m_labDataSO.data;
    // [SerializeField] private int m_serverCores;
    [SerializeField] private int m_qMaxSize;
    
    public void Calculate()
    {
        float Ax;
        float Bx;
        float dependencyParamDefValue = GetDependencyValue(m_labData.dependencyValue);

        int totalIters = (int)((m_labData.to - m_labData.from) / m_labData.step) + 2;
        List<ChartData> chartDataList = new List<ChartData>();
        // Debug.Log($"valuesLength: {(int) ((m_labData.to - m_labData.from) / m_labData.step)}");

        int arrayIter = -1;
        for (float i = m_labData.from; i <= m_labData.to; i += m_labData.step)
        {
            arrayIter++;
            SetDependencyValue(m_labData.dependencyValue, i);

            float sumIters = 0;
            for (int j = 0; j <= m_labData.iterAmount; j++)
            {
                float sumTasks = 0;
                System.Random rng = new System.Random();
                float serverTimer = 0;
                float T1 = 0;
                float T2 = 0;
                bool serverIsActive = false;
                int QSize = 0;

                float P_Prostoi = 0;

                for (int h = 0; h <= m_labData.k; h++)
                {
                    if (T1 < T2 || T1 == 0)
                    {
                        serverTimer = T1;
                        Ax = (float) ProbDistFuncModel.GenerateErlang(rng, 5, m_labData.lambda);
                        T1 += Ax;

                        if (!serverIsActive)
                        {
                            serverIsActive = true;
                            Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);
                            T2 = serverTimer + Bx;
                        }
                        else
                        {
                            QSize += Math.Min(QSize+1, m_qMaxSize);
                        }
                    }
                    else if (T1 > T2)
                    {
                        serverTimer = T2;
                        Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);

                        if (QSize == 0)
                        {
                            P_Prostoi += T1 - serverTimer;
                            T2 = T1 + Bx;
                        }
                        else if (QSize > 0)
                        {
                            QSize--;
                            T2 += Bx;
                        }
                    }
                    
                }
                

                // sumTasks /= m_labData.k;

                sumTasks = P_Prostoi / serverTimer;
                sumIters += sumTasks;

            }

            sumIters /= m_labData.iterAmount;

            ChartData newChartData = new ChartData();
            newChartData.x = i;
            newChartData.y = sumIters;
            chartDataList.Add(newChartData);

        }

        SetDependencyValue(m_labData.dependencyValue, dependencyParamDefValue);

        // foreach (var value in valuesArray)
        // {
        //     Debug.Log($"P_Prostoi: {value}");
        // }
        
        m_ChartView.UpdateChart(chartDataList);
        
        Debug.Log(chartDataList.Count);
        
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
