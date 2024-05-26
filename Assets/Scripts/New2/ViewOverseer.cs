using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ViewOverseer : MonoBehaviour
{

    public class NodeViewData
    {
        public int nodeViewID = -1;
        public List<NDT.ViewData> viewDataList = new List<NDT.ViewData>();
    }
    
    [SerializeField] TMP_Dropdown m_nodesDropdown;
    private List<List<int>> m_nodesNeighboursList = new List<List<int>>();
    private List<List<NodeViewData>> m_nodeViewDataList = new List<List<NodeViewData>>();
    
    private List<List<Vector2>> m_resultParamsDataListList = new List<List<Vector2>>();
    
    private void OnEnable()
    {
        GameEvents.OnLoadDataViewOverseer += OnLoadDataViewOverseer;
        GameEvents.OnInitNodes += OnInitNodes;
        GameEvents.OnAddResultAuxParamsDataList += OnLoadResultParamsData;
    }

    private void OnDisable()
    {
        GameEvents.OnLoadDataViewOverseer -= OnLoadDataViewOverseer;
        GameEvents.OnInitNodes -= OnInitNodes;
        GameEvents.OnAddResultAuxParamsDataList -= OnLoadResultParamsData;
    }
    
    private void OnInitNodes(List<List<int>> obj)
    {
        m_nodesDropdown.ClearOptions();
        m_nodesNeighboursList = obj;
        var options = new List<string>();
        options.Add($"Select a Node if any");
        for (int i = 0; i < obj.Count; i ++)
        {
            options.Add($"ServerNode {i}");
        }
        m_nodesDropdown.AddOptions(options);
        m_nodesDropdown.value = 0;
    }
    
    private void OnLoadDataViewOverseer(List<NodeViewData> obj)
    {
        m_nodeViewDataList.Add(obj);
    }

    // Buttons
    
    public void OnStartEmulation()
    {

        ClearInterExpData();
        
        GameEvents.OnDoFullProcess?.Invoke();

    }

    public void ClearAllData()
    {
        ClearInterExpData();
        
        GameEvents.OnClearAllAuxParams?.Invoke();
        
        m_nodeViewDataList = new List<List<NodeViewData>>();
        m_resultParamsDataListList = new List<List<Vector2>>();

        GameEvents.OnClearResultAuxParamsView?.Invoke();


    }
    
    // Buttons
    
    private void ClearInterExpData()
    {
        GameEvents.OnClearViewData?.Invoke();
        
        m_nodesDropdown.ClearOptions();
        var options = new List<string>();
        options.Add($"Select a Node if any");
        m_nodesDropdown.AddOptions(options);
        m_nodesDropdown.value = 0;
        
        m_neighboursText.text = "";
        m_currentNodeIndex = -1;
        m_nodesNeighboursList = new List<List<int>>();

        GameEvents.OnClearResultAuxParamsView?.Invoke();
        
    }
    
    // ----

    [SerializeField] private TMP_Text m_neighboursText;
    private int m_currentNodeIndex = -1;

    
    
    
    public void OnUpdateNodeDataViaDropdown()
    {
        if (m_nodesDropdown.value == 0)
            return;

        OnUpdateNodeDataCoreLogic(m_nodesDropdown.value - 1);
    }
    
    public void OnUpdateNodeDataCoreLogic(int index)
    {
        m_currentNodeIndex = index;
        
        if (m_nodesDropdown.value != m_currentNodeIndex)
        {
            m_nodesDropdown.value = m_currentNodeIndex + 1;
        }
        
        string neighboursFinalText = "";
        foreach (var neighbourId in m_nodesNeighboursList[m_currentNodeIndex])
        {
            neighboursFinalText += neighbourId + ", ";
        }
        m_neighboursText.text = neighboursFinalText;

        
        GameEvents.OnClearViewData?.Invoke();
        GameEvents.OnClearResultAuxParamsView?.Invoke();
        foreach (var nodeViewDataListIter in m_nodeViewDataList)
        {
            if (nodeViewDataListIter.Count <= m_currentNodeIndex)
            {
                foreach (var emptyViewData in NDT.ViewData.GetEmptyViewDataList())
                {
                    GameEvents.OnBuildView?.Invoke(emptyViewData);
                }
                continue;
            }

            foreach (var viewData in nodeViewDataListIter[m_currentNodeIndex].viewDataList)
            {
                GameEvents.OnBuildView?.Invoke(viewData);
            }
        }

        int i = -1;
        
        foreach (var paramsList in m_resultParamsDataListList)
        {
            i++;
            if (paramsList.Count <= m_currentNodeIndex)
            {
                GameEvents.OnChangeResultAuxParamsView?.Invoke(i, new UnityEngine.Vector2(-1, -1));
                continue;
            }
            
            GameEvents.OnChangeResultAuxParamsView?.Invoke(i, paramsList[m_currentNodeIndex]);
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    // ---

    private void OnLoadResultParamsData(List<Vector2> obj)
    {
        m_resultParamsDataListList.Add(obj);
        SaveParamsValues(obj, Application.dataPath + "/params.txt");
    }
    
    public void SaveParamsValues(List<Vector2> obj, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("case: " + m_resultParamsDataListList.Count);
            foreach (var vector2 in obj)
            {
                writer.WriteLine(ResultAuxParamsValues.GetFirstNDigitsIncludingDecimal(vector2.x, 4) + " " + ResultAuxParamsValues.GetFirstNDigitsIncludingDecimal(vector2.y, 4));
            }
           
        }
    }
}
