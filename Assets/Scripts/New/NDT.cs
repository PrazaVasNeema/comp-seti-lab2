using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NDT
{
    public class TaskData
    {
        public int num;
        public float arrivalTime;
        public float finishTime = -1;
    }
    public class ServerLog
    {
        public enum EventType
        {
            T1,
            T2
        }
        // All time tasks data
        public List<TaskData> allTimeTasksList = new();
        // A method to set finish time for a task in the tasksList with a certain num
        public void SetTaskFinishTime(int taskNum, float finishTime)
        {
            var task = allTimeTasksList.Find(x => x.num == taskNum);
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
            public int curTask;
            public float AxValue;
            public float BxValue;
            public void SetValues(EventType TIP, float serverTime, bool serverIsBusy, QBuffer qBuffer, int curTask,
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
