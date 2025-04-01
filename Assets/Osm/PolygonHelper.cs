using System.Collections.Generic;
using Unity.Mathematics;

// https://stackoverflow.com/questions/71021941/how-to-generate-a-random-vector3-point-inside-a-polygon
// https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
public static class PolygonHelper
{
    public static bool IsPointInPolygon(List<double3> polygon, double3 testPoint)
    {
        bool result = false;
        int j = polygon.Count - 1;
        for (int i = 0; i < polygon.Count; i++)
        {
            if (polygon[i].y < testPoint.y && polygon[j].y >= testPoint.y || 
                polygon[j].y < testPoint.y && polygon[i].y >= testPoint.y)
            {
                if (polygon[i].x + (testPoint.y - polygon[i].y) /
                    (polygon[j].y - polygon[i].y) *
                    (polygon[j].x - polygon[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

}