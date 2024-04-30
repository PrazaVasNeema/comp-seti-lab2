using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XCharts.Runtime;

public class ChartView : MonoBehaviour
{
    // [SerializeField] private TMP_Text m_investigatedValueText;

    [SerializeField] private LineChart m_LineChart;

    private SerieData m_defaultSerieData;

    [SerializeField] private InvestigatePieceAbstract.TargetChart m_chartType;
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
    }

    private void OnDisable()
    {
        GameEvents.OnBuildView -= UpdateChart;
    }

    private void UpdateChart(InvestigatePieceAbstract.ChartData chartData)
    {
        if(!Equals(chartData.targetChart, m_chartType))
            return;
        
        // m_dependencyValueText.text = chartData.xAxisName.ToString();
        
        m_LineChart.EnsureChartComponent<XAxis>().axisName.name = chartData.xAxisName.ToString();
        
        m_LineChart.series[0].data.Clear();

        foreach (var point in chartData.pointsList)
        {
// Debug.Log(1);

            // var newSerieData = new SerieData();
            m_LineChart.AddData(0, point.x, point.y);
            // m_defaultSerieData.data[0] = chartData.x;
            // m_defaultSerieData.data[1] = chartData.y;
            
            // m_LineChart.series[0].data.Add(m_defaultSerieData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
