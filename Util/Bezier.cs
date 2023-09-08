using System.Collections.Generic;
using UnityEngine;

public static partial class Utility
{
    static List<List<int>> lookupTable = new List<List<int>>
    {
        new List<int>{ 1 },
        new List<int>{ 1, 1 },
        new List<int>{ 1, 2, 1 },
        new List<int>{ 1, 3, 3, 1 },
        new List<int>{ 1, 4, 6, 4, 1 },
        new List<int>{ 1, 5, 10, 10, 5, 1 },
        new List<int>{ 1, 6, 15, 20, 15, 6, 1 }
    };

    public static int Binomial( int n, int k )
    {
        while( n >= lookupTable.Count )
        {
            var newRow = new List<int> { 1 };
            var length = lookupTable.Count - 1;

            for( int i = 1; i < length; i++ )
                newRow.Add( lookupTable[length][i - 1] + lookupTable[length][i] );

            newRow.Add( 1 );
            lookupTable.Add( newRow );
        }

        return lookupTable[n][k];
    }

    public static Vector3 Bezier( float interval, List<Vector3> controlPoints )
    {
        if( controlPoints.Count == 1 )
            return controlPoints[0];

        if( controlPoints.Count == 2 )
            return Lerp( controlPoints[0], controlPoints[1], interval );

        if( controlPoints.Count == 3 )
            return BezierQuadratic( interval, controlPoints[0], controlPoints[1], controlPoints[2] );

        if( controlPoints.Count == 4 )
            return BezierCubic( interval, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3] );

        var result = new Vector3();
        int n = controlPoints.Count - 1;

        for( int k = 0; k < controlPoints.Count; ++k )
            result += controlPoints[k] * Binomial( n, k ) * Mathf.Pow( 1.0f - interval, n - k ) * Mathf.Pow( interval, k );

        return result;
    }

    public static Vector3 BezierQuadratic( float interval, Vector3 start, Vector3 corner, Vector3 end )
    {
        var t2 = interval * interval;
        var mt = 1.0f - interval;
        var mt2 = mt * mt;
        return start * mt2 + corner * 2.0f * mt * interval + end * t2;
    }

    public static Vector3 BezierCubic( float interval, Vector3 start, Vector3 cornerA, Vector3 cornerB, Vector3 end )
    {
        var t2 = interval * interval;
        var t3 = t2 * interval;
        var mt = 1.0f - interval;
        var mt2 = mt * mt;
        var mt3 = mt2 * mt;
        return start * mt3 + 3.0f * cornerA * mt2 * interval + 3.0f * cornerB * mt * t2 + end * t3;
    }

    public static float CalculateBezierCurveLength( List<Vector3> controlPoints, int numSteps )
    {
        float length = 0.0f;
        float step = 0.0f;

        for( int i = 0; i < numSteps - 1; ++i )
        {
            var p0 = Bezier( step, controlPoints );
            step += ( 1.0f / numSteps );
            var p1 = Bezier( step, controlPoints );
            length += ( p1 - p0 ).magnitude;
        }

        return length;
    }

    //public static void IterateBezier( List<Vector3> controlPoints, float distanceBetween, int numSteps, System.Action<Vector3> predicate )
    //{
    //
    //}
    //
    //public static Vector3 GetBezierPoint( List<Vector3> controlPoints, float distance )
    //{
    //
    //}
}