using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lab4
{

    public class Visualizations : MonoBehaviour
    {
        public int resolution = 100; // Количество точек для отрисовки параболы
        public float xMin = -10f; // Минимальное значение x
        public float xMax = 10f; // Максимальное значение x
        public LineRenderer lineRenderer;
        public GameObject pointPrefab; // Префаб для визуализации точек
        public float basePointSize = 1f; // Базовый размер точки
        private List<GameObject> pointObjects = new List<GameObject>(); // Список созданных объектов точек
        private List<LineRenderer> radiusRenderers = new List<LineRenderer>(); // Список LineRenderer для визуализации радиусов

        [SerializeField] private GameObject arrowPrefab;
        private List<GameObject> arrowObjects = new List<GameObject>();

        private List<LineRenderer> arrowRenderers = new List<LineRenderer>(); // Список LineRenderer для визуализации радиусов

        private List<PointData> pointDataList = new List<PointData>();

        void Start()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = resolution;
            lineRenderer.widthMultiplier = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }

        private void Update()
        {
            for (int i = 0; i < pointObjects.Count; i++)
            {
                pointDataList[i].Test(Time.time);
                pointObjects[i].transform.position = new Vector3(pointDataList[i].position.x, pointDataList[i].position.y, 0);

            }

            if (pointDataList.Count > 0)
                Debug.Log(pointDataList[0].position.x);


        }

        public void Clear()
        {
            foreach (GameObject obj in pointObjects)
            {
                Destroy(obj);
            }

            foreach (LineRenderer obj in radiusRenderers)
            {
                Destroy(obj.transform.gameObject);
            }

            foreach (GameObject obj in arrowObjects)
            {
                Destroy(obj);
            }

            foreach (LineRenderer obj in arrowRenderers)
            {
                Destroy(obj.transform.gameObject);
            }

            pointObjects = new List<GameObject>();
            radiusRenderers = new List<LineRenderer>();
            arrowObjects = new List<GameObject>();
            arrowRenderers = new List<LineRenderer>();
        }    

        public void DrawParabola()
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

        public void VisualizePoints(List<PointData> points, bool withRadius = true)
        {
            pointDataList = points;
            foreach (var point in points)
            {
                GameObject pointObject = Instantiate(pointPrefab, new Vector3(point.position.x, point.position.y, 0), Quaternion.identity);
                pointObject.transform.localScale = Vector3.one * basePointSize;
                pointObjects.Add(pointObject);

                if (withRadius)
                {
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

                TrailRenderer trail = pointObject.AddComponent<TrailRenderer>();
                trail.time = 10.0f; // Время жизни следа
                trail.startWidth = 0.1f;
                trail.endWidth = 0.0f;
                trail.material = new Material(Shader.Find("Sprites/Default")); // Установите материал для следа
                trail.startColor = Color.blue;
                trail.endColor = Color.clear;
            }
        }


        public void VisualizeEdges(List<PointData> points, ref int[,] adjacencyMatrix)
        {
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (adjacencyMatrix[i, j] == 1)
                    {
                        Vector3 start = new Vector3(points[i].position.x, points[i].position.y, 0);
                        Vector3 end = new Vector3(points[j].position.x, points[j].position.y, 0);

                        // Создаем LineRenderer для ребра
                        GameObject edgeObject = new GameObject("Edge");
                        LineRenderer edgeRenderer = edgeObject.AddComponent<LineRenderer>();
                        edgeRenderer.positionCount = 2;
                        edgeRenderer.widthMultiplier = 0.05f;
                        edgeRenderer.material = new Material(Shader.Find("Sprites/Default"));
                        edgeRenderer.startColor = Color.green;
                        edgeRenderer.endColor = Color.green;
                        edgeRenderer.SetPositions(new Vector3[] { start, end });
                        arrowRenderers.Add(edgeRenderer);

                        // Создаем стрелку
                        GameObject arrowObject = Instantiate(arrowPrefab, end, Quaternion.identity);
                        Vector3 direction = (end - start).normalized;
                        arrowObject.transform.position = end - direction * .25f; // Смещение стрелки немного назад
                        //arrowObject.transform.rotation = Quaternion.LookRotation(arrowObject.transform.forward, direction);
                        arrowObject.transform.LookAt(end);
                        arrowObjects.Add(arrowObject);
                    }
                }
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
                radiusRenderer.widthMultiplier = 0.05f * zoom / 25;
            }

            var test = basePointSize * 5 / 25;
            foreach (var arrowObject in arrowObjects)
            {
                arrowObject.transform.localScale = Vector3.one * test * 20;
            }

            foreach (var arrowRenderer in arrowRenderers)
            {
                // Изменяем ширину LineRenderer при изменении зума
                arrowRenderer.widthMultiplier = 0.05f * zoom / 20;
            }
        }
    }

}