using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Server : MonoBehaviour
{
    
    public static Server Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
    }

    [FormerlySerializedAs("m_ChartView")] [SerializeField] private DataView dataView;
    
    [SerializeField] private Lab1DataSO m_labDataSO;
    private Lab1DataSO.Data m_labData => m_labDataSO.data;
    // [SerializeField] private int m_serverCores;
    [SerializeField] private int m_qMaxSize;

    private CancellationTokenSource cancelTokenSource;

    private bool m_shouldPrintData = false;

    [SerializeField] private GameObject m_progressBarGO;
    [SerializeField] private Image m_progressBar;
    private float m_progressBarFillRate = 0;

    private void Start()
    {
        cancelTokenSource = new CancellationTokenSource();
    }

    private void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            m_progressBar.fillAmount = m_progressBarFillRate;
        }
    }

    private void OnDestroy()
    {
        cancelTokenSource.Cancel();
    }


    public async void Calculate()
    {
        var processDataProbabilityGauss = new ProcessDataProbability(ProcessDataProbability.FuncEnum.Gauss);
        var processDataProbabilityErland5D = new ProcessDataProbability(ProcessDataProbability.FuncEnum.Erland5D);
        
        var processDataProbabilityUx = new ProcessDataProbabilityUx();
        var processDataProbabilityPprostoi = new ProcessDataProbabilityPprostoi();

        GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Grey);
        
        // m_progressBarGO.SetActive(true);

        
        // Debug.Log($"valuesLength: {(int) ((m_labData.to - m_labData.from) / m_labData.step)}");


        var resultServerLogList = await Task.Run(() =>
        {
            var serverLogList = new List<NDT.ServerLog>();
            for (int j = 0; j < m_labData.iterAmount; j++)
            {
                var serverLog = ImitateServer();
                if (serverLog == null)
                {
                    GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Red);
                    // m_progressBarGO.SetActive(false);
                    return null;
                }
                serverLogList.Add(serverLog);

                m_progressBarFillRate = Mathf.Lerp(0f, 0.5f, (float)j / m_labData.iterAmount);

                if (cancelTokenSource.IsCancellationRequested)
                    return null;
            }

            return serverLogList;
        });
        
        if(resultServerLogList == null)
            return;

        var TheData = new TheData();

        var viewDataList = new List<NDT.ViewData>();
        
        

        var theDataResult = await Task.Run(() =>
        {
            viewDataList.Add(processDataProbabilityGauss.GetViewData(resultServerLogList));
            m_progressBarFillRate += 0.5f / 3f;
            viewDataList.Add(processDataProbabilityErland5D.GetViewData(resultServerLogList));
            m_progressBarFillRate += 0.5f / 3f;
            viewDataList.Add(processDataProbabilityUx.GetViewData(resultServerLogList));
            m_progressBarFillRate += 0.5f / 3f;

            return viewDataList;
        });
        
        if(resultServerLogList == null)
            return; 

        foreach (var theData in theDataResult)
        {
            GameEvents.OnBuildView?.Invoke(theData);
        }
        
        // NDT.ViewData viewData;
        // viewData = processDataProbabilityGauss.GetViewData(resultServerLogList);
        // GameEvents.OnBuildView?.Invoke(viewData);
        // viewData = processDataProbabilityErland5D.GetViewData(resultServerLogList);
        // GameEvents.OnBuildView?.Invoke(viewData);
        // viewData = processDataProbabilityUx.GetViewData(resultServerLogList);
        // GameEvents.OnBuildView?.Invoke(viewData);
        // processDataProbabilityPprostoi.GetViewData(resultServerLogList, m_labData);
        // GameEvents.OnBuildView?.Invoke(viewData);
        GameEvents.OnChangeAuxParamsView?.Invoke(++DataView.curParamsValuesCount, m_labData);
        
        GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Green);
        

    }

    private class TheData
    {
        public ProcessDataProbability processDataProbabilityGauss;
        public ProcessDataProbability processDataProbabilityErland5D;

        public ProcessDataProbabilityUx processDataProbabilityUx;

        public TheData()
        {
            var processDataProbabilityGauss = new ProcessDataProbability(ProcessDataProbability.FuncEnum.Gauss);
            var processDataProbabilityErland5D = new ProcessDataProbability(ProcessDataProbability.FuncEnum.Erland5D);
        
            var processDataProbabilityUx = new ProcessDataProbabilityUx();
        }


    }

    // public void ImitateServerActivity()
    // {
    //     var serverLog = ImitateServer();
    //
    //     if (serverLog == null)
    //     {
    //         return;
    //     }
    //     
    //     var chartData = new InvestigatePieceAbstract.ChartData();
    //     chartData.xAxisName = Lab1DataSO.DependencyValue.justTime;
    //     chartData.targetChart = InvestigatePieceAbstract.TargetChart.ServerActivity;
    //     
    //     Debug.Log(" ---------------------- SERVER LOG START ---------------------- ");
    //     foreach (var log in serverLog.generalLogDatasList)
    //     {
    //         Debug.Log($"TIP: {log.TIP}, serverTime: {log.serverTime}, serverIsBusy: {log.serverIsBusy}, qBuffer: {log.qBuffer.bufferCount}, curTask: {log.curTask}, Ax: {log.AxValue}, Bx: {log.BxValue}");
    //         if (Equals(log.TIP, ServerLog.EventType.T1))
    //         {
    //             var point = new InvestigatePieceAbstract.ChartData.Points(log.serverTime, log.qBuffer.bufferCount + (log.serverIsBusy ? 1 : 0));
    //             chartData.pointsList.Add(point);
    //         }
    //     }
    //     
    //     GameEvents.OnBuildView?.Invoke(chartData);
    // }

    private NDT.ServerLog ImitateServer()
    {
        // if(m_labData.lambda)
        
        float serverTime = 0;
        float T1;
        float T2 = float.MaxValue;
        bool serverIsBusy = false;
        var qBuffer = new NDT.QBuffer(m_qMaxSize);
        int compTasksCount = 0;
        int totalTasksCount = 0;

        var rng = new System.Random();
        float Ax = 0f;
        float Bx = 0f;
        var TIP = NDT.ServerLog.EventType.T1;
        NDT.TaskData curProcessedTask = null;

        var serverLog = new NDT.ServerLog();
        // var curServerLog = new ServerLog();

        int totalIters = 0;

        int AxCounter = 0;
        int BxCounter = 0;
        float totalWaitingTime = 0;
        float totalProcessedTime = 0;
        float Mtau = 0;
        float Msigma = 0;
        
        Ax = (float) ProbDistFuncModel.GenerateErlang(rng, 5, m_labData.lambda, m_labData.D);
        T1 = Ax;
        
        
        while (compTasksCount < m_labData.k)
        {
            Ax = 0f;  
            Bx = 0f; 
            if (T1 < T2)
            {
                serverTime = T1;
                Ax = (float) ProbDistFuncModel.GenerateErlang(rng, 5, m_labData.lambda, m_labData.D);
                T1 += Ax;

                totalTasksCount++;
                
                var newTask = new NDT.TaskData{num = totalTasksCount, arrivalTime = serverTime};
                serverLog.alltimeTaskList.Add(newTask);

                if (!serverIsBusy)
                {
                    serverIsBusy = true;
                    Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);
                    T2 = serverTime + Bx;
                    curProcessedTask = newTask;
                }
                else
                {
                    qBuffer.AddTask(newTask);
                }

                TIP = NDT.ServerLog.EventType.T1;
            }
            else
            {
                serverTime = T2;
                
                serverLog.SetTaskFinishTime(curProcessedTask, serverTime);
                
                switch (qBuffer.bufferCount)
                {
                    case 0:
                        serverIsBusy = false;
                        T2 = float.MaxValue;
                        curProcessedTask = null;
                        break;
                    case > 0:
                    {
                        var newTaskFromBuffer = qBuffer.GetTaskRoundRobin();
                        curProcessedTask = newTaskFromBuffer;
                        Bx = (float)ProbDistFuncModel.GenerateNormal(rng, m_labData.mean, m_labData.std_dev);

                        T2 = serverTime + Bx;
                        break;
                    }
                }
                TIP = NDT.ServerLog.EventType.T2;
                compTasksCount++;
            }
            
            // Debug.Log($"1: TIP: {TIP}, serverTime: {serverTime}, serverIsBusy: {serverIsBusy}, qBuffer: {qBuffer.bufferCount}, curTask: {curProcessedTask}, Ax: {Ax}, Bx: {Bx}");

            var curServerGeneralLog = new NDT.ServerLog.ServerStatus();
            curServerGeneralLog.SetValues(TIP, serverTime, serverIsBusy, qBuffer.Clone(), curProcessedTask, Ax, Bx);
            serverLog.serverStatusList.Add(curServerGeneralLog);
            
            // Debug.Log($"Ax: {Ax}");

            if (Ax > 0)
            {
                totalWaitingTime += Ax;
                AxCounter++;
            }

            if (Bx > 0)
            {
                totalProcessedTime += Bx;
                BxCounter++;
            }
            totalIters++;

            if (BxCounter * AxCounter > 0)
            {
                Msigma = totalProcessedTime / BxCounter;
                Mtau = totalWaitingTime / AxCounter;
                
                if (totalIters > 10 && Msigma / (Mtau*1.5) > 1)
                {
                    Debug.LogError("Infinite loop occured");
                    return null;
                }
            }


            

        }

        
        return serverLog;
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