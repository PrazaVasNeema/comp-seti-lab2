using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace lab3
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


        void Start()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = resolution;
            lineRenderer.widthMultiplier = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
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
            pointObjects = new List<GameObject>();
            radiusRenderers = new List<LineRenderer>();
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

        public void VisualizePoints(List<NDT.PointData> points)
        {
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
                radiusRenderer.widthMultiplier = 0.05f * zoom / 25;
            }
        }
    }

}