using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using XCharts.Runtime;

namespace lab3
{

    public class DataView : MonoBehaviour
    {

        [SerializeField] private LineChart m_LineChart;
        [SerializeField] private NDT.TargetView m_targetViewType;


        private void OnEnable()
        {
            GameEvents.OnBuildView += UpdateChart;
        }

        private void OnDisable()
        {
            GameEvents.OnBuildView -= UpdateChart;
        }

        private void UpdateChart(NDT.ViewData viewData)
        {
            if (!Equals(viewData.targetView, m_targetViewType))
                return;

             FillChart(viewData.pointsList);
        }

        private void FillChart(List<NDT.ViewData.Points> pointsList)
        {
            m_LineChart.AddSerie<Line>($"line {m_LineChart.series.Count}");
            int seriesCount = m_LineChart.series.Count;
            foreach (var point in pointsList)
            {
                m_LineChart.AddData(seriesCount - 1, point.x, point.y);
            }
        }

        public void RemoveViewData()
        {
            m_LineChart.RemoveData();
        }

    }

}