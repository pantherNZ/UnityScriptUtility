using System;
using System.Diagnostics.Contracts;
using UnityEngine;

public static partial class Utility
{
    public static float Lerp( float a, float b, float interp ) { return a + ( b - a ) * interp; }
    public static int Lerp( int a, int b, float interp ) { return ( int )( a + ( b - a ) * interp ); }
    public static Vector2 Lerp( this Vector2 a, Vector2 b, float interp ) { return a + ( b - a ) * interp; }
    public static Vector3 Lerp( this Vector3 a, Vector3 b, float interp ) { return a + ( b - a ) * interp; }
    public static Vector4 Lerp( this Vector4 a, Vector4 b, float interp ) { return a + ( b - a ) * interp; }

	// Use this, not lerp https://www.youtube.com/watch?v=LSNQuFEDOyQ&list=WL&index=6&t=24s
	// Exponential decay constant, 1-25 useful range (slow to fast)
	public static float ExpLerp( float a, float b, float decay ) { return b + ( a - b ) * ExpDecayVal( decay ); }
	public static int ExpLerp( int a, int b, float decay ) { return ( int )( b + ( a - b ) * ExpDecayVal( -decay ) ); }
	public static Vector2 ExpLerp( this Vector2 a, Vector2 b, float decay ) { return b + ( a - b ) * ExpDecayVal( decay  ); }
	public static Vector3 ExpLerp( this Vector3 a, Vector3 b, float decay ) { return b + ( a - b ) * ExpDecayVal( decay  ); }
	public static Vector4 ExpLerp( this Vector4 a, Vector4 b, float decay ) { return b + ( a - b ) * ExpDecayVal( decay ); }
	private static float ExpDecayVal( float decay ) => Mathf.Exp( -decay * Time.deltaTime );

	public static float Distance( this GameObject a, GameObject b ) { return Distance( a.transform, b.transform ); }
    public static float Distance( this GameObject a, Transform b ) { return Distance( a.transform, b ); }
    public static float Distance( this Transform a, GameObject b ) { return Distance( a, b.transform ); }
    public static float Distance( this Transform a, Transform b ) { return Mathf.Sqrt( DistanceSq( a, b ) ); }
    public static float Distance( this Vector3 a, Vector3 b ) { return Mathf.Sqrt( DistanceSq( a, b ) ); }

    public static float DistanceSq( this GameObject a, GameObject b ) { return DistanceSq( a.transform, b.transform ); }
    public static float DistanceSq( this GameObject a, Transform b ) { return DistanceSq( a.transform, b ); }
    public static float DistanceSq( this Transform a, GameObject b ) { return DistanceSq( a, b.transform ); }
    public static float DistanceSq( this Transform a, Transform b ) { return DistanceSq( a.position, b.position ); }
    public static float DistanceSq( this Vector3 a, Vector3 b ) { return ( a - b ).sqrMagnitude; }

    public static Vector2 Set( this Vector2 vec, float val ) { return vec.SetX( val ).SetY( val ); }
    public static Vector3 Set( this Vector3 vec, float val ) { return vec.SetX( val ).SetY( val ).SetZ( val ); }
    public static Vector4 Set( this Vector4 vec, float val ) { return vec.SetX( val ).SetY( val ).SetZ( val ).SetW( val ); }

    [Pure] public static Vector2 SetMagnitude( this Vector2 vec, float val ) { return vec.sqrMagnitude < 0.0001f ? vec : vec.normalized * val; }
    [Pure] public static Vector3 SetMagnitude( this Vector3 vec, float val ) { return vec.sqrMagnitude < 0.0001f ? vec : vec.normalized * val; }
    [Pure] public static Vector4 SetMagnitude( this Vector4 vec, float val ) { return vec.sqrMagnitude < 0.0001f ? vec : vec.normalized * val; }

