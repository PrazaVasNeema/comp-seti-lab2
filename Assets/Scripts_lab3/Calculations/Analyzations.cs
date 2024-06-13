using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab3
{

    public class Analyzations
    {
        public float CalculateAverageOutDegree(ref int[,] matrix)
        {
            int n = matrix.GetLength(0);
            int totalOutDegree = 0;

            for (int i = 0; i < n; i++)
            {
                int outDegree = 0;
                for (int j = 0; j < n; j++)
                {
                    outDegree += matrix[i, j];
                }
                totalOutDegree += outDegree;
            }

            return (float)totalOutDegree / n;
        }
    }

}