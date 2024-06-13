using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab3
{

    public class GameEvents : MonoBehaviour
    {

        public static System.Action<UIController.UIStateAux> OnChangeUIStateAux;

        // Charts
        public static System.Action<NDT.ViewData> OnBuildView;
        public static System.Action OnClearViewData;

        // Aux
        public static System.Action OnClearAllAuxParams;
        public static System.Action<int> OnClearAuxParamsView;
        public static System.Action<int, Lab3DataSO.Data> OnChangeAuxParamsView;

        // 
    }

}
