using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class NDT
{
    public class TaskData
    {
        public int num;
        public float arrivalTime;
        public float finishTime = -1;
        
        // A method to clone data into another instance of that class
        public TaskData Clone()
        {
            TaskData newTaskData = new TaskData();
            newTaskData.num = num;
            newTaskData.arrivalTime = arrivalTime;
            newTaskData.finishTime = finishTime;
            return newTaskData;
        }
        
        // public TaskData(int num, float arrivalTime)
        // {
        //     this.num = num;
        //     this.arrivalTime = arrivalTime;
        // }
    }
    public class ServerLog
    {
        public enum EventType
        {
            T1,
            T2
        }
        // All time tasks data
        public List<TaskData> alltimeTaskList = new List<TaskData>();
        // A method to set finish time for a task in the tasksList with a certain num
        public void SetTaskFinishTime(TaskData curTask, float finishTime)
        {
            try
            {
                int test = curTask.num;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.LogError("Got null task!");
                throw;
            }
            var task = alltimeTaskList.Find(x => x.num == curTask.num);
            if (task != null) task.finishTime = finishTime;
        }
        // Server status upon event
        public List<ServerStatus> serverStatusList = new();
        public class ServerStatus
        {
            public EventType TIP;
            public float serverTime;
            public bool serverIsBusy;
            public QBuffer qBuffer;
            public TaskData curTask;
            public float AxValue;
            public float BxValue;
            public void SetValues(EventType TIP, float serverTime, bool serverIsBusy, QBuffer qBuffer, TaskData curTask,
                float AxValue,
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
    }
    public class QBuffer
    {
        private List<TaskData> qContentsList = new List<TaskData>();
        private int currentIndex = 0;
        public int bufferCount => qContentsList.Count;
        public TaskData RemoveTask(int taskNum)
        {
            var output = qContentsList.Find(x => x.num == taskNum);
            qContentsList.RemoveAll(x => x.num == taskNum);
            return output;
        }
        public TaskData RemoveTaskByIndex(int index)
        {
            if (index < 0 || index >= qContentsList.Count)
                return null;
            var output = qContentsList[index];
            qContentsList.RemoveAt(index);
            return output;
        }
        // Реализуй выбор элемента из списка qContentsList, использующий RoundRobin
        public TaskData GetTaskRoundRobin()
        {
            if (qContentsList.Count == 0)
                return null;
            currentIndex = ++currentIndex % qContentsList.Count;
            return RemoveTaskByIndex(currentIndex);
        }
        public void AddTask(TaskData qContent)
        {
            qContentsList.Add(qContent);
        }
        // Method to clone data into another instance of that class
        public QBuffer Clone()
        {
            QBuffer newQBuffer = new QBuffer();
            newQBuffer.qContentsList = new List<TaskData>(qContentsList);
            newQBuffer.currentIndex = currentIndex;
            return newQBuffer;
        }
    }
    
    // Chart data
    
    public enum TargetView
    {
        // Ux,
        // P_prostoi,
        Erland,
        Gauss,
        ServerActivity
    }
    public class ViewData
    {
        public enum ViewType
        {
            Chart,
            Table
        }
        
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

        public ViewType type;
        
        public List<Points> pointsList = new List<Points>();
        public TargetView targetView;

        public ViewData(ViewType type, TargetView targetView)
        {
            this.type = type;
            this.targetView = targetView;
        }
    }
}

