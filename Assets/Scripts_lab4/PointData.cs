using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab4
{

    public class PointData
    {

        public Vector2 position;
        public float radius;

        public PointData(Vector2 position, float radius)
        {
            this.position = position;
            this.radius = radius;

            this.currentTime = 0;
            ChooseNewPattern();
        }

        public PointData Clone()
        {
            PointData newPointData = new PointData(position, radius);
            newPointData.direction = this.direction;
            newPointData.remainingTime = this.remainingTime;
            newPointData.currentPattern = this.currentPattern;

            return newPointData;
        }

        private float currentTime;
        private string currentPattern;
        private float remainingTime;
        private Vector2 direction;

        private static readonly string[] patterns = { "Way-Point", "Scan" };
        private static float p_v;
        private static float p_t;

        public static void SetStaticPParams(float p_t_new, float p_v__new)
        {
            p_v = p_v__new;
            p_t = p_t_new;
        }


        // Move logics

        private void ChooseNewPattern()
        {
            System.Random rand = new System.Random();

            currentPattern = patterns[rand.NextDouble() * 10 < 5 ? 0 : 1];
            remainingTime = p_t;

            if (currentPattern == "Way-Point")
            {
                // Выбор случайного направления для Way-Point движения
                direction = new Vector2((float)rand.NextDouble() * 2f - 1f, (float)rand.NextDouble() * 2f - 1f).normalized * p_v;
            }
            else if (currentPattern == "Scan")
            {
                // Направление сканирования (можно изменить логику для конкретной области)
                direction = new Vector2(p_v, 0);
            }
        }

        public static PointData GetNewPointData(float newTime, PointData oldPoint)
        {
            PointData newPoint = oldPoint.Clone();

            float deltaTime = newTime - newPoint.currentTime;
            newPoint.currentTime = newTime;



            if (newPoint.remainingTime > 0)
            {
                newPoint.remainingTime -= deltaTime;
                newPoint.Move(deltaTime);
            }
            else
            {
                newPoint.ChooseNewPattern();
            }

            // Проверка на выход за границы параболы
            if (newPoint.position.y > -Mathf.Pow(newPoint.position.x, 2) + 100)
            {
                newPoint.position.y = -Mathf.Pow(newPoint.position.x, 2) + 100;
            }

            return newPoint;
        }

        private void Move(float deltaTime)
        {
            position += direction * deltaTime;

            if (currentPattern == "Scan")
            {
                // Логика для движения по траектории Scan (например, движение вперед-назад)
                direction = new Vector2(p_v * Mathf.Cos(currentTime), p_v * Mathf.Sin(currentTime));
            }
        }
    }

}