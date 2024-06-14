using System.Collections;
using UnityEditor;

using System.Collections.Generic;
using UnityEngine;
using static NDT.ServerLog;
using static ProcessDataProbability;
using XCharts.Runtime;
using System;

namespace lab4
{

    public class ProcessDensityY
    {
        public Dictionary<int, int> densityDict;
        public float minValue;

        public float maxValue;
        public void ClearData()
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;

            densityDict = new Dictionary<int, int>();
            for (int i = 0; i < 100; i++)
            {
                densityDict.Add(i, 0);
            }

            researchDict.Clear();
        }

        public void ValidateMinValue(float currentValue)
        {
            minValue = currentValue < minValue ? currentValue : minValue;
        }

        public void ValidateMaxValue(float currentValue)
        {
            maxValue = currentValue > maxValue ? currentValue : maxValue;
        }

        List<Dictionary<float, List<PointData>>> researchDict = new List<Dictionary<float, List<PointData>>>();

        public void CloneDictAndAdd(Dictionary<float, List<PointData>> curDict)
        {
            var newDict = new Dictionary<float, List<PointData>>();

            foreach (var kvp in curDict)
            {
                newDict.Add(kvp.Key, kvp.Value);
            }

            researchDict.Add(newDict);
        }

        public NDT.ViewData GetViewData()
        {
            int valueCounter = -1;

            foreach (var dict in researchDict)
            {
                foreach (var kvp in dict)
                {
                    foreach (var pointData in kvp.Value)
                    {
                        ValidateMinValue(pointData.position.y);
                        ValidateMaxValue(pointData.position.y);
                    }
                }

            }

            Debug.Log("max value: " + maxValue);


            foreach (var dict in researchDict)
            {
                foreach (var kvp in dict)
                {
                    foreach (var pointData in kvp.Value)
                    {

                        float value = pointData.position.y;
                        var inverse = Mathf.InverseLerp(minValue, maxValue, value);
                        densityDict[(int)(Mathf.Lerp(0, 99.9f, inverse))]++;
                        valueCounter++;
                    }
                }

            }




            //for (int i = 0; i < 100; i++)
            //{
            //    densityDict[i] /= researchDict.Count;
            //}

            var viewData = new NDT.ViewData(NDT.TargetView.densityY);

            var finalDict = new Dictionary<float, float>();

            for (int i = 0; i < 100; i++)
            {
                finalDict.Add(Mathf.Lerp(minValue, maxValue, (float)i / 99), densityDict[i]);
            }

            foreach (var density in finalDict)
            {
                viewData.pointsList.Add(new NDT.ViewData.Points(density.Key, density.Value));

            }

            return viewData;
        }
    }

}