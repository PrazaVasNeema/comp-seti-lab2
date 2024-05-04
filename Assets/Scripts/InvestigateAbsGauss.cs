// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class InvestigateAbsGauss : InvestigatePieceAbstract
// {
//     public override void InvestigateOne(ServerModel.ServerLog serverLog)
//     {
//         chartData.targetChart = TargetChart.Gauss;
//
//         float sumAx = 0f;
//         int iterCount = 0;
//         
//         foreach (var log in serverLog.generalLogDatasList)
//         {
//             if (log.BxValue > 0)
//             {
//                 sumAx += log.BxValue;
//                 iterCount++;
//             }
//         }
//         
//         sumAx /= iterCount;
//         
//         investigatedValueTempSum += sumAx;
//         
//     }
//
// }
