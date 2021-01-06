using UnityEngine;
using Eccentric;
using System.Linq;
public class Test : MonoBehaviour
{
    [SerializeField] Transform[] controlPoints = null;
    void OnDrawGizmos()
    {
        if (controlPoints == null || controlPoints.Length < 4) return;
        var v3 = controlPoints.Select(cp => cp.position).ToArray();

        var b = new Bezier(v3, .1f);
        var points = b.GetCurvesPoint();
        foreach (var p in points)
        {
            Gizmos.DrawSphere(p, .2f);
        }
    }
}
