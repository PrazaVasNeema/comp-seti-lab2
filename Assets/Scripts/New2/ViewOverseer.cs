using System.Collections;
using System.Collections.Generic;
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
    
    private void OnEnable()
    {
        GameEvents.OnLoadDataViewOverseer += OnLoadDataViewOverseer;
        GameEvents.OnInitNodes += OnInitNodes;
    }

    private void OnDisable()
    {
        GameEvents.OnLoadDataViewOverseer -= OnLoadDataViewOverseer;
        GameEvents.OnInitNodes -= OnInitNodes;
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
        
        string neighboursFinalText = "";
        foreach (var neighbourId in m_nodesNeighboursList[m_currentNodeIndex])
        {
            neighboursFinalText += neighbourId + ", ";
        }
        m_neighboursText.text = neighboursFinalText;

        
        GameEvents.OnClearViewData?.Invoke();
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
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
