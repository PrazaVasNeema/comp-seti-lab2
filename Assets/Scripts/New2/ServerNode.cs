using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public class ServerNode
{
    
    private List<ServerNode> nodeNeighbours = new List<ServerNode>();
    
    // int compTasksCount = 0;
    int totalTasksIncomeCount = 0;
    Random rng = new System.Random();
    private NDT.ServerLog.EventType TIP;
    NDT.TaskData curProcessedTask = null;

    private int AxCounter = 0;
    private int BxCounter = 0;
    private float totalWaitingTime = 0;
    private float totalProcessedTime = 0;
    private float Mtau = 0;
    private float Msigma = 0;
    private int m_qMaxSize = 8;
    NDT.QBuffer qBuffer = new NDT.QBuffer(8);
    
    private int totalIncomeTasksCount = 0;


    private float Ax;
    private float Bx;
    int totalIters = 0;

    public NDT.ServerLog thisServerLog;

    public int thisID = -1;

    public void FillServerNeighbours(List<ServerNode> neighboursList)
    {
        nodeNeighbours = new List<ServerNode>();
        
        nodeNeighbours = neighboursList;

        var str = "";
        foreach (var VARIABLE in neighboursList)
        {
            str += VARIABLE.thisID + " ";
        }
        
        Debug.Log($"This is {thisID}, My neighbours are: {str}");
    }

    public void ClearServerData()
    {
        AxCounter = 0;
        BxCounter = 0;
        totalWaitingTime = 0;
        totalProcessedTime = 0;
        Mtau = 0;
        Msigma = 0;
        totalIters = 0;
        
        qBuffer = new NDT.QBuffer(8);
        curProcessedTask = null;
        totalIncomeTasksCount = 0;


    }

    public bool DoTask(NDT.ServerLog.EventType eventType, ref NDT.ServerLog serverLog, NetworkOverseer overseer)
    {
        Ax = 0;
        Bx = 0;
        if (eventType == NDT.ServerLog.EventType.T1)
        {
            Ax = GetAx();
            
            var eventData = new NetworkOverseer.EventData(this, eventType, overseer.serverTime + Ax);
            overseer.AddEvent(eventData);
            
            totalTasksIncomeCount++;
            overseer.IncreaseTotalTaskCount();
            var newTask = new NDT.TaskData{num = overseer.totalTaskCount, arrivalTime = overseer.serverTime};
            serverLog.alltimeTaskList.Add(newTask);

            if (qBuffer.bufferCount == 0 && curProcessedTask == null)
            {
                Bx = GetBx();
                
                var eventData2 = new NetworkOverseer.EventData(this, NDT.ServerLog.EventType.T2, overseer.serverTime + Bx);
                overseer.AddEvent(eventData2);
                curProcessedTask = newTask;
            }
            else if(qBuffer.bufferCount < m_qMaxSize)
            {
                AddTaskToBuffer(newTask);
                // qBuffer.AddTask(newTask);
            }
            
            TIP = NDT.ServerLog.EventType.T1;
        }
        else
        {
            // Debug.Log($"curProcessedTask = {curProcessedTask}");
            serverLog.SetTaskFinishTime(curProcessedTask, overseer.serverTime);
            
            DecideTaskDestiny(curProcessedTask);

            switch (qBuffer.bufferCount)
            {
                case 0:
                    curProcessedTask = null;
                    break;
                case > 0:
                    var newTaskFromBuffer = qBuffer.GetTaskRoundRobin();
                    curProcessedTask = newTaskFromBuffer;
                    Bx = GetBx();
                    var eventData2 = new NetworkOverseer.EventData(this, NDT.ServerLog.EventType.T2, overseer.serverTime + Bx);
                    overseer.AddEvent(eventData2);
                    break;
            }
            
            overseer.IncreaseProcessedTaskCount();
            
            TIP = NDT.ServerLog.EventType.T2;
            // compTasksCount++;
        }
        
        var curServerGeneralLog = new NDT.ServerLog.ServerStatus();
        bool serverIsBusy = qBuffer.bufferCount != 0;
        curServerGeneralLog.SetValues(TIP, overseer.serverTime, serverIsBusy, qBuffer.Clone(), curProcessedTask, Ax, Bx);
        serverLog.serverStatusList.Add(curServerGeneralLog);

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
                return false;
            }
        }

        thisServerLog = serverLog;

        return true;
    }

    // private NDT.ServerLog DoT1()
    // {
    //     
    //     var serverLog = new NDT.ServerLog();
    //     return serverLog;
    //     
    // }
    //
    // private NDT.ServerLog DoT2()
    // {
    //     
    //     var serverLog = new NDT.ServerLog();
    //     return serverLog;
    //     
    // }

    public NetworkOverseer.EventData GenerateEvent(NDT.ServerLog.EventType eventType)
    {
        float increasingValue = eventType == NDT.ServerLog.EventType.T1 ? GetAx() : GetBx();
        
        return new NetworkOverseer.EventData(this, eventType, NetworkOverseer.Instance.serverTime + increasingValue);
    }
    
    public float GetAx()
    {
        return (float) ProbDistFuncModel.GenerateErlang(rng, 5, NetworkOverseer.Instance.labData.lambda, NetworkOverseer.Instance.labData.D);
    }
    
    public float GetBx()
    {
        return (float)ProbDistFuncModel.GenerateNormal(rng, NetworkOverseer.Instance.labData.mean, NetworkOverseer.Instance.labData.std_dev);
    }
    
    // --- TOTALLY NEW ---
    
    
    public void AddTaskToBuffer(NDT.TaskData task)
    {
        qBuffer.AddTask(task);
        thisServerLog.allTimeIncomeTasksCount++;
    }
    
    private void DecideTaskDestiny(NDT.TaskData task)
    {
        if (nodeNeighbours.Count == 0)
        {
            // Debug.LogError("No neighbours found");
            return;
        }
        
        float p = (float) rng.NextDouble();
        
        if (p > NetworkOverseer.Instance.labData.pToPassTask)
        {
            return;
        }
        
        SendTaskToNeighbour(task);
    }

    private void SendTaskToNeighbour(NDT.TaskData task)
    {
        float minProcessTime = float.MaxValue;
        ServerNode minProcessTimeNode = null;
        
        foreach (var neighbour in nodeNeighbours)
        {
            float totalTaskTime = 0f;
            int tasksCounter = 0;

            if (neighbour.thisServerLog.alltimeTaskList.Count == 0)
            {
                // minProcessTimeNode = neighbour;
                // break;
                continue;
            }
            
            // Else normal case
            foreach (var taskData in neighbour.thisServerLog.alltimeTaskList)
            {
                if (taskData.finishTime == -1f)
                    continue;
                totalTaskTime += taskData.finishTime - taskData.arrivalTime;
                tasksCounter++;
            }
            
            var thisTime = totalTaskTime / tasksCounter;
            // Debug.Log($"thisTime = {thisTime}");
            if (thisTime < minProcessTime)
            {
                minProcessTime = thisTime;
                minProcessTimeNode = neighbour;
            }
        }

        if (minProcessTimeNode != null)
        {
            minProcessTimeNode.AddTaskToBuffer(task);
        }
    }
    
}
