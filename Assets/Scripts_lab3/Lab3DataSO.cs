using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab3
{

    [CreateAssetMenu(fileName = "Lab3DataSO", menuName = "Lab3/Lab3DataSO")]
    public class Lab3DataSO : ScriptableObject
    {

        [SerializeField] private Data m_data;
        public Data data => m_data;

        [System.Serializable]
        public class Data
        {

            [Header("Gauss (распределение точек на плоскости)")]
            public float meanX;
            public float stdDevX;
            public float meanY;
            public float stdDevY;
            [Header("R[a, b] (Радиус)")]
            public float r_min;
            public float r_max;
            [Header("Вершины")]
            public int n_start;
            public int n_end;
            [Header("Количество шагов (зависит от отрезка значений вершин)")]
            public int n_stepCount;

            [Header("Количество итераций на одном наборе параметров")]
            public int iterAmount;

        }
    }

}