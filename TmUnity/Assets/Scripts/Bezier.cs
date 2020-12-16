using UnityEngine;
using System.Collections.Generic;
namespace Eccentric
{
    class Bezier
    {
        Vector3[] controlPoint = null;
        float precision = 0f;
        public Bezier(Vector3[] controlPoint, float precision)
        {
            this.controlPoint = controlPoint;
            this.precision = precision;
        }

        public Vector3[] GetCurvesPoint()
        {
            var points = new List<Vector3>();
            points.Add(controlPoint[0]);
            for (float f = precision; f <= 1f; f += precision)
                points.Add(
                    Mathf.Pow(1 - f, 3) * controlPoint[0] +
                    3 * Mathf.Pow(1 - f, 2) * f * controlPoint[1] +
                    3 * (1 - f) * Mathf.Pow(f, 2) * controlPoint[2] +
                    Mathf.Pow(f, 3) * controlPoint[3]
                );
            points.Add(controlPoint[3]);
            return points.ToArray();
        }
    }
}