using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace test2
{
    public class Server
    {
        public int Id { get; private set; }
        public List<Server> Neighbors { get; private set; }

        public Server(int id)
        {
            Id = id;
            Neighbors = new List<Server>();
        }

        public void AddNeighbor(Server neighbor)
        {
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
            }
        }
    }
    
    public class HexGrid
{
    public static int[,] CreateAdjacencyMatrix(int rows, int cols)
    {
        int totalServers = rows * cols;
        int[,] matrix = new int[totalServers, totalServers];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int current = row * cols + col;

                // Add the right neighbor
                if (col < cols - 1)
                {
                    int right = current + 1;
                    matrix[current, right] = 1;
                    matrix[right, current] = 1;
                }

                // Add the bottom-right neighbor
                if (row < rows - 1 && col < cols - 1)
                {
                    int bottomRight = current + cols + 1;
                    matrix[current, bottomRight] = 1;
                    matrix[bottomRight, current] = 1;
                }

                // Add the bottom-left neighbor
                if (row < rows - 1 && col > 0)
                {
                    int bottomLeft = current + cols - 1;
                    matrix[current, bottomLeft] = 1;
                    matrix[bottomLeft, current] = 1;
                }

                // Add the bottom neighbor
                if (row < rows - 1)
                {
                    int bottom = current + cols;
                    matrix[current, bottom] = 1;
                    matrix[bottom, current] = 1;
                }
            }
        }

        return matrix;
    }

    public static List<Server> InitializeServers(int rows, int cols, int[,] adjacencyMatrix)
    {
        int totalServers = rows * cols;
        List<Server> servers = new List<Server>();

        for (int i = 0; i < totalServers; i++)
        {
            servers.Add(new Server(i));
        }

        for (int i = 0; i < totalServers; i++)
        {
            for (int j = 0; j < totalServers; j++)
            {
                if (adjacencyMatrix[i, j] == 1)
                {
                    servers[i].AddNeighbor(servers[j]);
                }
            }
        }

        return servers;
    }

    public static void SaveAdjacencyMatrixToFile(int[,] matrix, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            int size = matrix.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    writer.Write(matrix[i, j] + " ");
                }
                writer.WriteLine();
            }
        }
    }
}
    
    public class HexGridManager : MonoBehaviour
    {
        void Start()
        {
            int rows = 1;  // Задайте количество рядов
            int cols = 1;  // Задайте количество колонок

            int[,] adjacencyMatrix = HexGrid.CreateAdjacencyMatrix(rows, cols);
        
            List<Server> servers = HexGrid.InitializeServers(rows, cols, adjacencyMatrix);
        
            string filePath = Application.dataPath + "/adjacency_matrix.txt";
            HexGrid.SaveAdjacencyMatrixToFile(adjacencyMatrix, filePath);

            Debug.Log("Матрица смежности сохранена в " + filePath);
        }
    }
    
}