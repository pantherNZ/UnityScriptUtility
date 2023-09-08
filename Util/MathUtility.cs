using UnityEngine;

public static partial class Utility
{
    public static float Lerp( float a, float b, float interp ) { return a + ( b - a ) * interp; }
    public static int Lerp( int a, int b, float interp ) { return ( int )( a + ( b - a ) * interp ); }
    public static Vector2 Lerp( Vector2 a, Vector2 b, float interp ) { return a + ( b - a ) * interp; }
    public static Vector3 Lerp( Vector3 a, Vector3 b, float interp ) { return a + ( b - a ) * interp; }
    public static Vector4 Lerp( Vector4 a, Vector4 b, float interp ) { return a + ( b - a ) * interp; }
}

public static partial class Extensions
{


}