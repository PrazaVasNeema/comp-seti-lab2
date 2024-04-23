using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ProbDistFuncModel
{
    public static double GenerateNormal(System.Random rng, double mean, double stdDev)
    {
        // Использование метода Бокса-Мюллера
        double u1 = 1.0 - rng.NextDouble(); // равномерное распределение [0,1)
        double u2 = 1.0 - rng.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                               Math.Sin(2.0 * Math.PI * u2); // стандартное нормальное распределение
        double randNormal =
            mean + stdDev * randStdNormal; // нормальное распределение

        return Math.Abs(randNormal); // Модуль значения для |Gauss|
    }
    
    // ---
    
    private static double GenerateExponential(System.Random rng, double lambda)
    {
        double uniform = rng.NextDouble();
        return -Math.Log(uniform) / lambda;
    }
    
    public static double GenerateErlang(System.Random rng, int k, double lambda)
    {
        double erlang = 0.0;
        for (int i = 0; i < k; i++)
        {
            erlang += GenerateExponential(rng, lambda);
        }
        return erlang;
    }
}
