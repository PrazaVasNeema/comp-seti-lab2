using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using XCharts.Runtime;

public class DataView : MonoBehaviour
{
    // [SerializeField] private TMP_Text m_investigatedValueText;

    [SerializeField] private LineChart m_LineChart;

    private SerieData m_defaultSerieData;

    [SerializeField] private NDT.TargetView m_targetViewType;

    [SerializeField] private NDT.ViewData.ViewType m_ViewType;
    // [SerializeField] private TMP_Text m_dependencyValueText;


    
    // Start is called before the first frame update
    void Start()
    {
        // m_defaultSerieData = m_LineChart.series[0].data[0];
        // var seriesData = new XCharts.Runtime.SerieData();
        // seriesData = m_LineChart.series[0].data[0];
        // // seriesData.data
        // m_LineChart.series[0].data.Clear();
        // seriesData.data[0] = 10;
        // seriesData.data[1] = 10;
        // m_LineChart.series[0].data.Add(seriesData);
    }

    private void OnEnable()
    {
        GameEvents.OnBuildView += UpdateChart;
        GameEvents.OnClearViewData += RemoveAllViewData;
        GameEvents.OnClearAllAuxParams += OnClearAuxParamsView;
    }

    private void OnDisable()
    {
        GameEvents.OnBuildView -= UpdateChart;
        GameEvents.OnClearViewData -= RemoveAllViewData;
        GameEvents.OnClearAllAuxParams -= OnClearAuxParamsView;
    }

    private void UpdateChart(NDT.ViewData viewData)
    {
        if(!Equals(viewData.targetView, m_targetViewType))
            return;

        switch (m_ViewType)
        {
            case NDT.ViewData.ViewType.Chart:
                FillChart(viewData.pointsList);
                break;
            case NDT.ViewData.ViewType.Table:
                break;
        };
        

        // m_dependencyValueText.text = chartData.xAxisName.ToString();


    }

    private void FillChart(List<NDT.ViewData.Points> pointsList)
    {
        // m_LineChart.EnsureChartComponent<XAxis>().axisName.name = chartData.xAxisName.ToString();
        
        // m_LineChart.series[0].data.Clear();

        m_LineChart.AddSerie<Line>($"line {m_LineChart.series.Count}");

        int seriesCount = m_LineChart.series.Count;
        
        foreach (var point in pointsList)
        {
// Debug.Log(1);


            // var newSerieData = new SerieData();
            m_LineChart.AddData(seriesCount - 1, point.x, point.y);
            // m_defaultSerieData.data[0] = chartData.x;
            // m_defaultSerieData.data[1] = chartData.y;
            
            // m_LineChart.series[0].data.Add(m_defaultSerieData);
        }
    }

    public static int curParamsValuesCount = 0;
    
    public void RemoveViewData()
    {
        m_LineChart.RemoveData();
    }
    
    public void RemoveAllViewData()
    {
        // Debug.Log(1);

        var charts = FindObjectsOfType<DataView>();

        foreach (var chart in charts)
        {
            // Debug.Log(2);

            chart.RemoveViewData();
        }
    }

    public void OnClearAuxParamsView()
    {
        for (int i = curParamsValuesCount; i > 0; i--)
        {
            GameEvents.OnClearAuxParamsView?.Invoke(i);
        }
        curParamsValuesCount = 0;
    }
}
