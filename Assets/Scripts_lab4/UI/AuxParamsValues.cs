using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace lab4
{

    public class AuxParamsValues : MonoBehaviour
    {
        [SerializeField] private int thisNum;

        [SerializeField] private GameObject paramsThemselvesGO;

        [SerializeField] private TMP_Text n_count;
        [SerializeField] private TMP_Text r_min;
        [SerializeField] private TMP_Text r_max;
        [SerializeField] private TMP_Text p_t;
        [SerializeField] private TMP_Text p_v;


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

        private void OnChangeAuxParamsView(int arg1, Lab4DataSO.Data arg2)
        {
            //Debug.Log(arg1);
            if (arg1 == thisNum)
            {
                paramsThemselvesGO.SetActive(true);

                n_count.text = arg2.n_count.ToString();
                r_min.text = arg2.r_min.ToString();
                r_max.text = arg2.r_max.ToString();
                p_t.text = arg2.p_t.ToString();
                p_v.text = arg2.p_v.ToString();

                UIController.curParamsValuesCount++;

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