    public static Vector2 SetX( this Vector2 vec, float x ) { vec.x = x; return vec; }
    public static Vector2 SetY( this Vector2 vec, float y ) { vec.y = y; return vec; }
    public static Vector3 SetX( this Vector3 vec, float x ) { vec.x = x; return vec; }
    public static Vector3 SetY( this Vector3 vec, float y ) { vec.y = y; return vec; }
    public static Vector3 SetZ( this Vector3 vec, float z ) { vec.z = z; return vec; }
    public static Vector4 SetX( this Vector4 vec, float x ) { vec.x = x; return vec; }
    public static Vector4 SetY( this Vector4 vec, float y ) { vec.y = y; return vec; }
    public static Vector4 SetZ( this Vector4 vec, float z ) { vec.z = z; return vec; }
    public static Vector4 SetW( this Vector4 vec, float w ) { vec.w = w; return vec; }
    public static Vector2 XZ( this Vector3 vec ) { return new Vector2( vec.x, vec.z ); }
    public static Vector2 YZ( this Vector3 vec ) { return new Vector2( vec.y, vec.z ); }
    public static Vector2 ZX( this Vector3 vec ) { return new Vector2( vec.z, vec.x ); }
    public static Vector2 YX( this Vector3 vec ) { return new Vector2( vec.y, vec.x ); }
    public static Vector2 ZY( this Vector3 vec ) { return new Vector2( vec.z, vec.y ); }
    public static Vector2 YX( this Vector2 vec ) { return new Vector2( vec.y, vec.x ); }

    public static Vector2Int Set( this Vector2Int vec, int val ) { return vec.SetX( val ).SetY( val ); }
    public static Vector3Int Set( this Vector3Int vec, int val ) { return vec.SetX( val ).SetY( val ).SetZ( val ); }

    public static Vector2Int SetX( this Vector2Int vec, int x ) { vec.x = x; return vec; }
    public static Vector2Int SetY( this Vector2Int vec, int y ) { vec.y = y; return vec; }
    public static Vector3Int SetX( this Vector3Int vec, int x ) { vec.x = x; return vec; }
    public static Vector3Int SetY( this Vector3Int vec, int y ) { vec.y = y; return vec; }
    public static Vector3Int SetZ( this Vector3Int vec, int z ) { vec.z = z; return vec; }
    public static Vector2Int XZ( this Vector3Int vec ) { return new Vector2Int( vec.x, vec.z ); }
    public static Vector2Int YZ( this Vector3Int vec ) { return new Vector2Int( vec.y, vec.z ); }
    public static Vector2Int ZX( this Vector3Int vec ) { return new Vector2Int( vec.z, vec.x ); }
    public static Vector2Int YX( this Vector3Int vec ) { return new Vector2Int( vec.y, vec.x ); }
    public static Vector2Int ZY( this Vector3Int vec ) { return new Vector2Int( vec.z, vec.y ); }
    public static Vector2Int YX( this Vector2Int vec ) { return new Vector2Int( vec.y, vec.x ); }

