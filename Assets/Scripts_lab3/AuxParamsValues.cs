using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace lab3
{

    public class AuxParamsValues : MonoBehaviour
    {
        [SerializeField] private int thisNum;

        [SerializeField] private GameObject paramsThemselvesGO;

        [SerializeField] private TMP_Text r_min;
        [SerializeField] private TMP_Text r_max;

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

        private void OnChangeAuxParamsView(int arg1, Lab3DataSO.Data arg2)
        {
            if (arg1 == thisNum)
            {
                paramsThemselvesGO.SetActive(true);

                r_min.text = arg2.r_min.ToString();
                r_max.text = arg2.r_max.ToString();
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

}