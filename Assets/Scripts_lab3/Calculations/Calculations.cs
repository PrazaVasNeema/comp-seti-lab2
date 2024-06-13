using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace lab3
{

    public class Calculations
    {
        public float meanX = 0f;
        public float stdDevX = 10f;
        public float meanY = 50f;
        public float stdDevY = 20f;

        public float minRadius = 0.5f; // Минимальное значение радиуса
        public float maxRadius = 2.0f; // Максимальное значение радиуса

        // ------

        public Calculations(float meanX, float stdDevX, float meanY, float stdDevY, float minRadius, float maxRadius)
        {
            this.meanX = meanX;
            this.stdDevX = stdDevX;
            this.meanY = meanY;
            this.stdDevY = stdDevY;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
        }

        public List<NDT.PointData> GeneratePointsBelowParabola(int numberOfPoints)
        {
            List<NDT.PointData> points = new List<NDT.PointData>();
            System.Random rand = new System.Random();

            while (points.Count < numberOfPoints)
            {
                float x = NextGaussian(rand, meanX, stdDevX);
                float y = NextGaussian(rand, meanY, stdDevY);
                float radius = (float)(rand.NextDouble() * (maxRadius - minRadius) + minRadius);

                if (y <= -Mathf.Pow(x, 2) + 100)
                {
                    points.Add(new NDT.PointData(new Vector2(x, y), radius));
                }
            }

            return points;
        }

        float NextGaussian(System.Random rand, float mean, float stdDev)
        {
            // ????????????? ????????????? ?????-??????? ??? ????????? ???????? ? ?????????? ??????????????
            double u1 = 1.0 - rand.NextDouble(); // Uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log((float)u1)) * Mathf.Sin(2.0f * Mathf.PI * (float)u2); // Random normal(0,1)
            return mean + stdDev * (float)randStdNormal; // Random normal(mean,stdDev^2)
        }

        public void CreateAdjacencyMatrix(List<NDT.PointData> points, ref int[,] adjacencyMatrix)
        {
            int n = points.Count;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i != j && Vector2.Distance(points[i].position, points[j].position) <= points[i].radius)
                    {
                        adjacencyMatrix[i, j] = 1;
                    }
                    else
                    {
                        adjacencyMatrix[i, j] = 0;
                    }
                }
            }

            //WriteMatrixToFile(adjacencyMatrix, "AdjacencyMatrix.txt");
        }

        public void WriteMatrixToFile(int[,] matrix, string fileName)
        {
            int n = matrix.GetLength(0);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        writer.Write(matrix[i, j]);
                        if (j < n - 1) writer.Write(" ");
                    }
                    writer.WriteLine();
                }
            }
        }

    }

}