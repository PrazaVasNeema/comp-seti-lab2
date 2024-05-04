// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class InvestigatePProstoi : InvestigatePieceAbstract
// {
//     public override void InvestigateOne(ServerModel.ServerLog serverLog)
//     {
//         chartData.targetChart = TargetChart.P_prostoi;
//
//         float timeProstoi = 0f;
//         float totalTime = 0f;
//         float prevTime = 0f;
//
//         bool serverWasBusy = true;
//         
//         foreach (var log in serverLog.generalLogDatasList)
//         {
//             if (log.serverTime > 0)
//             {
//                 if (!serverWasBusy)
//                 {
//                     timeProstoi += log.serverTime - prevTime;
//                 }
//
//                 totalTime = log.serverTime;
//                 prevTime = log.serverTime;
//                 serverWasBusy = log.serverIsBusy;
//             }
//         }
//
//         investigatedValueTempSum += timeProstoi / totalTime;
//         
//     }
//
// }
