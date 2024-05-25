using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Form3DGraph : MonoBehaviour
{
    public struct Point
    {
        public float x;
        public float y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"Point(x: {x}, y: {y})";
        }
    }
    
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private float multiplier = 5f;
    
    public List<Point> points;

    void Start()
    {
        points = ReadPointsFromFile("vertex_positions");
        foreach (var point in points)
        {
            Debug.Log(point);
        }
        
        FormGraph();
    }

    List<Point> ReadPointsFromFile(string fileName)
    {
        List<Point> points = new List<Point>();

        TextAsset file = Resources.Load<TextAsset>(fileName);
        if (file != null)
        {
            StringReader reader = new StringReader(file.text);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] coords = line.Split(' ');
                if (coords.Length == 2)
                {
                    if (float.TryParse(coords[0], out float x) && float.TryParse(coords[1], out float y))
                    {
                        points.Add(new Point(x, y));
                    }
                }
            }
        }
        else
        {
            Debug.LogError("File not found!");
        }

        return points;
    }
    
    private void FormGraph()
    {
        foreach (var point in points)
        {
            Instantiate(pointPrefab, new Vector3(point.x * multiplier, point.y * multiplier, 0), Quaternion.identity);
        }
    }
}
