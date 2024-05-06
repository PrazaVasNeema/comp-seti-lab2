using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuxParamsValues : MonoBehaviour
{
    [SerializeField] private int thisNum;
    
    [SerializeField] private GameObject paramsThemselvesGO;
    
    [SerializeField] private TMP_Text lambda;
    [SerializeField] private TMP_Text D;
    [SerializeField] private TMP_Text mean;
    [SerializeField] private TMP_Text std_dev;


    private void OnEnable()
    {
        GameEvents.OnChangeAuxParamsView += OnChangeAuxParamsView;
        GameEvents.OnClearAuxParamsView += OnClearAuxParamsView;
    }



    private void OnDisable()
    {
        GameEvents.OnChangeAuxParamsView -= OnChangeAuxParamsView;
        GameEvents.OnClearAuxParamsView -= OnClearAuxParamsView;
    }
    
    private void OnChangeAuxParamsView(int arg1, Lab1DataSO.Data arg2)
    {
        if (arg1 == thisNum)
        {
            paramsThemselvesGO.SetActive(true);

            lambda.text = arg2.lambda.ToString();
            D.text = arg2.D.ToString();
            mean.text = arg2.mean.ToString();
            std_dev.text = arg2.std_dev.ToString();
        }
    }
    
    private void OnClearAuxParamsView(int obj)
    {
        if (obj == thisNum)
        {
            paramsThemselvesGO.SetActive(false);
        }
    }


}
