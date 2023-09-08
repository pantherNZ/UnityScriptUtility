using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static partial class Extensions
{
    public static void Destroy( this GameObject gameObject, bool allowImmediate = false )
    {
        if( Application.isEditor && allowImmediate )
            UnityEngine.Object.DestroyImmediate( gameObject );
        else
            UnityEngine.Object.Destroy( gameObject );
    }

    public static void ToggleActive( this GameObject gameObject )
    {
        gameObject.SetActive( !gameObject.activeSelf );
    }

    public static void DestroyAll( this List<GameObject> objects )
    {
        foreach( var x in objects )
            if( x != null )
                x.Destroy();
        objects.Clear();
    }

    public static void DestroyObject( this MonoBehaviour component )
    {
        UnityEngine.Object.Destroy( component.gameObject );
    }

    public static void DestroyComponent( this MonoBehaviour component )
    {
        UnityEngine.Object.Destroy( component );
    }


    public static void Deconstruct<T1, T2>( this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value )
    {
        key = tuple.Key;
        value = tuple.Value;
    }

    public static bool IsVisible( this CanvasGroup group )
    {
        return group.alpha != 0.0f;
    }

    public static void ToggleVisibility( this CanvasGroup group )
    {
        group.SetVisibility( !group.IsVisible() );
    }

    public static void SetVisibility( this CanvasGroup group, bool visible )
    {
        group.alpha = visible ? 1.0f : 0.0f;
        group.blocksRaycasts = visible;
        group.interactable = visible;
    }

    // Deep clone
    public static T DeepCopy<T>( this T a )
    {
        using( MemoryStream stream = new MemoryStream() )
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize( stream, a );
            stream.Position = 0;
            return ( T )formatter.Deserialize( stream );
        }
    }

#if UNITY_EDITOR
    public static string GetDataPathAbsolute( this TextAsset textAsset )
    {
        return Application.dataPath.Substring( 0, Application.dataPath.Length - 6 ) + UnityEditor.AssetDatabase.GetAssetPath( textAsset );
    }

    public static string GetDataPathRelative( this TextAsset textAsset )
    {
        return UnityEditor.AssetDatabase.GetAssetPath( textAsset );
    }
