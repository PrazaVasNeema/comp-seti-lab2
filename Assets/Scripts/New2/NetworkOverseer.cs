using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NetworkOverseer : MonoBehaviour
{
    public static NetworkOverseer Instance { get; private set; }

    private void OnEnable()
    {
        Instance = this;
    }

    public class EventData
    {
        public ServerNode serverNode;
        public NDT.ServerLog.EventType eventType;
        public float expectedTime;
        
        public EventData(ServerNode serverNode, NDT.ServerLog.EventType eventType, float expectedTime)
        {
            this.serverNode = serverNode;
            this.eventType = eventType;
            this.expectedTime = expectedTime;
        }
    }
    
    [SerializeField] private Lab1DataSO m_labDataSO;
    public Lab1DataSO.Data labData => m_labDataSO.data;
    
    private CancellationTokenSource cancelTokenSource;
    
    private List<ServerNode> serverNodes = new List<ServerNode>();
    private List<EventData> eventQueueList = new List<EventData>();
    private float currentClosestEventTime = float.MaxValue;
    // private List<List<NDT.ServerLog>> resultServerLogListList;
    private Dictionary<int, List<NDT.ServerLog>> resultServerLogListDict;
    
    public float serverTime { get; private set; }
    public float m_progressBarFillRate { get; private set; }
    public int totalTaskCount { get; private set; }
    public int processedTaskCount { get; private set; }

    [SerializeField] private GameObject m_progressBarGO;
    [SerializeField] private Image m_progressBar;
    private void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            m_progressBar.fillAmount = m_progressBarFillRate;
        }
    }
    
    public void IncreaseTotalTaskCount()
    {
        totalTaskCount++;
    }
    public void IncreaseProcessedTaskCount()
    {
        processedTaskCount++;
    }
    
    // A method to add an event to the eventQueue for a certain serverNode with a certain eventType and the lowest value
    public void AddEvent(EventData eventData)
    {

        
        if (eventData.expectedTime < currentClosestEventTime)
        {
            
            eventQueueList.Add(eventData);
            
            SetCurrentClosestEventTime(eventData.expectedTime);
        }
        else
        {
            bool notFound = true;
            for(int i = eventQueueList.Count -1; i >= 1; i--)
            {
                if (eventData.expectedTime > eventQueueList[i].expectedTime && eventData.expectedTime < eventQueueList[i-1].expectedTime)
                {
                    eventQueueList.Insert(i, eventData);
                    notFound = false;
                    break;
                }
            }
            if (notFound)
            {
                eventQueueList.Insert(0, eventData);
            }
        }
    }
    
    private void SetCurrentClosestEventTime(float time)
    {
        currentClosestEventTime = time;
    }
    
    public void IncreaseServerTime(float time)
    {
        serverTime += time;
    }
    
    private void Start()
    {
        cancelTokenSource = new CancellationTokenSource();
        InitializeEmulation();
    }

    private void InitializeEmulation()
    {

        for (int i = 0; i < labData.nodesCount; i++)
        {
            var newServerNode = new ServerNode();
            serverNodes.Add(newServerNode);

            var eventData = new EventData(newServerNode, NDT.ServerLog.EventType.T1, newServerNode.GetAx());
            
            AddEvent(eventData);
        }
        
        CreateGraphAndFillNeighbours();
        
    }
    
    private void CreateGraphAndFillNeighbours()
    {
        
        if (serverNodes.Count == 1)
            return;
        var neighboursList = new List<ServerNode>();
        neighboursList.Add(serverNodes[1]);
        serverNodes[0].FillServerNeighbours(neighboursList);
        
        for (int i = 1; i < serverNodes.Count - 1; i++)
        {
            neighboursList = new List<ServerNode>();
            neighboursList.Add(serverNodes[i - 1]);
            neighboursList.Add(serverNodes[i + 1]);
            serverNodes[i].FillServerNeighbours(neighboursList);
        }
        
        // ??
        
        neighboursList = new List<ServerNode>();
        neighboursList.Add(serverNodes[^2]);
        serverNodes[^1].FillServerNeighbours(neighboursList);
        
    }

    public async void DoEmulation()
    {
        GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Grey);

        resultServerLogListDict = new Dictionary<int, List<NDT.ServerLog>>();
        m_progressBarFillRate = 0f;

        // TotalClearData();
        
        resultServerLogListDict = await Task.Run(() =>
        {
            var serverLogListDict = new Dictionary<int, List<NDT.ServerLog>>();
            // serverLogListDict.Add(0, new List<NDT.ServerLog>());
            for (int i = 0; i < serverNodes.Count; i++)
            {
                // serverLogListDict[i].Add(new NDT.ServerLog());
                serverLogListDict.Add(i, new List<NDT.ServerLog>());
            }


            for (int j = 0; j < labData.iterAmount; j++)
            {
                TotalClearData();
                for (int i = 0; i < serverNodes.Count; i++)
                {
                    var eventData = new EventData(serverNodes[i], NDT.ServerLog.EventType.T1, serverNodes[i].GetAx());
                    AddEvent(eventData);

                    serverLogListDict[i].Add(new NDT.ServerLog());

                    serverNodes[i].thisServerLog = serverLogListDict[i][j];
                }

                while (processedTaskCount < labData.k)
                {
                    // Debug.Log($"eventQueueList.Count = {eventQueueList.Count}");
                    var eventData = eventQueueList[^1];
                    eventQueueList.RemoveAt(eventQueueList.Count - 1);
                    currentClosestEventTime = eventQueueList[^1].expectedTime;
                    // get index of the serverNode from eventData in the serverNodes list

                    var serverNodeIndex = serverNodes.FindIndex(x => x == eventData.serverNode);
                    var serverLog = serverLogListDict[serverNodeIndex][j];

                    serverTime = eventData.expectedTime;

                    bool isGood = eventData.serverNode.DoTask(eventData.eventType, ref serverLog, this);

                    if (!isGood)
                    {
                        // m_progressBarGO.SetActive(false);
                        return null;
                    }

                    // m_progressBarFillRate = Mathf.Lerp(0f, 0.5f, (float)j / m_labData.iterAmount);
                    
                    // Debug.Log($"processedTaskCount = {processedTaskCount}");

                    if (cancelTokenSource.IsCancellationRequested)
                        return null;
                }
                
                m_progressBarFillRate = Mathf.Lerp(0f, 0.5f, (float) j / labData.iterAmount);


            }

            return serverLogListDict;
        });
        
        // !!

        if (resultServerLogListDict == null)
        {
            GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Red);

            return;
        }


        Debug.Log("Done!");
        
        var processDataProbabilityGauss = new ProcessDataProbability(ProcessDataProbability.FuncEnum.Gauss);
        var processDataProbabilityErland5D = new ProcessDataProbability(ProcessDataProbability.FuncEnum.Erland5D);
        var processDataProbabilityUx = new ProcessDataProbabilityUx();
        var processDataProbabilityPprostoi = new ProcessDataProbabilityPprostoi();

        
        var viewDataList = new List<NDT.ViewData>();
        
        var theDataResult = await Task.Run(() =>
        {
            float iterStep = 0.5f / serverNodes.Count; 
            for (int i = 0; i < serverNodes.Count; i++)
            {
                viewDataList.Add(processDataProbabilityGauss.GetViewData(resultServerLogListDict[i]));
                viewDataList.Add(processDataProbabilityErland5D.GetViewData(resultServerLogListDict[i]));
                viewDataList.Add(processDataProbabilityUx.GetViewData(resultServerLogListDict[i]));
                processDataProbabilityPprostoi.GetViewData(resultServerLogListDict[i], labData);

                m_progressBarFillRate += iterStep;
            }
            

            return viewDataList;
        });
            

        
        // !!
        
        // foreach (var theData in theDataResult)
        // {
        //     GameEvents.OnBuildView?.Invoke(theData);
        // }
        
        GameEvents.OnChangeAuxParamsView?.Invoke(++DataView.curParamsValuesCount, labData);
        
        GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Green);
        
    }
    
    private void OnDestroy()
    {
        cancelTokenSource.Cancel();
    }

    private void TotalClearData()
    {
        eventQueueList = new List<EventData>();
        currentClosestEventTime = float.MaxValue;
        serverTime = 0f;
        totalTaskCount = 0;
        processedTaskCount = 0;

        ClearServerNodesData();
    }

    private void ClearServerNodesData()
    {
        for (int i = 0; i < serverNodes.Count; i++)
        {
            serverNodes[i].ClearServerData();
        }
    }
    
}
