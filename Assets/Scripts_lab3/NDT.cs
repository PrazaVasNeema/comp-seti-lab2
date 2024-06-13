using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lab3
{
    public class NDT
    {
        public enum TargetView
        {
            MathExp,
            MainCompVertComp,
        }

        public class ViewData
        {

            public class Points
            {
                public float x;
                public float y;
                public Points(float x, float y)
                {
                    this.x = x;
                    this.y = y;
                }
            }

            public List<Points> pointsList = new List<Points>();
            public TargetView targetView;

            public ViewData(TargetView targetView)
            {
                this.targetView = targetView;
            }
        }
    }
}