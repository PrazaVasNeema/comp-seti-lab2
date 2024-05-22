using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace test
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
            Neighbors.Add(neighbor);
        }
    }
    
    public class HexGridGraph : MonoBehaviour
    {
        public int gridWidth = 5;  // Ширина сетки
        public int gridHeight = 5; // Высота сетки
        private Server[,] servers;
    
        void Start()
        {
            InitializeGrid();
            int[,] adjacencyMatrix = CreateAdjacencyMatrix();
            SaveAdjacencyMatrixToFile(adjacencyMatrix, "adjacency_matrix.txt");
        }
    
        void InitializeGrid()
        {
            servers = new Server[gridWidth, gridHeight];
            int id = 0;
    
            // Создание серверов
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    servers[x, y] = new Server(id++);
                }
            }
    
            // Установка соседей
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Server current = servers[x, y];
                    foreach (var neighbor in GetNeighbors(x, y))
                    {
                        current.AddNeighbor(neighbor);
                    }
                }
            }
        }
    
        List<Server> GetNeighbors(int x, int y)
        {
            List<Server> neighbors = new List<Server>();
    
            // Правильные шестиугольные соты имеют шесть возможных направлений
            int[][] directions;
            if (y % 2 == 0)
            {
                directions = new int[][]
                {
                    new int[] { 1, 0 }, new int[] { -1, 0 },
                    new int[] { 0, 1 }, new int[] { 0, -1 },
                    new int[] { 1, -1 }, new int[] { 1, 1 }
                };
            }
            else
            {
                directions = new int[][]
                {
                    new int[] { 1, 0 }, new int[] { -1, 0 },
                    new int[] { 0, 1 }, new int[] { 0, -1 },
                    new int[] { -1, -1 }, new int[] { -1, 1 }
                };
            }
    
            foreach (var dir in directions)
            {
                int nx = x + dir[0];
                int ny = y + dir[1];
    
                if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                {
                    neighbors.Add(servers[nx, ny]);
                }
            }
    
            return neighbors;
        }
    
        int[,] CreateAdjacencyMatrix()
        {
            int k = gridWidth * gridHeight;
            int[,] matrix = new int[k, k];
    
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Server current = servers[x, y];
                    foreach (var neighbor in current.Neighbors)
                    {
                        matrix[current.Id, neighbor.Id] = 1;
                    }
                }
            }
    
            return matrix;
        }
    
        void SaveAdjacencyMatrixToFile(int[,] matrix, string filename)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
    
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        writer.Write(matrix[i, j]);
                        if (j < cols - 1)
                        {
                            writer.Write(",");
                        }
                    }
                    writer.WriteLine();
                }
            }
    
            Debug.Log($"Матрица смежностей сохранена в файл {filename}");
        }
    }
    
}