using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaAndPointsVisualizer : MonoBehaviour
{
    public int resolution = 100; // Количество точек для отрисовки параболы
    public float xMin = -10f; // Минимальное значение x
    public float xMax = 10f; // Максимальное значение x
    public LineRenderer lineRenderer;
    public GameObject pointPrefab; // Префаб для визуализации точек
    public float basePointSize = 1f; // Базовый размер точки
    public float minRadius = 0.5f; // Минимальное значение радиуса
    public float maxRadius = 2.0f; // Максимальное значение радиуса

    public int numberOfPoints = 100; // Количество точек
    public float meanX = 0f;
    public float stdDevX = 10f;
    public float meanY = 50f;
    public float stdDevY = 20f;

    private List<GameObject> pointObjects = new List<GameObject>(); // Список созданных объектов точек
    private List<LineRenderer> radiusRenderers = new List<LineRenderer>(); // Список LineRenderer для визуализации радиусов

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        DrawParabola();
        VisualizePoints();
    }

    void DrawParabola()
    {
        float step = (xMax - xMin) / (resolution - 1);
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float x = xMin + i * step;
            float y = -Mathf.Pow(x, 2) + 100;
            points[i] = new Vector3(x, y, 0);
        }

        lineRenderer.SetPositions(points);
    }

    void VisualizePoints()
    {
        List<PointData> points = GeneratePointsBelowParabola(numberOfPoints, meanX, stdDevX, meanY, stdDevY, minRadius, maxRadius);
        foreach (var point in points)
        {
            GameObject pointObject = Instantiate(pointPrefab, new Vector3(point.position.x, point.position.y, 0), Quaternion.identity);
            pointObject.transform.localScale = Vector3.one * basePointSize;
            pointObjects.Add(pointObject);

            // Создаем LineRenderer для визуализации радиуса
            GameObject radiusObject = new GameObject("Radius");
            radiusObject.transform.position = new Vector3(point.position.x, point.position.y, 0);
            LineRenderer radiusRenderer = radiusObject.AddComponent<LineRenderer>();
            radiusRenderer.positionCount = 100;
            radiusRenderer.widthMultiplier = 0.05f;
            radiusRenderer.material = new Material(Shader.Find("Sprites/Default"));
            radiusRenderer.startColor = Color.blue;
            radiusRenderer.endColor = Color.blue;

            // Устанавливаем точки для LineRenderer
            Vector3[] radiusPoints = new Vector3[100];
            for (int i = 0; i < 100; i++)
            {
                float angle = i * Mathf.PI * 2 / 100;
                float x = Mathf.Cos(angle) * point.radius;
                float y = Mathf.Sin(angle) * point.radius;
                radiusPoints[i] = new Vector3(x, y, 0) + new Vector3(point.position.x, point.position.y, 0);
            }
            radiusRenderer.SetPositions(radiusPoints);

            radiusRenderers.Add(radiusRenderer);
        }
    }

    public void UpdatePointSizes(float zoom)
    {
        float scaleFactor = basePointSize * zoom / 25;
        foreach (var pointObject in pointObjects)
        {
            pointObject.transform.localScale = Vector3.one * scaleFactor;
        }

        foreach (var radiusRenderer in radiusRenderers)
        {
            // Изменяем ширину LineRenderer при изменении зума
            radiusRenderer.widthMultiplier = 0.1f * zoom / 25;
        }
    }

    List<PointData> GeneratePointsBelowParabola(int numberOfPoints, float meanX, float stdDevX, float meanY, float stdDevY, float minRadius, float maxRadius)
    {
        List<PointData> points = new List<PointData>();
        System.Random rand = new System.Random();

        while (points.Count < numberOfPoints)
        {
            float x = NextGaussian(rand, meanX, stdDevX);
            float y = NextGaussian(rand, meanY, stdDevY);
            float radius = (float)(rand.NextDouble() * (maxRadius - minRadius) + minRadius);

            if (y <= -Mathf.Pow(x, 2) + 100)
            {
                points.Add(new PointData(new Vector2(x, y), radius));
            }
        }

        return points;
    }

    float NextGaussian(System.Random rand, float mean, float stdDev)
    {
        // Использование трансформации Бокса-Мюллера для генерации значений с нормальным распределением
        double u1 = 1.0 - rand.NextDouble(); // Uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log((float)u1)) * Mathf.Sin(2.0f * Mathf.PI * (float)u2); // Random normal(0,1)
        return mean + stdDev * (float)randStdNormal; // Random normal(mean,stdDev^2)
    }
}

public class PointData
{
    public Vector2 position;
    public float radius;

    public PointData(Vector2 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
}