    /* REF vec 3 versions
    public static Vector2 Set( this Vector2 vec, float val ) { vec.SetX( val ); vec.SetY( val ); return vec; }
    public static Vector3 Set( ref this Vector3 vec, float val ) { vec.SetX( val ); vec.SetY( val ); vec.SetZ( val ); return vec; }
    public static Vector4 Set( ref this Vector4 vec, float val ) { vec.SetX( val ); vec.SetY( val ); vec.SetZ( val ); vec.SetW( val ); return vec; }

    public static Vector2 SetMagnitude( ref this Vector2 vec, float val ) { vec.Normalize(); vec *= val; return vec; }
    public static Vector3 SetMagnitude( ref this Vector3 vec, float val ) { vec.Normalize(); vec *= val; return vec; }
    public static Vector4 SetMagnitude( ref this Vector4 vec, float val ) { vec.Normalize(); vec *= val; return vec; }

    public static Vector2 SetX( ref this Vector2 vec, float x ) { vec.x = x; return vec; }
    public static Vector2 SetY( ref this Vector2 vec, float y ) { vec.y = y; return vec; }
    public static Vector3 SetX( ref this Vector3 vec, float x ) { vec.x = x; return vec; }
    public static Vector3 SetY( ref this Vector3 vec, float y ) { vec.y = y; return vec; }
    public static Vector3 SetZ( ref this Vector3 vec, float z ) { vec.z = z; return vec; }
    public static Vector4 SetX( ref this Vector4 vec, float x ) { vec.x = x; return vec; }
    public static Vector4 SetY( ref this Vector4 vec, float y ) { vec.y = y; return vec; }
    public static Vector4 SetZ( ref this Vector4 vec, float z ) { vec.z = z; return vec; }
    public static Vector4 SetW( ref this Vector4 vec, float w ) { vec.w = w; return vec; }

    public static Vector2Int Set( ref this Vector2Int vec, int val ) { return vec.SetX( val ).SetY( val ); }
    public static Vector3Int Set( ref this Vector3Int vec, int val ) { return vec.SetX( val ).SetY( val ).SetZ( val ); }

    public static Vector2Int SetX( ref this Vector2Int vec, int x ) { vec.x = x; return vec; }
    public static Vector2Int SetY( ref this Vector2Int vec, int y ) { vec.y = y; return vec; }
    public static Vector3Int SetX( ref this Vector3Int vec, int x ) { vec.x = x; return vec; }
    public static Vector3Int SetY( ref this Vector3Int vec, int y ) { vec.y = y; return vec; }
    public static Vector3Int SetZ( ref this Vector3Int vec, int z ) { vec.z = z; return vec; }
    */

    // Helper function to get Modulus (not remainder which is what % gives)
    public static int Mod( int a, int b )
    {
        return ( ( a %= b ) < 0 ) ? a + b : a;
    }

    public static float Mod( float a, float b )
    {
        return ( ( a %= b ) < 0.0f ) ? a + b : a;
    }

	public static Vector2 Vector2FromAngle( float angleDegrees )
    {
        return new Vector2( Mathf.Cos( angleDegrees * Mathf.Deg2Rad ), Mathf.Sin( angleDegrees * Mathf.Deg2Rad ) );
    }

