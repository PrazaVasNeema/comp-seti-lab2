using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static System.Action<NDT.ViewData> OnBuildView;
    public static System.Action<UIController.UIStateAux> OnChangeUIStateAux;
}
