using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static partial class Extensions
{
    public static void Destroy( this GameObject gameObject )
    {
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

    public static void Resize<T>( this List<T> list, int size, T value = default )
    {
        int cur = list.Count;
        if( size < cur )
            list.RemoveRange( size, cur - size );
        else if( size > cur )
            list.AddRange( Enumerable.Repeat( value, size - cur ) );
    }

    public static void Deconstruct<T1, T2>( this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value )
    {
        key = tuple.Key;
        value = tuple.Value;
    }

    public static TValue GetOrAdd<TKey, TValue>( this IDictionary<TKey, TValue> dict, TKey key )
    where TValue : new()
    {
        if( !dict.TryGetValue( key, out TValue val ) )
        {
            val = new TValue();
            dict.Add( key, val );
        }

        return val;
    }

    public static Pair<U, V> FindPairFirst<U, V>( this List<Pair<U, V>> list, U item )
    {
        return list.Find( x => x.First.Equals( item ) );
    }

    public static Pair<U, V> FindPairSecond<U, V>( this List<Pair<U, V>> list, U item )
    {
        return list.Find( x => x.Second.Equals( item ) );
    }

    public static void RemoveBySwap<T>( this List<T> list, int index )
    {
        list[index] = list[list.Count - 1];
        list.RemoveAt( list.Count - 1 );
    }

    public static bool Remove<T>( this List<T> list, Predicate<T> match )
    {
        foreach( var ( idx, x ) in Enumerate( list ) )
        {
            if( match( x ) )
            {
                list.RemoveAt( idx );
                return true;
            }
        }

        return false;
    }

    public static void RemovePairFirst<U, V>( this List<Pair<U, V>> list, U item )
    {
        RemoveBySwap( list, x => x.First.Equals( item ) );
    }

    public static void RemovePairSecond<U, V>( this List<Pair<U, V>> list, U item )
    {
        RemoveBySwap( list, x => x.Second.Equals( item ) );
    }

    public static bool RemoveBySwap<T>( this List<T> list, Func<T, bool> predicate )
    {
        if( list.IsEmpty() )
            return false;

        var end = list.Count;

        for( int i = 0; i < end; ++i )
        {
            if( predicate( list[i] ) )
            {
                if( i != end - 1 )
                    list[i] = list[end - 1];

                if( end > 0 )
                    end--;
            }
        }

        bool removed = end < list.Count;
        list.Resize( end );
        return removed;
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

    public static List<T> Rotate<T>( this List<T> list, int offset )
    {
        if( offset == 0 )
            return list;
        offset = list.Count - Utility.Mod( offset, list.Count );
        offset = ( offset < 0 ? list.Count + offset : offset );
        return list.Skip( offset ).Concat( list.Take( offset ) ).ToList();
    }

    public static T PopFront<T>( this List<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use PopFront on an empty list!" );

        var last = list[0];
        list.RemoveAt( 0 );
        return last;
    }

    public static T PopBack<T>( this List<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use PopBack on an empty list!" );

        var last = list[list.Count - 1];
        list.RemoveAt( list.Count - 1 );
        return last;
    }

    public static T Front<T>( this List<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use Front on an empty list!" );

        var first = list[0];
        return first;
    }

    public static T Back<T>( this List<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use Back on an empty list!" );

        var last = list[list.Count - 1];
        return last;
    }

    public static bool IsEmpty<T>( this List<T> list )
    {
        return list.Count == 0;
    }

    public static bool IsEmpty<T>( this HashSet<T> list )
    {
        return list.Count == 0;
    }

    public static T RandomItem<T>( this List<T> list, T defaultValue = default )
    {
        if( list.IsEmpty() )
            return defaultValue;
        return list[UnityEngine.Random.Range( 0, list.Count )];
    }

    public static List<T> RandomShuffle<T>( this List<T> list )
    {
        for( int i = 0; i < list.Count; i++ )
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range( i, list.Count );
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    public static T RemoveAndGet<T>( this List<T> list, Predicate<T> match )
    {
        var idx = list.FindIndex( match );

        if( idx == -1 )
            return default;

        var found = list[idx];
        list.RemoveBySwap( idx );
        return found;
    }

    public static bool Any( this BitArray bitArray )
    {
        for( int i = 0; i < bitArray.Length; ++i )
            if( bitArray.Get( i ) )
                return true;
        return false;
    }

    public static int CountBits( this BitArray bitArray )
    {
        int count = 0;
        for( int i = 0; i < bitArray.Length; ++i )
            if( bitArray.Get( i ) )
                count++;
        return count;
    }

    public static BitArray ToBitArray( this Int64 numeral )
    {
        var array = new int[2];
        array[0] = ( int )numeral;
        array[1] = ( int )( numeral >> 32 );
        return new BitArray( array );
    }

    public static Int64 ToNumeral( this BitArray binary )
    {
        if( binary == null )
            throw new ArgumentNullException( "binary" );
        if( binary.Length > 64 )
            throw new ArgumentException( "must be at most 64 bits long" );

        var array = new int[2];
        binary.CopyTo( array, 0 );
        Int64 output = ( Int64 )array[0];
        output |= ( ( Int64 )array[1] ) << 32;
        return output;
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

    public static Vector2 SetX( this Vector2 vec, float x ) { vec.x = x; return vec; }
    public static Vector2 SetY( this Vector2 vec, float y ) { vec.y = y; return vec; }
    public static Vector3 SetX( this Vector3 vec, float x ) { vec.x = x; return vec; }
    public static Vector3 SetY( this Vector3 vec, float y ) { vec.y = y; return vec; }
    public static Vector3 SetZ( this Vector3 vec, float z ) { vec.z = z; return vec; }
    public static Vector4 SetX( this Vector4 vec, float x ) { vec.x = x; return vec; }
    public static Vector4 SetY( this Vector4 vec, float y ) { vec.y = y; return vec; }
    public static Vector4 SetZ( this Vector4 vec, float z ) { vec.z = z; return vec; }
    public static Vector4 SetW( this Vector4 vec, float w ) { vec.w = w; return vec; }
    public static Color SetR( this Color col, float r ) { col.r = r; return col; }
    public static Color SetG( this Color col, float g ) { col.g = g; return col; }
    public static Color SetB( this Color col, float b ) { col.b = b; return col; }
    public static Color SetA( this Color col, float a ) { col.a = a; return col; }

    public static Color ToColour( this int rgba )
    {
        return Utility.ColourFromHex( rgba );
    }

    public static Vector2 ToVector2( this Vector3 vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector2 ToVector2( this Vector4 vec ) { return new Vector2( vec.x, vec.y ); }
    public static Vector3 ToVector3( this Vector4 vec ) { return new Vector3( vec.x, vec.y, vec.z ); }
    public static Vector3 ToVector3( this Vector2 vec, float z = 0.0f ) { return new Vector3( vec.x, vec.y, z ); }
    public static Vector2 ToVector2( this Vector2Int vec ) { return new Vector2( vec.x, vec.y ); }

    public static Vector2Int ToVector2Int( this Vector2 vec, bool round = false )
    {
        if( round )
            return new Vector2Int( Mathf.RoundToInt( vec.x ), Mathf.RoundToInt( vec.y ) );
        return new Vector2Int( Mathf.FloorToInt( vec.x ), Mathf.FloorToInt( vec.y ) );
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

    public static IEnumerable<Tuple<int, T>> Enumerate<T>( this IEnumerable<T> collection )
    {
        int counter = 0;
        foreach( var item in collection )
        {
            yield return new Tuple<int, T>( counter, item );
            counter++;
        }
    }

    static public Rect GetWorldRect( this RectTransform rt )
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners( corners );
        Vector2 scaledSize = new Vector2( rt.lossyScale.x * rt.rect.size.x, rt.lossyScale.y * rt.rect.size.y );
        return new Rect( ( corners[1] + corners[3] ) / 2.0f, scaledSize );
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
	    switch (InAxis )
	    {
	    case EAxis.X:
		    return new Vector3( mat[0,0], mat[0,1], mat[0,2]);

	    case EAxis.Y:
		    return new Vector3( mat[1,0], mat[1,1], mat[1,2]);

            case EAxis.Z:
		    return new Vector3( mat[2,0], mat[2,1], mat[2,2]);

	    default:
            Debug.LogError( "GetScaledAxis: Invalid axis" );
            return Vector3.zero;
	    }
    }

    static public void GetScaledAxes( this Matrix4x4 mat, out Vector3 X, out Vector3 Y, out Vector3 Z)
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
        mat[i,0] = axis.x;
        mat[i,1] = axis.y;
        mat[i,2] = axis.z;
    }

    static public void SetOrigin( this Matrix4x4 mat, Vector3 newOrigin )
    {
        mat[3,0] = newOrigin.x;
        mat[3,1] = newOrigin.y;
        mat[3,2] = newOrigin.z;
    }

    static public void SetAxes( this Matrix4x4 mat, Vector3? axis0 = null, Vector3? axis1 = null, Vector3? axis2 = null, Vector3? origin = null)
    {
        if( axis0 != null )
        {
            mat[0,0] = axis0.Value.x;
            mat[0,1] = axis0.Value.y;
            mat[0,2] = axis0.Value.z;
        }
        if( axis1 != null )
        {
            mat[1,0] = axis1.Value.x;
            mat[1,1] = axis1.Value.y;
            mat[1,2] = axis1.Value.z;
        }
        if( axis2 != null ) 
        {
            mat[2,0] = axis2.Value.x;
            mat[2,1] = axis2.Value.y;
            mat[2,2] = axis2.Value.z;
        }
        if( origin != null )
        {
            mat[3,0] = origin.Value.x;
            mat[3,1] = origin.Value.y;
            mat[3,2] = origin.Value.z;
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

    public static float SafeDivide( this float v, float denominator )
    {
        return denominator == 0.0f ? 0.0f : v / denominator;
    }

    public static int SafeDivide( this int v, int denominator )
    {
        return denominator == 0 ? 0 : v / denominator;
    }
}