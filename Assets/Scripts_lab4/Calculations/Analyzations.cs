using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab4
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

        public int CalculateMainComponentSize(ref int[,] matrix)
        {
            int n = matrix.GetLength(0);
            bool[] visited = new bool[n];
            int maxSize = 0;

            for (int i = 0; i < n; i++)
            {
                if (!visited[i])
                {
                    int size = DFS(matrix, i, visited);
                    if (size > maxSize)
                    {
                        maxSize = size;
                    }
                }
            }

            return maxSize;
        }

        int DFS(int[,] matrix, int node, bool[] visited)
        {
            Stack<int> stack = new Stack<int>();
            stack.Push(node);
            int size = 0;

            while (stack.Count > 0)
            {
                int current = stack.Pop();
                if (!visited[current])
                {
                    visited[current] = true;
                    size++;
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        if (matrix[current, i] == 1 && !visited[i])
                        {
                            stack.Push(i);
                        }
                    }
                }
            }

            return size;
        }
    }

}