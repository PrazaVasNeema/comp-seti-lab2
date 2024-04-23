using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XCharts.Runtime;

public class ChartView : MonoBehaviour
{
    [SerializeField] private TMP_Text m_investigatedValueText;
    [SerializeField] private TMP_Text m_dependencyValueText;

    [SerializeField] private LineChart m_LineChart;

    private SerieData m_defaultSerieData;

    
    // Start is called before the first frame update
    void Start()
    {
        m_defaultSerieData = m_LineChart.series[0].data[0];
        // var seriesData = new XCharts.Runtime.SerieData();
        // seriesData = m_LineChart.series[0].data[0];
        // // seriesData.data
        // m_LineChart.series[0].data.Clear();
        // seriesData.data[0] = 10;
        // seriesData.data[1] = 10;
        // m_LineChart.series[0].data.Add(seriesData);
    }

    public void UpdateChart(List<Lab1.ChartData> chartDataList)
    {
        m_LineChart.series[0].data.Clear();

        foreach (var chartData in chartDataList)
        {
// Debug.Log(1);

            // var newSerieData = new SerieData();
            m_LineChart.AddData(0, chartData.x, chartData.y);
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
