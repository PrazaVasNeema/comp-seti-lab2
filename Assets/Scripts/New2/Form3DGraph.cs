using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Form3DGraph : MonoBehaviour
{
    
    public class NodeGraphData
    {
        public Vector2 Coords;
        public List<int> Neighbours;
    }
    



    
    [SerializeField] private Node3D pointPrefab;
    [SerializeField] private float multiplier = 5f;

    [SerializeField] private GameObject nodesParent;
    

    private void OnEnable()
    {
        Debug.Log("TEST");
        GameEvents.OnVisualizeGraph += VisualizeGraph;
    }

    private void OnDisable()
    {
        GameEvents.OnVisualizeGraph -= VisualizeGraph;
    }

    private void Clear()
    {
        foreach (var child in nodesParent.transform.GetComponentsInChildren<Transform>())
        {
            if (child == nodesParent.transform)
                continue;
            Destroy(child.gameObject);
        }
    }

    private void VisualizeGraph(List<NodeGraphData> nodeDataList)
    {
        Clear();
        List<Node3D> nodes = new List<Node3D>();
        
        for (int i = 0; i < nodeDataList.Count; i ++)
        {
            var node = Instantiate(pointPrefab, new Vector3(nodeDataList[i].Coords.x * multiplier, nodeDataList[i].Coords.y * multiplier, 0), Quaternion.identity);
            
            node.transform.SetParent(nodesParent.transform);
            node.LoadData(i);
            nodes.Add(node);
            
        }

        
        for (int i = 0; i < nodeDataList.Count; i++)
        {
            var neighbours = new List<Transform>();
            foreach (var neighbourID in nodeDataList[i].Neighbours)
            {
                neighbours.Add(nodes[neighbourID].transform);
            }
            
            nodes[i].neighbours = neighbours;
            
            nodes[i].SetupLineRenderers();
        }
    }


    // void Start()
    // {
    //     points = ReadPointsFromFile("vertex_positions");
    //     foreach (var point in points)
    //     {
    //         Debug.Log(point);
    //     }
    //     
    //     FormGraph();
    // }

    // List<NDT.ViewData.Points> ReadPointsFromFile(string fileName)
    // {
    //     List<Point> points = new List<Point>();
    //
    //     TextAsset file = Resources.Load<TextAsset>(fileName);
    //     if (file != null)
    //     {
    //         StringReader reader = new StringReader(file.text);
    //         string line;
    //         while ((line = reader.ReadLine()) != null)
    //         {
    //             string[] coords = line.Split(' ');
    //             if (coords.Length == 2)
    //             {
    //                 if (float.TryParse(coords[0], out float x) && float.TryParse(coords[1], out float y))
    //                 {
    //                     points.Add(new Point(x, y));
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("File not found!");
    //     }
    //
    //     return points;
    // }
    
    // private void FormGraph()
    // {
    //     foreach (var point in points)
    //     {
    //         Instantiate(pointPrefab, new Vector3(point.x * multiplier, point.y * multiplier, 0), Quaternion.identity);
    //     }
    // }
}
