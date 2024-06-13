using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.U2D.Aseprite;

namespace lab3
{
    public class Imitations : MonoBehaviour
    {
        [SerializeField] private Lab3DataSO m_lab3DataSO;
        [SerializeField] private Visualizations m_visualizations;
        private Lab3DataSO.Data m_data => m_lab3DataSO.data;
        private Calculations m_calculations;
        private Analyzations m_analyzations = new Analyzations();

        private CancellationTokenSource cancelTokenSource;

        public async void Imitate()
        {
            m_visualizations.Clear();
            m_calculations = new Calculations(m_data.meanX, m_data.stdDevX, m_data.meanY, m_data.stdDevY, m_data.r_min, m_data.r_max);

            NDT.ViewData mathExpViewData = new NDT.ViewData(NDT.TargetView.MathExp);
            NDT.ViewData MainCompVertCountViewData = new NDT.ViewData(NDT.TargetView.MainCompVertComp);

            // curN usage
            //List<float> mathExpList = new List<float>();
            //List<int> MainCompVertCount = new List<int>();
            float[] mathExpArray = new float[m_data.iterAmount];
            float[] MainCompVertCountArray = new float[m_data.iterAmount];
            float mathExpSum;

            // Iter usage
            List<NDT.PointData> pointDataList = new List<NDT.PointData>();

            int step = (int)((m_data.n_end - m_data.n_start) / m_data.n_stepCount);

            GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Grey);


            List<NDT.ViewData> viewDatas = new List<NDT.ViewData>();
            viewDatas.Add(mathExpViewData);
            viewDatas.Add(MainCompVertCountViewData);

            UIController.m_progressBarFillRate = 0f;



            viewDatas = await Task.Run(() =>
            {

                for (int curN = m_data.n_start; curN <= m_data.n_end; curN += step)
                {
                    int[,] adjacencyMatrix = new int[curN, curN];
                    mathExpSum = 0f;

                    for (int iter = 0; iter < m_data.iterAmount; iter++)
                    {
                        pointDataList = m_calculations.GeneratePointsBelowParabola(curN);

                        m_calculations.CreateAdjacencyMatrix(pointDataList, ref adjacencyMatrix);

                        //m_calculations.WriteMatrixToFile(adjacencyMatrix, "dfgfdggfd.txt");

                        var test = m_analyzations.CalculateAverageOutDegree(ref adjacencyMatrix);
                        mathExpSum += test;
                    }

                    viewDatas[0].pointsList.Add(new NDT.ViewData.Points(curN, mathExpSum / (float)m_data.iterAmount));


                    UIController.m_progressBarFillRate = Mathf.InverseLerp(m_data.n_start, m_data.n_end, curN);
                }

                return viewDatas;
            });

            GameEvents.OnChangeAuxParamsView?.Invoke(UIController.curParamsValuesCount, m_data);

            GameEvents.OnBuildView?.Invoke(viewDatas[0]);

            GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Green);

            int[,] adjacencyMatrix = new int[m_data.n_end, m_data.n_end];
            pointDataList = m_calculations.GeneratePointsBelowParabola(m_data.n_end);
            m_calculations.CreateAdjacencyMatrix(pointDataList, ref adjacencyMatrix);

            m_visualizations.DrawParabola();
            m_visualizations.VisualizePoints(pointDataList);
        }

        private void OnDestroy()
        {
            cancelTokenSource.Cancel();
        }

    }

}