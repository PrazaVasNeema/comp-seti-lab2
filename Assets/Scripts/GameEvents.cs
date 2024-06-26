using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static System.Action<NDT.ViewData> OnBuildView;
    public static System.Action<UIController.UIStateAux> OnChangeUIStateAux;
    
    public static System.Action<int, Lab1DataSO.Data> OnChangeAuxParamsView;
    public static System.Action<int> OnClearAuxParamsView;

    
    public static System.Action<List<List<int>>> OnInitNodes;
    public static System.Action OnClearViewData;
    public static System.Action<List<ViewOverseer.NodeViewData>> OnLoadDataViewOverseer;
    

    public static System.Action OnDoFullProcess;

    public static System.Action OnClearAllAuxParams;

    public static System.Action<List<Form3DGraph.NodeGraphData>> OnVisualizeGraph;
    public static System.Action <int> OnClickOnNode;
    
    
    public static System.Action<int, Vector2> OnChangeResultAuxParamsView;
    public static System.Action OnClearResultAuxParamsView;
    public static System.Action <List<Vector2>> OnAddResultAuxParamsDataList;




}
