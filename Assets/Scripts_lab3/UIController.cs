using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab3
{

    public class UIController : MonoBehaviour
    {

        public enum UIStateAux
        {
            Green,
            Grey,
            Red
        }

        [SerializeField] private GameObject m_greenAux;
        [SerializeField] private GameObject m_greyAux;
        [SerializeField] private GameObject m_redAux;

        [SerializeField] private GameObject m_charts;


        public void OnEnable()
        {
            GameEvents.OnChangeUIStateAux += ChangeUIStateAux;

            GameEvents.OnClearViewData += RemoveAllViewData;
            GameEvents.OnClearAllAuxParams += OnClearAuxParamsView;
        }

        public void OnDisable()
        {
            GameEvents.OnChangeUIStateAux -= ChangeUIStateAux;

            GameEvents.OnClearViewData -= RemoveAllViewData;
            GameEvents.OnClearAllAuxParams -= OnClearAuxParamsView;
        }

        // Charts

        public void OnBuildAllDataViews(List<NDT.ViewData> viewDataList)
        {
            foreach(var viewData in viewDataList)
            {
                GameEvents.OnBuildView?.Invoke(viewData);
            }
        }

        public void RemoveAllViewData()
        {
            var charts = FindObjectsOfType<DataView>();

            foreach (var chart in charts)
            {
                chart.RemoveViewData();
            }
        }

        // Aux

        private void ChangeUIStateAux(UIStateAux uiStateAux)
        {
            m_greenAux.SetActive(false);
            m_greyAux.SetActive(false);
            m_redAux.SetActive(false);

            switch (uiStateAux)
            {
                case UIStateAux.Green:
                    m_greenAux.SetActive(true);
                    break;
                case UIStateAux.Grey:
                    m_greyAux.SetActive(true);
                    break;
                case UIStateAux.Red:
                    m_redAux.SetActive(true);
                    break;
            }
        }

        public void JustShowGrey()
        {
            m_greenAux.SetActive(false);
            m_greyAux.SetActive(true);
            m_redAux.SetActive(false);
        }

        // Changing current View

        [SerializeField] private GameObject m_ui3D;
        [SerializeField] private GameObject m_canvas;
        [SerializeField] private ViewOverseer m_ViewOverseer;

        public void Set3DView()
        {
            m_ui3D.SetActive(true);
            m_canvas.SetActive(false);
        }

        public void SetCanvasView()
        {
            m_ui3D.SetActive(false);
            m_canvas.SetActive(true);

        }


        public void SetCanvasView(int index)
        {
            m_canvas.SetActive(true);

            m_ViewOverseer.OnUpdateNodeDataCoreLogic(index);
        }

        // Params

        public static int curParamsValuesCount = 0;

        public void OnClearAuxParamsView()
        {
            for (int i = curParamsValuesCount; i > 0; i--)
            {
                GameEvents.OnClearAuxParamsView?.Invoke(i);
            }
            curParamsValuesCount = 0;
        }

    }

}