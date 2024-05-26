using System;
using System.Collections.Generic;
using test2;
using UnityEngine;

public class HexagonGridGenerator : MonoBehaviour
{
    public int numHexagons = 3; // Number of hexagons in the row

    private int verticesPerHexagon = 6;
    private int sharedVertices = 2;

    // void Start()
    // {
    //     var (adjMatrix, positions) = GenerateHexagonRowAdjacencyMatrix(numHexagons);
    //     PrintAdjacencyMatrix(adjMatrix);
    //     PrintPositions(positions);
    //
    //     test2.HexGrid.SaveAdjacencyMatrixToFile(adjMatrix, Application.dataPath + "/TESTTESTTESTadjacency_matrix.txt");
    //
    //     var NodeGraphFormDataList = new List<Form3DGraph.NodeGraphData>();
    //     
    //     for (int i = 0; i < positions.Count; i++)
    //     {
    //         var node = new Form3DGraph.NodeGraphData();
    //         node.Coords = positions[i];
    //         node.Neighbours = new List<int>();
    //         for (int j = 0; j < positions.Count; j++)
    //         {
    //             if (adjMatrix[i, j] == 1)
    //             {
    //                 node.Neighbours.Add(j);
    //             }
    //         }
    //         NodeGraphFormDataList.Add(node);
    //     }
    //     
    //     GameEvents.OnVisualizeGraph?.Invoke(NodeGraphFormDataList);
    // }

    public int[,] CREATE(int numHexagons, out int COUNT)
    {
        var (adjMatrix, positions) = GenerateHexagonRowAdjacencyMatrix(numHexagons);
        
        // PrintAdjacencyMatrix(adjMatrix);
        // PrintPositions(positions);

        test2.HexGrid.SaveAdjacencyMatrixToFile(adjMatrix, Application.dataPath + "/TESTTESTTESTadjacency_matrix.txt");

        var NodeGraphFormDataList = new List<Form3DGraph.NodeGraphData>();
        
        for (int i = 0; i < positions.Count; i++)
        {
            var node = new Form3DGraph.NodeGraphData();
            node.Coords = positions[i];
            node.Neighbours = new List<int>();
            for (int j = 0; j < positions.Count; j++)
            {
                if (adjMatrix[i, j] == 1)
                {
                    node.Neighbours.Add(j);
                }
            }
            NodeGraphFormDataList.Add(node);
        }
        
        GameEvents.OnVisualizeGraph?.Invoke(NodeGraphFormDataList);


        COUNT = positions.Count;
        
        return adjMatrix;
    }


    private (int[,], List<Vector2>) GenerateHexagonRowAdjacencyMatrix(int numVertices)
    {
        int NumFullHexagons = (numVertices >= verticesPerHexagon) ? 1 : 0;
        int remainingVertices = numVertices - verticesPerHexagon;
        
        while (remainingVertices >= verticesPerHexagon - sharedVertices)
        {
            NumFullHexagons++;
            remainingVertices -= (verticesPerHexagon - sharedVertices);
        }

        int totalVertices = NumFullHexagons * (verticesPerHexagon - sharedVertices) + sharedVertices + remainingVertices;
        
        Debug.Log($"NumFullHexagons: {NumFullHexagons}, remainingVertices: {remainingVertices}, totalVertices: {totalVertices}");

        int[,] adjMatrix = new int[totalVertices, totalVertices];

        List<Vector2> positions = new List<Vector2>();
        Dictionary<Vector2, int> vertexIndex = new Dictionary<Vector2, int>();
        int currentVertex = 0;

        bool brokenCase = false;
        
        if (remainingVertices > 0)
        {
            NumFullHexagons++;
            brokenCase = true;
        }

        bool isGood = true;

        for (int hexagon = 0; hexagon < NumFullHexagons; hexagon++)
        {
            int verticesInHexagon = verticesPerHexagon;
            
            int[] v;
            if (hexagon == 0)
            {
                v = new int[verticesInHexagon];
                for (int i = 0; i < verticesInHexagon; i++) v[i] = i;
            }
            else
            {
                v = new int[verticesInHexagon];
                for (int i = 0; i < verticesInHexagon; i++) v[i] = currentVertex - 2 + i;
            }

            float base_x = hexagon * 1.5f;
            float base_y = (hexagon % 2 == 0) ? 0 : Mathf.Sqrt(3) / 2;

            Vector2[] pos = new Vector2[]
            {
                new Vector2(base_x, base_y),
                new Vector2(base_x + 0.5f, base_y + Mathf.Sqrt(3) / 2),
                new Vector2(base_x + 1.5f, base_y + Mathf.Sqrt(3) / 2),
                new Vector2(base_x + 2.0f, base_y),
                new Vector2(base_x + 1.5f, base_y - Mathf.Sqrt(3) / 2),
                new Vector2(base_x + 0.5f, base_y - Mathf.Sqrt(3) / 2)
            };

            for (int i = 0; i < verticesInHexagon; i++)
            {
                if (!vertexIndex.ContainsKey(pos[i]) && isGood)
                {
                    vertexIndex[pos[i]] = currentVertex;
                    positions.Add(pos[i]);
                    currentVertex++;

                    if (brokenCase && NumFullHexagons - 1 == hexagon)
                    {
                        remainingVertices--;
                        if (remainingVertices == 0)
                            isGood = false;
                    }
                }
            }

            for (int i = 0; i < verticesInHexagon; i++)
            {
                if (!vertexIndex.ContainsKey(pos[(i + 1) % verticesInHexagon]))
                    break;
                int v1 = vertexIndex[pos[i]];
                int v2 = vertexIndex[pos[(i + 1) % verticesInHexagon]];
                adjMatrix[v1, v2] = 1;
                adjMatrix[v2, v1] = 1;
            }
        }

        return (adjMatrix, positions);
    }

    private void PrintAdjacencyMatrix(int[,] adjMatrix)
    {
        int rows = adjMatrix.GetLength(0);
        int cols = adjMatrix.GetLength(1);
        Debug.Log("Adjacency Matrix:");
        for (int i = 0; i < rows; i++)
        {
            string row = "";
            for (int j = 0; j < cols; j++)
            {
                row += adjMatrix[i, j] + " ";
            }
            Debug.Log(row);
        }
    }

    private void PrintPositions(List<Vector2> positions)
    {
        Debug.Log("Positions:");
        foreach (var pos in positions)
        {
            Debug.Log(pos.x + " " + pos.y);
        }
    }

    
}
