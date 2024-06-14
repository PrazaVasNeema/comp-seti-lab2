using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab4
{

    [CreateAssetMenu(fileName = "Lab4DataSO", menuName = "Lab4/Lab4DataSO")]
    public class Lab4DataSO : ScriptableObject
    {

        [SerializeField] private Data m_data;
        public Data data => m_data;

        [System.Serializable]
        public class Data
        {
            public enum Independant
            {
                P_t,
                P_v
            }


            [Header("Gauss (распределение точек на плоскости)")]
            public float meanX;
            public float stdDevX;
            public float meanY;
            public float stdDevY;
            [Header("R[a, b] (Радиус)")]
            public float r_min;
            public float r_max;
            [Header("Вершины")]
            public int n_count;

            [Header("Характеристики p по умолчанию")]
            public float p_t;
            public float p_v;

            [Header("Независимая переменная")]
            public Independant independant;
            [Header("Исследуемый интервал")]
            public float start;
            public float end;
            public float step;

            [Header("Per Iter Data")]
            public float timePerIter;
            public float deltaTime;

            [Header("Количество итераций на одном наборе параметров")]
            public int iterAmount;

        }
    }

}