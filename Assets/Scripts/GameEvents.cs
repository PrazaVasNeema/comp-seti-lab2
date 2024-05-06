using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static System.Action<NDT.ViewData> OnBuildView;
    public static System.Action<UIController.UIStateAux> OnChangeUIStateAux;
    
    public static System.Action<int, Lab1DataSO.Data> OnChangeAuxParamsView;
    public static System.Action<int> OnClearAuxParamsView;


}
