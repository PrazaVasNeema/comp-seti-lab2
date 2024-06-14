using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.U2D.Aseprite;
using lab3;

namespace lab4
{
    public class Imitations : MonoBehaviour
    {
        [SerializeField] private Lab4DataSO m_lab4DataSO;
        [SerializeField] private Visualizations m_visualizations;
        private Lab4DataSO.Data m_data => m_lab4DataSO.data;
        private Calculations m_calculations;
        private Analyzations m_analyzations = new Analyzations();
        private ProcessDensityX m_processDensityX = new ProcessDensityX();
        private ProcessDensityY m_processDensityY = new ProcessDensityY();

        private CancellationTokenSource cancelTokenSource;

        public void Start()
        {
            cancelTokenSource = new CancellationTokenSource();
        }

        public async void Imitate()
        {
            PointData.SetStaticPParams(m_data.p_t, m_data.p_v);

            m_visualizations.Clear();
            m_calculations = new Calculations(m_data.meanX, m_data.stdDevX, m_data.meanY, m_data.stdDevY, m_data.r_min, m_data.r_max);

            NDT.ViewData mathExpViewData = new NDT.ViewData(NDT.TargetView.MathExp);
            NDT.ViewData MainCompVertCountViewData = new NDT.ViewData(NDT.TargetView.MainCompVertComp);

            // curN usage
            //List<float> mathExpList = new List<float>();
            //List<int> MainCompVertCount = new List<int>();
            //float[] mathExpArray = new float[m_data.iterAmount];
            //float[] MainCompVertCountArray = new float[m_data.iterAmount];
            float mathExpSum;
            float mainCompSum;

            // Iter usage
            Dictionary<float,List<PointData>> pointDataInTimeDict = new Dictionary<float, List<PointData>>();

            //int step = (int)((m_data.n_end - m_data.n_start) / m_data.n_stepCount) > 0 ? (int)((m_data.n_end - m_data.n_start) / m_data.n_stepCount) : 1;
            //Debug.Log("Step:" + step);
            int[,] adjacencyMatrix = new int[m_data.n_count, m_data.n_count];
            //var test = m_calculations.GeneratePointsBelowParabola(m_data.n_count);

            //m_visualizations.DrawParabola();
            //m_visualizations.VisualizePoints(test);



            //return;

            GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Grey);





            List<NDT.ViewData> viewDatas = new List<NDT.ViewData>();
            viewDatas.Add(mathExpViewData);
            viewDatas.Add(MainCompVertCountViewData);

            UIController.m_progressBarFillRate = 0f;

            float saveIndevendantValue = GetIndependantValue();

            SetIndependantValue(m_data.start);


            m_processDensityX.ClearData();
            m_processDensityY.ClearData();

