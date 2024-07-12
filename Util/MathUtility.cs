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

    public static bool Overlaps( this RectTransform rectTrans1, RectTransform rectTrans2 )
    {
        Rect rect1 = new Rect( rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width * rectTrans1.localScale.x, rectTrans1.rect.height * rectTrans1.localScale.y );
        Rect rect2 = new Rect( rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width * rectTrans1.localScale.x, rectTrans2.rect.height * rectTrans1.localScale.y );

        return rect1.Overlaps( rect2 );
    }


    static public Vector2 TopLeft( this Rect rect ) { return new Vector2( rect.xMin, rect.yMax ); }
    static public Vector2 TopRight( this Rect rect ) { return new Vector2( rect.xMax, rect.yMax ); }
    static public Vector2 BottomLeft( this Rect rect ) { return new Vector2( rect.xMin, rect.yMin ); }
    static public Vector2 BottomRight( this Rect rect ) { return new Vector2( rect.xMax, rect.yMin ); }
    static public Rect ToRect( this Bounds bound ) { return new Rect( bound.center - bound.extents, bound.size ); }

    static public bool Contains( this Rect rect, Rect other )
    {
        return rect.Contains( other.TopLeft() )
             && rect.Contains( other.TopRight() )
             && rect.Contains( other.BottomLeft() )
             && rect.Contains( other.BottomRight() );
    }

    static public bool Contains( this Rect rect, Bounds other )
    {
        return rect.Contains( other.ToRect() );
    }

    static public Vector3 UnrotateVector( this Quaternion quat, Vector3 v )
    {
        return Matrix4x4.Rotate( quat ).transpose.MultiplyVector( v );
    }

    static public Vector3 RotateVector( this Quaternion quat, Vector3 v )
    {
        return Matrix4x4.Rotate( quat ).MultiplyVector( v );
    }

    static public Vector3 GetScaledAxis( this Matrix4x4 mat, EAxis InAxis )
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

    static public void GetScaledAxes( this Matrix4x4 mat, out Vector3 X, out Vector3 Y, out Vector3 Z )
    {
        X = new Vector3( mat[0, 0], mat[0, 1], mat[0, 2] );
        Y = new Vector3( mat[1, 0], mat[1, 1], mat[1, 2] );
        Z = new Vector3( mat[2, 0], mat[2, 1], mat[2, 2] );
    }

    static public Vector3 GetUnitAxis( this Matrix4x4 mat, EAxis InAxis )
    {
        return mat.GetScaledAxis( InAxis ).normalized;
    }

    static public void GetUnitAxes( this Matrix4x4 mat, Vector3 x, Vector3 y, Vector3 z )
    {
        mat.GetScaledAxes( out x, out y, out z );
        x.Normalize();
        y.Normalize();
        z.Normalize();
    }

    static public void SetAxis( this Matrix4x4 mat, int i, Vector3 axis )
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

    static public Vector3 Rotation( this Vector3 vec )
    {
        return new Vector3( Mathf.Atan2( vec.y, vec.x ), Mathf.Atan2( vec.z, vec.ToVector2().magnitude ), 0.0f );
    }

    static public void SetTransformData( this Transform transform, TransformData data )
    {
        transform.position = data.translation;
        transform.localScale = data.scale;
        transform.rotation = data.rotation;
    }

	static public TransformData GetTransformData( this Transform transform )
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

	public static float[] StandardNormalTable = new float[] {
		0.5f,
		0.50399f,
		0.50798f,
		0.51197f,
		0.51595f,
		0.51994f,
		0.52392f,
		0.5279f,
		0.53188f,
		0.53586f,
		0.53983f,
		0.5438f,
		0.54776f,
		0.55172f,
		0.55567f,
		0.55962f,
		0.5636f,
		0.56749f,
		0.57142f,
		0.57535f,
		0.57926f,
		0.58317f,
		0.58706f,
		0.59095f,
		0.59483f,
		0.59871f,
		0.60257f,
		0.60642f,
		0.61026f,
		0.61409f,
		0.61791f,
		0.62172f,
		0.62552f,
		0.6293f,
		0.63307f,
		0.63683f,
		0.64058f,
		0.64431f,
		0.64803f,
		0.65173f,
		0.65542f,
		0.6591f,
		0.66276f,
		0.6664f,
		0.67003f,
		0.67364f,
		0.67724f,
		0.68082f,
		0.68439f,
		0.68793f,
		0.69146f,
		0.69497f,
		0.69847f,
		0.70194f,
		0.7054f,
		0.70884f,
		0.71226f,
		0.71566f,
		0.71904f,
		0.7224f,
		0.72575f,
		0.72907f,
		0.73237f,
		0.73565f,
		0.73891f,
		0.74215f,
		0.74537f,
		0.74857f,
		0.75175f,
		0.7549f,
		0.75804f,
		0.76115f,
		0.76424f,
		0.7673f,
		0.77035f,
		0.77337f,
		0.77637f,
		0.77935f,
		0.7823f,
		0.78524f,
		0.78814f,
		0.79103f,
		0.79389f,
		0.79673f,
		0.79955f,
		0.80234f,
		0.80511f,
		0.80785f,
		0.81057f,
		0.81327f,
		0.81594f,
		0.81859f,
		0.82121f,
		0.82381f,
		0.82639f,
		0.82894f,
		0.83147f,
		0.83398f,
		0.83646f,
		0.83891f,
		0.84134f,
		0.84375f,
		0.84614f,
		0.84849f,
		0.85083f,
		0.85314f,
		0.85543f,
		0.85769f,
		0.85993f,
		0.86214f,
		0.86433f,
		0.8665f,
		0.86864f,
		0.87076f,
		0.87286f,
		0.87493f,
		0.87698f,
		0.879f,
		0.881f,
		0.88298f,
		0.88493f,
		0.88686f,
		0.88877f,
		0.89065f,
		0.89251f,
		0.89435f,
		0.89617f,
		0.89796f,
		0.89973f,
		0.90147f,
		0.9032f,
		0.9049f,
		0.90658f,
		0.90824f,
		0.90988f,
		0.91149f,
		0.91308f,
		0.91466f,
		0.91621f,
		0.91774f,
		0.91924f,
		0.92073f,
		0.9222f,
		0.92364f,
		0.92507f,
		0.92647f,
		0.92785f,
		0.92922f,
		0.93056f,
		0.93189f,
		0.93319f,
		0.93448f,
		0.93574f,
		0.93699f,
		0.93822f,
		0.93943f,
		0.94062f,
		0.94179f,
		0.94295f,
		0.94408f,
		0.9452f,
		0.9463f,
		0.94738f,
		0.94845f,
		0.9495f,
		0.95053f,
		0.95154f,
		0.95254f,
		0.95352f,
		0.95449f,
		0.95543f,
		0.95637f,
		0.95728f,
		0.95818f,
		0.95907f,
		0.95994f,
		0.9608f,
		0.96164f,
		0.96246f,
		0.96327f,
		0.96407f,
		0.96485f,
		0.96562f,
		0.96638f,
		0.96712f,
		0.96784f,
		0.96856f,
		0.96926f,
		0.96995f,
		0.97062f,
		0.97128f,
		0.97193f,
		0.97257f,
		0.9732f,
		0.97381f,
		0.97441f,
		0.975f,
		0.97558f,
		0.97615f,
		0.9767f,
		0.97725f,
		0.97778f,
		0.97831f,
		0.97882f,
		0.97932f,
		0.97982f,
		0.9803f,
		0.98077f,
		0.98124f,
		0.98169f,
		0.98214f,
		0.98257f,
		0.983f,
		0.98341f,
		0.98382f,
		0.98422f,
		0.98461f,
		0.985f,
		0.98537f,
		0.98574f,
		0.9861f,
		0.98645f,
		0.98679f,
		0.98713f,
		0.98745f,
		0.98778f,
		0.98809f,
		0.9884f,
		0.9887f,
		0.98899f,
		0.98928f,
		0.98956f,
		0.98983f,
		0.9901f,
		0.99036f,
		0.99061f,
		0.99086f,
		0.99111f,
		0.99134f,
		0.99158f,
		0.9918f,
		0.99202f,
		0.99224f,
		0.99245f,
		0.99266f,
		0.99286f,
		0.99305f,
		0.99324f,
		0.99343f,
		0.99361f,
		0.99379f,
		0.99396f,
		0.99413f,
		0.9943f,
		0.99446f,
		0.99461f,
		0.99477f,
		0.99492f,
		0.99506f,
		0.9952f,
		0.99534f,
		0.99547f,
		0.9956f,
		0.99573f,
		0.99585f,
		0.99598f,
		0.99609f,
		0.99621f,
		0.99632f,
		0.99643f,
		0.99653f,
		0.99664f,
		0.99674f,
		0.99683f,
		0.99693f,
		0.99702f,
		0.99711f,
		0.9972f,
		0.99728f,
		0.99736f,
		0.99744f,
		0.99752f,
		0.9976f,
		0.99767f,
		0.99774f,
		0.99781f,
		0.99788f,
		0.99795f,
		0.99801f,
		0.99807f,
		0.99813f,
		0.99819f,
		0.99825f,
		0.99831f,
		0.99836f,
		0.99841f,
		0.99846f,
		0.99851f,
		0.99856f,
		0.99861f,
		0.99865f,
		0.99869f,
		0.99874f,
		0.99878f,
		0.99882f,
		0.99886f,
		0.99889f,
		0.99893f,
		0.99896f,
		0.999f,
		0.99903f,
		0.99906f,
		0.9991f,
		0.99913f,
		0.99916f,
		0.99918f,
		0.99921f,
		0.99924f,
		0.99926f,
		0.99929f,
		0.99931f,
		0.99934f,
		0.99936f,
		0.99938f,
		0.9994f,
		0.99942f,
		0.99944f,
		0.99946f,
		0.99948f,
		0.9995f,
		0.99952f,
		0.99953f,
		0.99955f,
		0.99957f,
		0.99958f,
		0.9996f,
		0.99961f,
		0.99962f,
		0.99964f,
		0.99965f,
		0.99966f,
		0.99968f,
		0.99969f,
		0.9997f,
		0.99971f,
		0.99972f,
		0.99973f,
		0.99974f,
		0.99975f,
		0.99976f,
		0.99977f,
		0.99978f,
		0.99978f,
		0.99979f,
		0.9998f,
		0.99981f,
		0.99981f,
		0.99982f,
		0.99983f,
		0.99983f,
		0.99984f,
		0.99985f,
		0.99985f,
		0.99986f,
		0.99986f,
		0.99987f,
		0.99987f,
		0.99988f,
		0.99988f,
		0.99989f,
		0.99989f,
		0.9999f,
		0.9999f,
		0.9999f,
		0.99991f,
		0.99991f,
		0.99992f,
		0.99992f,
		0.99992f,
		0.99992f,
		0.99993f,
		0.99993f,
		0.99993f,
		0.99994f,
		0.99994f,
		0.99994f,
		0.99994f,
		0.99995f,
		0.99995f,
		0.99995f,
		0.99995f,
		0.99995f,
		0.99996f,
		0.99996f,
		0.99996f,
		0.99996f,
		0.99996f,
		0.99996f,
		0.99997f,
		0.99997f,
		0.99997f,
		0.99997f,
		0.99997f,
		0.99997f,
		0.99997f,
		0.99997f,
		0.99998f,
		0.99998f,
		0.99998f,
		0.99998f };

	static float CumulativeNormalDistribution( float x )
	{
		if ( x > 4.0f )
			return 1.0f;
		else if ( x < -4.0f )
			return 0.0f;
		else if ( x >= 0.0f )
			return StandardNormalTable[( int )( x * 100.0f )];
		else
			return 1.0f - StandardNormalTable[( int )( -x * 100.0f )];
	}

	static public bool RayCastAgainst2DCircle( Vector2 rayStart, Vector2 rayEnd, Vector2 circleCenter, float circleRadius, out Vector2 hitPoint )
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

	static public bool RayCastAgainstObject( Transform ray, float length, float width, GameObject target, out Vector2 hit )
	{
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

		if ( hit == Vector2.negativeInfinity )
			return false;

		Vector2 collisionNormal = ( hit - circleCenter ).normalized;
		hit -= collisionNormal * width / 2.0f;
		return true;
	}
}
