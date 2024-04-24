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
        public QBuffer qBuffer;
        public int curTask;
        public float AxValue;
        public float BxValue;

        public void SetValues(EventType TIP, float serverTime, bool serverIsBusy, QBuffer qBuffer, int curTask, float AxValue,
            float BxValue)
        {
            this.TIP = TIP;
            this.serverTime = serverTime;
            this.serverIsBusy = serverIsBusy;
            this.qBuffer = qBuffer;
            this.curTask = curTask;
            this.AxValue = AxValue;
            this.BxValue = BxValue;
        }
    }
    
    public class QBuffer
    {
        public class QContent
        {
            public int taskNum;
            public float arrivalTime;
        }
        
        private List<QContent> qContentsList = new List<QContent>();
        private int currentIndex = 0;
        
        public int bufferCount => qContentsList.Count;
        
        public QContent RemoveTask(int taskNum)
        {
            var output = qContentsList.Find(x => x.taskNum == taskNum);
            qContentsList.RemoveAll(x => x.taskNum == taskNum);

            return output;
        }
        
        public QContent RemoveTaskByIndex(int index)
        {
            if (index < 0 || index >= qContentsList.Count)
                return null;
            
            var output = qContentsList[index];
            qContentsList.RemoveAt(index);

            return output;
        }
        
        // Реализуй выбор элемента из списка qContentsList, использующий RoundRobin

        public QContent GetTaskRoundRobin()
        {
            if (qContentsList.Count == 0)
                return null;
            
            currentIndex = ++currentIndex % qContentsList.Count;

            return RemoveTaskByIndex(currentIndex);
        }
        
        public void AddTask(QContent qContent)
        {
            qContentsList.Add(qContent);
        }
        
        // Method to clone data into another instance of that class
        public QBuffer Clone()
        {
            QBuffer newQBuffer = new QBuffer();
            newQBuffer.qContentsList = new List<QContent>(qContentsList);
            newQBuffer.currentIndex = currentIndex;

            return newQBuffer;
        }
        
    }



    [SerializeField] private ChartView m_ChartView;
    
    [SerializeField] private Lab1DataSO m_labDataSO;
    private Lab1DataSO.Data m_labData => m_labDataSO.data;
    // [SerializeField] private int m_serverCores;
    [SerializeField] private int m_qMaxSize;
    
    public void Calculate()
    {
        float dependencyParamDefValue = GetDependencyValue(m_labData.dependencyValue);
    
        int totalIters = (int)((m_labData.to - m_labData.from) / m_labData.step) + 2;
        List<InvestigatePieceAbstract> investigatePiecesList = new List<InvestigatePieceAbstract>();
        
        investigatePiecesList.Add(new InvestigateErland5D());
        investigatePiecesList.Add(new InvestigateAbsGauss());

        
        
        // Debug.Log($"valuesLength: {(int) ((m_labData.to - m_labData.from) / m_labData.step)}");
    
        int arrayIter = -1;
        for (float i = m_labData.from; i <= m_labData.to; i += m_labData.step)
        {
            arrayIter++;
            SetDependencyValue(m_labData.dependencyValue, i);
    
            float sumIters = 0;
            for (int j = 0; j <= m_labData.iterAmount; j++)
            {
    
                //////

                var serverLog = ImitateServer();

                if (serverLog == null)
                {
                    SetDependencyValue(m_labData.dependencyValue, dependencyParamDefValue);
                    return;
                }

                foreach (var investigatePiece in investigatePiecesList)
                {
                    investigatePiece.InvestigateOne(serverLog);
                }
    
                // sumTasks /= m_labData.k;
    
    
            }

            foreach (var investigatePiece in investigatePiecesList)
            {
                investigatePiece.InvestigateTwo(m_labData.iterAmount, i);
            }
            
    
        }

        foreach (var investigatePiece in investigatePiecesList)
        {
            var chartData = investigatePiece.InvestigateFinal(m_labData.dependencyValue);
            
            GameEvents.OnBuildChart?.Invoke(chartData);
        }
    
        SetDependencyValue(m_labData.dependencyValue, dependencyParamDefValue);
    
        // foreach (var value in valuesArray)
        // {
        //     Debug.Log($"P_Prostoi: {value}");
        // }
        
        // m_ChartView.UpdateChart(chartDataList);
        
        // Debug.Log(chartDataList.Count);
        
    }

    public void ImitateServerActivity()
    {
        var serverLogList = ImitateServer();

        if (serverLogList == null)
        {
            return;
        }
        
        var chartData = new InvestigatePieceAbstract.ChartData();
        chartData.xAxisName = Lab1DataSO.DependencyValue.justTime;
        chartData.targetChart = InvestigatePieceAbstract.TargetChart.ServerActivity;

        foreach (var log in serverLogList)
        {
            Debug.Log(" ---------------------- SERVER LOG START ---------------------- ");
            Debug.Log($"TIP: {log.TIP}, serverTime: {log.serverTime}, serverIsBusy: {log.serverIsBusy}, qBuffer: {log.qBuffer.bufferCount}, curTask: {log.curTask}, Ax: {log.AxValue}, Bx: {log.BxValue}");
            if (Equals(log.TIP, ServerLog.EventType.T1))
            {
                var point = new InvestigatePieceAbstract.ChartData.Points(log.serverTime, log.qBuffer.bufferCount + (log.serverIsBusy ? 1 : 0));
                chartData.pointsList.Add(point);
            }
        }
        
        GameEvents.OnBuildChart?.Invoke(chartData);
    }

    private List<ServerLog> ImitateServer()
    {
        // if(m_labData.lambda)
        
        float serverTime = 0;
        float T1 = 0;
        float T2 = 0;
        bool serverIsBusy = false;
        QBuffer qBuffer = new QBuffer();
        int compTasksCount = 0;
        int totalTasksCount = 0;

        System.Random rng = new System.Random();
        float Ax;
        float Bx;
        ServerLog.EventType TIP = ServerLog.EventType.T1;
        int curProcessedTask = -1;

        var serverLogList = new List<ServerLog>();
        // var curServerLog = new ServerLog();

        int totalIters = 0;
        
        
        while (compTasksCount < m_labData.k)
        {
            Ax = 0f;  
            Bx = 0f; 
            if (T1 < T2 || T1 == 0)
            {
                serverTime = T1;
                Ax = (float) ProbDistFuncModel.GenerateErlang(rng, 5, m_labData.lambda, m_labData.D);
                T1 += Ax;

                totalTasksCount++;

                if (!serverIsBusy)
                {
                    serverIsBusy = true;
                    Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);
                    T2 = serverTime + Bx;

                    curProcessedTask = totalTasksCount;
                }
                else
                {
                    if (qBuffer.bufferCount < m_qMaxSize)
                    {
                        var qContent = new QBuffer.QContent
                        {
                            taskNum = totalTasksCount,
                            arrivalTime = serverTime
                        };
                        qBuffer.AddTask(qContent);
                    }
                    else
                    {
                        Debug.Log("Buffer is full");
                    }
                }

                TIP = ServerLog.EventType.T1;
            }
            else if (T1 > T2)
            {
                serverTime = T2;
                Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);
                
                if (qBuffer.bufferCount == 0)
                {
                    serverIsBusy = false;
                    T2 = T1 + Bx;
                    curProcessedTask = -1;
                }
                else if (qBuffer.bufferCount > 0)
                {
                    var newTaskFromBuffer = qBuffer.GetTaskRoundRobin();
                    curProcessedTask = newTaskFromBuffer.taskNum;
                    T2 = serverTime + Bx;
                }
                
                
                
                TIP = ServerLog.EventType.T2;
                compTasksCount++;
            }
            // Debug.Log($"1: TIP: {TIP}, serverTime: {serverTime}, serverIsBusy: {serverIsBusy}, qBuffer: {qBuffer.bufferCount}, curTask: {curProcessedTask}, Ax: {Ax}, Bx: {Bx}");

            var curServerLog = new ServerLog();
            curServerLog.SetValues(TIP, serverTime, serverIsBusy, qBuffer.Clone(), curProcessedTask, Ax, Bx);
            serverLogList.Add(curServerLog);


            totalIters++;
            if (totalIters > 100000000)
            {
                Debug.LogError("Infinite loop occured");
                return null;
            }

        }

        Debug.Log(serverLogList);
        
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
