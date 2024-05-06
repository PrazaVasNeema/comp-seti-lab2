using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lab1DataSO", menuName = "Lab1/Lab1DataSO")]
public class Lab1DataSO : ScriptableObject
{
    public enum InvestigatedValue
    {
        Ux,
        P_prosti
    }
    public enum DependencyValue
    {
        lambda,
        D,
        mean,
        std_dev,
        justTime
    }
    [SerializeField] private Data m_data;
    public Data data => m_data;
    
    [System.Serializable]
    public class Data
    {
        [Header("Параметры распределения")] 
        [Header("A(x): Erland5 + D")] 
        public float lambda;
        public float D;
        [Header("B(x): |Gauss|")]
        public float mean;
        public float std_dev;
        [Header("Построение графиков")]
        [Header("Количество итераций на одном наборе параметров")]
        public int iterAmount;
        // [Header("Изменять параметры")]
        // public float from;
        // public float to;
        // public float step;
        [Header("Количество заданий для остановки")]
        public int k;
        // [Header("Исследуемое значение")]
        // public InvestigatedValue investigatedValue;
        // [Header("График зависит от параметра:")]
        // public DependencyValue dependencyValue;


    }
}
