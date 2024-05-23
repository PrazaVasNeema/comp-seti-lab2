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

}