#endif

    public static Vector2 Set( this Vector2 vec, float val ) { return vec.SetX( val ).SetY( val ); }
    public static Vector3 Set( this Vector3 vec, float val ) { return vec.SetX( val ).SetY( val ).SetZ( val ); }
    public static Vector4 Set( this Vector4 vec, float val ) { return vec.SetX( val ).SetY( val ).SetZ( val ).SetW( val ); }

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


    public static Color SetR( this Color col, float r ) { col.r = r; return col; }
    public static Color SetG( this Color col, float g ) { col.g = g; return col; }
    public static Color SetB( this Color col, float b ) { col.b = b; return col; }
    public static Color SetA( this Color col, float a ) { col.a = a; return col; }

    public static Color ToColour( this uint rgba )
    {
        return Utility.ColourFromHex( rgba );
    }

    public static Vector2 ToVector2( this Vector3 vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector2 ToVector2( this Vector4 vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector3 ToVector3( this Vector4 vec ) { return new Vector3( vec.x, vec.y, vec.z ); }
    public static Vector3 ToVector3( this Vector2 vec, float z = 0.0f ) { return new Vector3( vec.x, vec.y, z ); }
    public static Vector2 ToVector2( this Vector2Int vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector3 ToVector3( this Vector3Int vec ) { return new Vector3( vec.x, vec.y, vec.z ); }

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

    public static Vector2 RandomPosition( this Rect rect )
    {
        return new Vector2(
            rect.x + UnityEngine.Random.value * rect.width,
            rect.y + UnityEngine.Random.value * rect.height );
    }

    public static bool Overlaps( this RectTransform rectTrans1, RectTransform rectTrans2 )
    {
        Rect rect1 = new Rect( rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width * rectTrans1.localScale.x, rectTrans1.rect.height * rectTrans1.localScale.y );
        Rect rect2 = new Rect( rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width * rectTrans1.localScale.x, rectTrans2.rect.height * rectTrans1.localScale.y );

        return rect1.Overlaps( rect2 );
    }

    static public Rect GetWorldRect( this RectTransform rt )
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners( corners );
        Vector2 scaledSize = new Vector2( rt.lossyScale.x * rt.rect.size.x, rt.lossyScale.y * rt.rect.size.y );
        return new Rect( corners[0], scaledSize );
    }

    static public Rect ConvertToWorldRect( this RectTransform rt, Rect localRect )
    {
        var min = rt.localToWorldMatrix.MultiplyPoint( localRect.min );
        var max = rt.localToWorldMatrix.MultiplyPoint( localRect.max );
        return new Rect( ( max + min ) / 2.0f, max - min );
    }

    public static Rect GetSceenSpaceRect( this RectTransform rt )
    {
        Vector2 size = Vector2.Scale( rt.rect.size, rt.lossyScale );
        return new Rect( ( Vector2 )rt.position - ( size * 0.5f ), size );
    }

    public static int GetChildIndex( this Transform transform )
    {
        return transform.parent.GetIndexOfChild( transform );
    }

    public static int GetIndexOfChild( this Transform transform, Transform child )
    {
        int idx = 0;
        foreach( Transform c in transform )
        {
            if( c == child )
                return idx;
            idx++;
        }
        return -1;
    }

    public static void Match( this Transform transform, Transform other )
    {
        transform.position = other.position;
        transform.localScale = other.localScale;
        transform.rotation = other.rotation;
    }

    static public Vector2 TopLeft( this Rect rect )
    {
        return new Vector2( rect.xMin, rect.yMax );
    }

    static public Vector2 TopRight( this Rect rect )
    {
        return new Vector2( rect.xMax, rect.yMax );
    }

    static public Vector2 BottomLeft( this Rect rect )
    {
        return new Vector2( rect.xMin, rect.yMin );
    }

    static public Vector2 BottomRight( this Rect rect )
    {
        return new Vector2( rect.xMax, rect.yMin );
    }

    static public Rect ToRect( this Bounds bound )
    {
        return new Rect( bound.center - bound.extents, bound.size );
    }

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

    public static string ToSuperscriptString<T>( this T t ) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        return Utility.ToSuperscript( t.ToString() );
    }
    
    public static string ToSubscriptString< T >( this T t ) where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        return Utility.ToSubscript( t.ToString() );
    }

    public static long NextLong( this System.Random random, long min, long max )
    {
        if( max <= min )
            throw new ArgumentOutOfRangeException( "max", "max must be > min!" );

        ulong uRange = ( ulong )( max - min );

        //Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
        ulong ulongRand;
        do
        {
            byte[] buf = new byte[8];
            random.NextBytes( buf );
            ulongRand = ( ulong )BitConverter.ToInt64( buf, 0 );
        }
        while( ulongRand > ulong.MaxValue - ( ( ulong.MaxValue % uRange ) + 1 ) % uRange );

        return ( long )( ulongRand % uRange ) + min;
    }

    public static long NextLong( this System.Random random, long max )
    {
        return random.NextLong( 0, max );
    }

    public static long NextLong( this System.Random random )
    {
        return random.NextLong( long.MinValue, long.MaxValue );
    }

    public static bool StartsWithGet( this string str, string startsWith, out string secondPart )
    {
        var idx = str.IndexOf( startsWith );
        if( idx != -1 )
        {
            secondPart = str.Substring( idx + startsWith.Length );
            return true;
        }

        secondPart = string.Empty;
        return false;
    }

    public static ulong ToU64( this BitArray ba )
    {
        Debug.Assert( ba.Length <= 64 );
        var len = Math.Min( 64, ba.Count );
        ulong n = 0;
        for( int i = 0; i < len; i++ )
        {
            if( ba.Get( i ) )
                n |= 1UL << i;
        }
        return n;
    }

    // String colour utility
    public static string Bold( this string str ) { return $"<b>{str}</b>"; }
    public static string Italic( this string str ) { return $"<i>{str}</i>"; }
    public static string Size( this string str, int size ) { return $"<size={size}>{str}</size>"; }
    public static string Aqua( this string str ) { return Colour( str, Color.cyan ); }
    public static string Black( this string str ) { return Colour( str, Color.black ); }
    public static string Blue( this string str ) { return Colour( str, Color.blue ); }
    public static string Brown( this string str ) { return Colour( str, 0xa52a2aff ); }
    public static string Cyan( this string str ) { return Aqua( str ); }
    public static string DarkBlue( this string str ) { return Colour( str, 0x0000a0ff ); }
    public static string Fuchsia( this string str ) { return Magenta( str ); }
    public static string Green( this string str ) { return Colour( str, Color.green ); }
    public static string Gray( this string str ) { return Colour( str, Color.gray ); }
    public static string Grey( this string str ) { return Grey( str ); }
    public static string LightBlue( this string str ) { return Colour( str, 0xadd8e6ff ); }
    public static string Lime( this string str ) { return Colour( str, 0x00ff00ff ); }
    public static string Magenta( this string str ) { return Colour( str, Color.magenta ); }
    public static string Maroon( this string str ) { return Colour( str, 0x800000ff ); }
    public static string Navy( this string str ) { return Colour( str, 0x000080ff ); }
    public static string Olive( this string str ) { return Colour( str, 0x808000ff ); }
    public static string Orange( this string str ) { return Colour( str, 0xffa500ff ); }
    public static string Purple( this string str ) { return Colour( str, 0x800080ff ); }
    public static string Red( this string str ) { return Colour( str, Color.red ); }
    public static string Silver( this string str ) { return Colour( str, 0xc0c0c0ff ); }
    public static string Teal( this string str ) { return Colour( str, 0x008080ff ); }
    public static string White( this string str ) { return Colour( str, Color.white ); }
    public static string Yellow( this string str ) { return Colour( str, Color.yellow ); }

    public static string Colour( this string str, Color colour )
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA( colour )}>{str}</color>";
    }

    public static string Colour( this string str, uint rgba )
    {
        return Colour( str, Utility.ColourFromHex( rgba ) );
    }

    public static string ColourRGB( this string str, uint rgb )
    {
        return Colour( str, Utility.ColourFromHexRGB( rgb ) );
    }

    public static string Format( this string str, params object[] args )
    {
        return string.Format( str, args );
    }
}