            viewDatas = await Task.Run(() =>
            {

                for (float expIndValue = m_data.start; expIndValue <= m_data.end; expIndValue += m_data.step)
                {
                    mathExpSum = 0f;
                    mainCompSum = 0f;

                    SetPointDataIndependantValue(expIndValue);

                    for (int iter = 0; iter < m_data.iterAmount; iter++)
                    {

                        float imitationTime = 0f;
                        float prevImitationTime = 0f;

                        pointDataInTimeDict.Clear();

                        pointDataInTimeDict[imitationTime] = m_calculations.GeneratePointsBelowParabola(m_data.n_count);
                        m_calculations.CreateAdjacencyMatrix(pointDataInTimeDict[imitationTime], ref adjacencyMatrix);
                        prevImitationTime = imitationTime;
                        imitationTime += m_data.deltaTime;

                        while (imitationTime <= m_data.timePerIter)
                        {
                            pointDataInTimeDict[imitationTime] = new List<PointData>();

                            foreach (PointData prevPointData in pointDataInTimeDict[prevImitationTime])
                            {
                                pointDataInTimeDict[imitationTime].Add(PointData.GetNewPointData(imitationTime, prevPointData));
                            }

                            imitationTime += m_data.deltaTime;
                        }

                        float iterScaleMathExpSum = 0f;
                        float iterScalemainCompSum = 0f;

                        foreach (List<PointData> pointDataList in pointDataInTimeDict.Values)
                        {
                            m_calculations.CreateAdjacencyMatrix(pointDataList, ref adjacencyMatrix);

                            iterScaleMathExpSum += m_analyzations.CalculateAverageOutDegree(ref adjacencyMatrix);
                            iterScalemainCompSum += m_analyzations.CalculateMainComponentSize(ref adjacencyMatrix);
                        }
                        mathExpSum += iterScaleMathExpSum / pointDataInTimeDict.Count;
                        mainCompSum += iterScalemainCompSum / pointDataInTimeDict.Count;
                        //Debug.Log("dict count:" + pointDataInTimeDict.Count);

                        if (expIndValue == m_data.start)
                        {
                            m_processDensityX.CloneDictAndAdd(pointDataInTimeDict);
                            m_processDensityY.CloneDictAndAdd(pointDataInTimeDict);
                        }

                    }


                    viewDatas[0].pointsList.Add(new NDT.ViewData.Points(expIndValue, mathExpSum / (float)m_data.iterAmount));
                    viewDatas[1].pointsList.Add(new NDT.ViewData.Points(expIndValue, mainCompSum / (float)m_data.iterAmount));

                    if (expIndValue == m_data.start)
                    {
                        viewDatas.Add(m_processDensityX.GetViewData());
                        viewDatas.Add(m_processDensityY.GetViewData());
                    }


                    UIController.m_progressBarFillRate = Mathf.InverseLerp(m_data.start, m_data.end, expIndValue);
                    //Debug.Log("curN: " + curN);
                }

                return viewDatas;
            });

            SetIndependantValue(saveIndevendantValue);

            GameEvents.OnChangeAuxParamsView?.Invoke(UIController.curParamsValuesCount, m_data);

            GameEvents.OnBuildView?.Invoke(viewDatas[0]);
            GameEvents.OnBuildView?.Invoke(viewDatas[1]);
            GameEvents.OnBuildView?.Invoke(viewDatas[2]);
            GameEvents.OnBuildView?.Invoke(viewDatas[3]);



            GameEvents.OnChangeUIStateAux?.Invoke(UIController.UIStateAux.Green);

            //int[,] adjacencyMatrix = new int[m_data.n_end, m_data.n_end];
            //pointDataList = m_calculations.GeneratePointsBelowParabola(m_data.n_end);
            //m_calculations.CreateAdjacencyMatrix(pointDataList, ref adjacencyMatrix);

            //m_visualizations.DrawParabola();
            //m_visualizations.VisualizePoints(pointDataList);
            //m_visualizations.VisualizeEdges(pointDataList, ref adjacencyMatrix);
        }

        private void OnDestroy()
        {
            cancelTokenSource.Cancel();
        }

        private float GetIndependantValue()
        {
            float returnValue = -1f;
            switch (m_data.independant)
            {
                case Lab4DataSO.Data.Independant.P_t:
                    {
                        returnValue = m_data.p_t;
                        break;
                    }
                case Lab4DataSO.Data.Independant.P_v:
                    {
                        returnValue = m_data.p_v;
                        break;
                    }
            }

            return returnValue;
        }

        private void SetIndependantValue(float newValue)
        {
            switch (m_data.independant)
            {
                case Lab4DataSO.Data.Independant.P_t:
                    {
                        m_data.p_t = newValue;
                        break;
                    }
                case Lab4DataSO.Data.Independant.P_v:
                    {
                        m_data.p_v = newValue;
                        break;
                    }
            }
        }

        private void SetPointDataIndependantValue(float newValue)
        {
            switch (m_data.independant)
            {
                case Lab4DataSO.Data.Independant.P_t:
                    {
                        PointData.SetStaticPParams(newValue, m_data.p_v);
                        break;
                    }
                case Lab4DataSO.Data.Independant.P_v:
                    {
                        PointData.SetStaticPParams(m_data.p_t, newValue);
                        break;
                    }
            }
        }


        // RANDOMS

        public static int randomStuffIter;
        public static List<float> randomStuffList;

        public static void SetRandomStuff()
        {
            randomStuffList = new List<float>();
            randomStuffIter = 0;

            for (int i = 0; i < 1000; i++)
            {

            }
        }

        //public static float GetRandomStuff()
        //{

        //}

    }

}