    public static Vector2 ToVector2( this Vector3 vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector2 ToVector2( this Vector4 vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector3 ToVector3( this Vector4 vec ) { return new Vector3( vec.x, vec.y, vec.z ); }
    public static Vector3 ToVector3( this Vector2 vec, float z = 0.0f ) { return new Vector3( vec.x, vec.y, z ); }
    public static Vector3 ToVector3XZ( this Vector2 vec, float y = 0.0f ) { return new Vector3( vec.x, y, vec.y ); }
    public static Vector2 ToVector2( this Vector2Int vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector3 ToVector3( this Vector2Int vec, float z = 0.0f ) { return new Vector3( vec.x, vec.y, z ); }
    public static Vector3 ToVector3XZ( this Vector2Int vec, float y = 0.0f ) { return new Vector3( vec.x, y, vec.y ); }
    public static Vector3 ToVector3( this Vector3Int vec ) { return new Vector3( vec.x, vec.y, vec.z ); }
	public static Vector3 ToVector3XZ( this Vector3Int vec, float y = 0.0f ) { return new Vector3( vec.x, y, vec.y ); }

	public static Vector2Int ToVector2Int( this Vector2 vec, bool round = false )
    {
        if( round )
            return new Vector2Int( Mathf.RoundToInt( vec.x ), Mathf.RoundToInt( vec.y ) );
        return new Vector2Int( Mathf.FloorToInt( vec.x ), Mathf.FloorToInt( vec.y ) );
    }

    public static Vector3Int ToVector3Int( this Vector3 vec, bool round = false )
    {
        if( round )
            return new Vector3Int( Mathf.RoundToInt( vec.x ), Mathf.RoundToInt( vec.y ), Mathf.RoundToInt( vec.z ) );
        return new Vector3Int( Mathf.FloorToInt( vec.x ), Mathf.FloorToInt( vec.y ), Mathf.RoundToInt( vec.z ) );
    }

    public static float Angle( this Vector2 vec )
    {
        if( vec.x < 0 )
        {
            return 360.0f - ( Mathf.Atan2( vec.x, vec.y ) * Mathf.Rad2Deg * -1.0f );
        }
        else
        {
            return Mathf.Atan2( vec.x, vec.y ) * Mathf.Rad2Deg;
        }
    }

    public static Vector2 Rotate( this Vector2 vec, float angleDegrees )
    {
        return Quaternion.AngleAxis( angleDegrees, Vector3.forward ) * vec;
    }

    public static Vector3 RotateX( this Vector3 vec, float angleDegrees )
    {
        return Quaternion.AngleAxis( angleDegrees, Vector3.right ) * vec;
    }

    public static Vector3 RotateY( this Vector3 vec, float angleDegrees )
    {
        return Quaternion.AngleAxis( angleDegrees, Vector3.up ) * vec;
    }

    public static Vector3 RotateZ( this Vector3 vec, float angleDegrees )
    {
        return Quaternion.AngleAxis( angleDegrees, Vector3.forward ) * vec;
    }

    public static Vector3 Clamp( this Vector3 vec, float min, float max )
    {
        var length = vec.magnitude;
        if( length > max )
            vec *= max / length;
        if( length < min )
            vec *= min / length;
        return vec;
    }

    public static Vector2 RandomPosition( this Rect rect, IRandom rng = null )
    {
        return new Vector2(
            rect.x + ( rng ?? Utility.DefaultRng ).value * rect.width,
            rect.y + ( rng ?? Utility.DefaultRng ).value * rect.height );
    }

	public static Vector3 RandomPosition( this Bounds bound, IRandom rng = null )
	{
		return bound.min + new Vector3(
			( rng ?? Utility.DefaultRng ).value * bound.size.x,
			( rng ?? Utility.DefaultRng ).value * bound.size.y,
			( rng ?? Utility.DefaultRng ).value * bound.size.z );
	}

	public static Vector3Int RandomPosition( this BoundsInt bound, IRandom rng = null )
	{
		return new Vector3Int(
			( rng ?? Utility.DefaultRng ).Range( bound.xMin, bound.xMax + 1 ),
			( rng ?? Utility.DefaultRng ).Range( bound.yMin, bound.yMax + 1 ),
			( rng ?? Utility.DefaultRng ).Range( bound.zMin, bound.zMax + 1 ) );
	}

	public static Color Random( this Gradient gradient, IRandom rng = null )
	{
		return gradient.Evaluate( ( rng ?? Utility.DefaultRng ).value );
	}

	public static bool Overlaps( this RectTransform rectTrans1, RectTransform rectTrans2 )
    {
        Rect rect1 = new Rect( rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width * rectTrans1.localScale.x, rectTrans1.rect.height * rectTrans1.localScale.y );
        Rect rect2 = new Rect( rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width * rectTrans1.localScale.x, rectTrans2.rect.height * rectTrans1.localScale.y );

        return rect1.Overlaps( rect2 );
    }


    public static Vector2 TopLeft( this Rect rect ) { return new Vector2( rect.xMin, rect.yMax ); }
    public static Vector2 TopRight( this Rect rect ) { return new Vector2( rect.xMax, rect.yMax ); }
    public static Vector2 BottomLeft( this Rect rect ) { return new Vector2( rect.xMin, rect.yMin ); }
    public static Vector2 BottomRight( this Rect rect ) { return new Vector2( rect.xMax, rect.yMin ); }
    public static Rect ToRect( this Bounds bound ) { return new Rect( bound.center - bound.extents, bound.size ); }

	public static bool Contains( this Rect rect, Rect other )
    {
        return rect.Contains( other.TopLeft() )
             && rect.Contains( other.TopRight() )
             && rect.Contains( other.BottomLeft() )
             && rect.Contains( other.BottomRight() );
    }

    public static bool Contains( this Rect rect, Bounds other )
    {
        return rect.Contains( other.ToRect() );
    }

	public static Rect Inflate( this ref Rect rect, Vector2 pos )
	{
		rect = new Rect(
			Mathf.Min( rect.xMin, pos.x ),
			Mathf.Min( rect.yMin, pos.y ),
			Mathf.Max( rect.xMax, pos.x ) - Mathf.Min( rect.xMin, pos.x ),
			Mathf.Max( rect.yMax, pos.y ) - Mathf.Min( rect.yMin, pos.y ) );
		return rect;
	}

	public static RectInt Inflate( this ref RectInt rect, Vector2Int pos )
	{
		rect = new RectInt(
			Mathf.Min( rect.xMin, pos.x ),
			Mathf.Min( rect.yMin, pos.y ),
			Mathf.Max( rect.xMax, pos.x ) - Mathf.Min( rect.xMin, pos.x ),
			Mathf.Max( rect.yMax, pos.y ) - Mathf.Min( rect.yMin, pos.y ) );
		return rect;
	}

	public static Vector3 UnrotateVector( this Quaternion quat, Vector3 v )
    {
        return Matrix4x4.Rotate( quat ).transpose.MultiplyVector( v );
    }

    public static Vector3 RotateVector( this Quaternion quat, Vector3 v )
    {
        return Matrix4x4.Rotate( quat ).MultiplyVector( v );
    }

    public static Vector3 GetScaledAxis( this Matrix4x4 mat, EAxis InAxis )
    {
        switch( InAxis )
        {
            case EAxis.X:
                return new Vector3( mat[0, 0], mat[0, 1], mat[0, 2] );

            case EAxis.Y:
                return new Vector3( mat[1, 0], mat[1, 1], mat[1, 2] );

            case EAxis.Z:
                return new Vector3( mat[2, 0], mat[2, 1], mat[2, 2] );

            default:
                Debug.LogError( "GetScaledAxis: Invalid axis" );
                return Vector3.zero;
        }
    }

    public static void GetScaledAxes( this Matrix4x4 mat, out Vector3 X, out Vector3 Y, out Vector3 Z )
    {
        X = new Vector3( mat[0, 0], mat[0, 1], mat[0, 2] );
        Y = new Vector3( mat[1, 0], mat[1, 1], mat[1, 2] );
        Z = new Vector3( mat[2, 0], mat[2, 1], mat[2, 2] );
    }

    public static Vector3 GetUnitAxis( this Matrix4x4 mat, EAxis InAxis )
    {
        return mat.GetScaledAxis( InAxis ).normalized;
    }

    public static void GetUnitAxes( this Matrix4x4 mat, Vector3 x, Vector3 y, Vector3 z )
    {
        mat.GetScaledAxes( out x, out y, out z );
        x.Normalize();
        y.Normalize();
        z.Normalize();
    }

    public static void SetAxis( this Matrix4x4 mat, int i, Vector3 axis )
    {
        //checkSlow( i >= 0 && i <= 2 );
        mat[i, 0] = axis.x;
        mat[i, 1] = axis.y;
        mat[i, 2] = axis.z;
    }

    static public void SetOrigin( this Matrix4x4 mat, Vector3 newOrigin )
    {
        mat[3, 0] = newOrigin.x;
        mat[3, 1] = newOrigin.y;
        mat[3, 2] = newOrigin.z;
    }

    static public void SetAxes( this Matrix4x4 mat, Vector3? axis0 = null, Vector3? axis1 = null, Vector3? axis2 = null, Vector3? origin = null )
    {
        if( axis0 != null )
        {
            mat[0, 0] = axis0.Value.x;
            mat[0, 1] = axis0.Value.y;
            mat[0, 2] = axis0.Value.z;
        }
        if( axis1 != null )
        {
            mat[1, 0] = axis1.Value.x;
            mat[1, 1] = axis1.Value.y;
            mat[1, 2] = axis1.Value.z;
        }
        if( axis2 != null )
        {
            mat[2, 0] = axis2.Value.x;
            mat[2, 1] = axis2.Value.y;
            mat[2, 2] = axis2.Value.z;
        }
        if( origin != null )
        {
            mat[3, 0] = origin.Value.x;
            mat[3, 1] = origin.Value.y;
            mat[3, 2] = origin.Value.z;
        }
    }

    public static Vector3 Rotation( this Vector3 vec )
    {
        return new Vector3( Mathf.Atan2( vec.y, vec.x ), Mathf.Atan2( vec.z, vec.ToVector2().magnitude ), 0.0f );
    }

    public static void SetTransformData( this Transform transform, TransformData data )
    {
        transform.position = data.translation;
        transform.localScale = data.scale;
        transform.rotation = data.rotation;
    }

	public static TransformData GetTransformData( this Transform transform )
	{
		return new TransformData()
		{
			translation = transform.position,
			scale = transform.localScale,
			rotation = transform.rotation
		};
	}

	public static short SafeDivide( this short v, short denominator ) { return ( short )( denominator == 0 ? 0 : v / denominator ); }
    public static int SafeDivide( this int v, int denominator ) { return denominator == 0 ? 0 : v / denominator; }
    public static long SafeDivide( this long v, long denominator ) { return denominator == 0L ? 0L : v / denominator; }
    public static ushort SafeDivide( this ushort v, ushort denominator ) { return ( ushort )( denominator == 0 ? 0 : v / denominator ); }
    public static uint SafeDivide( this uint v, uint denominator ) { return denominator == 0U ? 0U : v / denominator; }
    public static ulong SafeDivide( this ulong v, ulong denominator ) { return denominator == 0UL ? 0UL : v / denominator; }
    public static float SafeDivide( this float v, float denominator ) { return denominator == 0.0f ? 0.0f : v / denominator; }
    public static double SafeDivide( this double v, double denominator ) { return denominator == 0.0d ? 0.0d : v / denominator; }
    public static decimal SafeDivide( this decimal v, decimal denominator ) { return denominator == 0.0m ? 0.0m : v / denominator; }
    public static byte SafeDivide( this byte v, byte denominator ) { return ( byte )( denominator == 0 ? 0 : v / denominator ); }

	public static string ToQuantifiedString( int number )
	{
		if ( number < 1000 )
			return number.ToString();
		else if ( number < 10000 )
			return string.Format( "{0:0.00}K", number / 1000.0f );
		else if ( number < 100000 )
			return string.Format( "{0:0.0}K", number / 1000.0f );
		else if ( number < 1000000 )
			return string.Format( "{0}K", number / 1000 );
		else if ( number < 10000000 )
			return string.Format( "{0:0.00}M", number / 1000000.0f );
		else if ( number < 100000000 )
			return string.Format( "{0:0.0}M", number / 1000000.0f );
		else if ( number < 1000000000 )
			return string.Format( "{0}M", number / 1000000 );
		else
			return string.Format( "{0:0.00}G", number / 1000000000.0f );
	}

	public static bool RayCastAgainst2DCircle( Vector2 rayStart, Vector2 rayEnd, Vector2 circleCenter, float circleRadius, out Vector2 hitPoint )
	{
		Vector2 d = rayEnd - rayStart;
		Vector2 f = rayStart - circleCenter;
		float r = circleRadius;
		float a = Vector2.Dot( d, d );
		float b = 2 * Vector2.Dot( f, d );
		float c = Vector2.Dot( f, f ) - r * r;

		float discriminant = b * b - 4 * a * c;
		if ( discriminant < 0 )
		{
			hitPoint = Vector2.negativeInfinity;
			return false;
		}
		// ray didn't totally miss sphere,
		// so there is a solution to
		// the equation.

		discriminant = Mathf.Sqrt( discriminant );

		// either solution may be on or off the ray so need to test both
		// t1 is always the smaller value, because BOTH discriminant and
		// a are nonnegative.
		float t1 = ( -b - discriminant ) / ( 2 * a );
		float t2 = ( -b + discriminant ) / ( 2 * a );

		// 3x HIT cases:
		//          -o->             --|-->  |            |  --|->
		// Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

		// 3x MISS cases:
		//       ->  o                     o ->              | -> |
		// FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

		if ( t1 >= 0 && t1 <= 1 )
		{
			// t1 is the intersection, and it's closer than t2
			// (since t1 uses -b - discriminant)
			// Impale, Poke
			hitPoint = rayStart + ( rayEnd - rayStart ) * t1;
			return true;
		}

		// here t1 didn't intersect so we are either started
		// inside the sphere or completely past it
		if ( t2 >= 0 && t2 <= 1 )
		{
			// ExitWound
			hitPoint = rayStart + ( rayEnd - rayStart ) * t2;
			return true;
		}

		// no intn: FallShort, Past, CompletelyInside
		hitPoint = circleCenter;
		return true;
	}

	public static bool RayCastAgainstObject( Transform ray, float length, float width, GameObject target, out Vector2 hit )
	{
		if ( ray == null || target == null )
		{
			hit = Vector2.zero;
			return false;
		}

		Vector2 rayStart = new( ray.position.x, ray.position.z );
		Vector2 rayEnd = rayStart + new Vector2( ray.transform.forward.x, ray.transform.forward.z ) * length;
		Vector2 circleCenter = new( target.transform.position.x, target.transform.position.z );
		var targetCollider = target.GetComponent<Collider>();
		if ( targetCollider is SphereCollider sphereCollider )
		{
			circleCenter.x += sphereCollider.center.x;
			circleCenter.y += sphereCollider.center.z;
			RayCastAgainst2DCircle( rayStart, rayEnd, circleCenter, sphereCollider.radius + width / 2.0f, out hit );
		}
		else if ( targetCollider is CapsuleCollider capsuleCollider )
		{
			circleCenter.x += capsuleCollider.center.x;
			circleCenter.y += capsuleCollider.center.z;
			float cylinderLength = Mathf.Max(0, capsuleCollider.height - capsuleCollider.radius * 2);
			if ( cylinderLength == 0 || capsuleCollider.direction == 1 )
			{
				RayCastAgainst2DCircle( rayStart, rayEnd, circleCenter, capsuleCollider.radius + width / 2.0f, out hit );
			}
			else
			{
				Vector2 delta = new Vector2( target.transform.forward.x, target.transform.forward.z ) * cylinderLength / 2.0f;
				Vector2 circleCenter1 = circleCenter + delta;
				RayCastAgainst2DCircle( rayStart, rayEnd, circleCenter1, capsuleCollider.radius + width / 2.0f, out var hit1 );
				Vector2 circleCenter2 = circleCenter - delta;
				RayCastAgainst2DCircle( rayStart, rayEnd, circleCenter2, capsuleCollider.radius + width / 2.0f, out var hit2 );
				if ( hit1 == Vector2.negativeInfinity && hit2 == Vector2.negativeInfinity )
				{
					hit = Vector2.negativeInfinity;
				}
				else if ( hit1 == Vector2.negativeInfinity )
					hit = hit2;
				else if ( hit2 == Vector2.negativeInfinity )
					hit = hit1;
				else if ( ( hit1 - rayStart ).sqrMagnitude < ( hit2 - rayStart ).sqrMagnitude )
					hit = hit1;
				else
					hit = hit2;
			}
		}
		else
			hit = Vector2.negativeInfinity;

		if ( hit == Vector2.negativeInfinity || Double.IsNaN( hit.x ) || Double.IsNaN( hit.y ) )
			return false;

		Vector2 collisionNormal = ( hit - circleCenter ).normalized;
		hit -= collisionNormal * width / 2.0f;
		return true;
	}
}
