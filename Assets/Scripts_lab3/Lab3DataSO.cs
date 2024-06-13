using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab3
{

    [CreateAssetMenu(fileName = "Lab1DataSO", menuName = "Lab1/Lab1DataSO")]
    public class Lab3DataSO : ScriptableObject
    {

        [SerializeField] private Data m_data;
        public Data data => m_data;

        [System.Serializable]
        public class Data
        {

            [Header("Gauss (распределение точек на плоскости)")]
            public float mean;
            public float std_dev;
            [Header("R[a, b] (Радиус)")]
            public float r_min;
            public float r_max;
            [Header("Вершины")]
            public float n_start;
            public float n_end;
            [Header("Количество шагов (зависит от отрезка значений вершин)")]
            public int n_stepCount;

            [Header("Количество итераций на одном наборе параметров")]
            public int iterAmount;

        }
    }

}