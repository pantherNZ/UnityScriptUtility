using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static partial class Utility
{
    public static uint SetBits( uint word, uint value, int pos, int size )
    {
        uint mask = ( ( ( ( uint )1 ) << size ) - 1 ) << pos;
        word &= ~mask;
        word |= ( value << pos ) & mask;
        return word;
    }

    public static uint ReadBits( uint word, int pos, int size )
    {
        uint mask = ( ( ( ( uint )1 ) << size ) - 1 ) << pos;
        return ( word & mask ) >> pos;
    }

    public class DestroySelf : MonoBehaviour
    {
        public void DestroyMe()
        {
            Destroy( gameObject );
        }
    }

    public static IEnumerable<T> GetEnumValues<T>()
    {
        return Enum.GetValues( typeof( T ) ).Cast<T>();
    }

    public static int GetNumEnumValues<T>()
    {
        return GetEnumValues<T>().Count();
    }

    public static void Swap<T>( ref T a, ref T b )
    {
        (b, a) = (a, b);
    }

#if UNITY_EDITOR
    public static string GetResourcePath( UnityEngine.Object @object )
    {
        var resource = AssetDatabase.GetAssetPath( @object );
        var startIdx = resource.IndexOf( "Resources/" ) + 10;
        resource = resource[startIdx..resource.LastIndexOf( '.' )];
        return resource;
    }
#endif

	public static void GetResourcePaths( string path, ref List<string> pathsOut, bool recursive = true )
	{
		if ( path.EndsWith( '/' ) )
			path = path[0..^1];

		var fullPath = Application.dataPath + "/Resources/" + path;
		DirectoryInfo dirInfo = new DirectoryInfo( fullPath );

		foreach ( var file in dirInfo.GetFiles() )
			if ( file.Name.EndsWith( ".asset" ) )
				pathsOut.Add( path + "/" + file.Name[0..^6] );

		if( recursive )
			foreach ( var dir in dirInfo.GetDirectories() )
				GetResourcePaths( path + "/" + dir.Name, ref pathsOut );
	}

	public static Sprite CreateSprite( Texture2D texture )
    {
        if( texture == null )
        {
            Debug.LogError( "Utility::CreateSprite texture was null" );
            return null;
        }
        return Sprite.Create( texture, new Rect( 0.0f, 0.0f, texture.width, texture.height ), new Vector2( 0.5f, 0.5f ) );
    }

    public static void DrawCircle( Vector3 position, float diameter, float lineWidth = 0.1f, Color? colour = null, float duration = 5.0f, int segments = 20 )
	{
		DrawCircleInternal( position, diameter, lineWidth, colour, duration, segments, EAxis.Y );
	}

	static void DrawCircleInternal( Vector3 position, float diameter, float lineWidth, Color? colour, float duration, int segments, EAxis axis )
	{
		var newObj = new GameObject();
		newObj.transform.position = position;
		colour ??= new Color( 1.0f, 1.0f, 1.0f, 1.0f );

		var line = newObj.AddComponent<LineRenderer>();
		line.useWorldSpace = false;
		line.startWidth = lineWidth;
		line.endWidth = lineWidth;
		line.startColor = line.endColor = colour.Value;
		var points = GetCirclePoints( diameter, segments, axis );
		line.positionCount = points.Length;
		line.SetPositions( points );
		Material whiteDiffuseMat = new Material( Shader.Find( "Unlit/Texture" ) );
		line.material = whiteDiffuseMat;
		FunctionTimer.CreateTimer( duration, () =>
		{
			whiteDiffuseMat.Destroy();
			newObj.Destroy();
		} );
	}

	static Vector3[] GetCirclePoints( float diameter, int segments, EAxis axis )
	{
        var pointCount = segments + 2;
        var points = new Vector3[pointCount];

        for( int i = 0; i < pointCount; i++ )
        {
            var rad = Mathf.Deg2Rad * ( i * 360.0f / segments );
			points[i] = axis switch
			{
				EAxis.X => new Vector3( Mathf.Sin( rad ) * diameter / 2.0f, Mathf.Cos( rad ) * diameter / 2.0f, 0.0f ),
				EAxis.Y => new Vector3( Mathf.Sin( rad ) * diameter / 2.0f, 0.0f, Mathf.Cos( rad ) * diameter / 2.0f ),
				EAxis.Z => new Vector3( 0.0f, Mathf.Sin( rad ) * diameter / 2.0f, Mathf.Cos( rad ) * diameter / 2.0f ),
				_ => throw new NotImplementedException(),
			};
        }

		return points;
    }

	public static void DrawSphere( Vector3 position, float diameter, float lineWidth = 0.1f, Color? colour = null, float duration = 5.0f, int segments = 20 )
	{
		DrawCircleInternal( position, diameter, lineWidth, colour, duration, segments, EAxis.X );
		DrawCircleInternal( position, diameter, lineWidth, colour, duration, segments, EAxis.Y );
		DrawCircleInternal( position, diameter, lineWidth, colour, duration, segments, EAxis.Z );
	}

	public static void DrawRect( Rect rect, Color? colour = null, float duration = 0.01f )
    {
        colour ??= new Color( 1.0f, 1.0f, 1.0f, 1.0f );
        Debug.DrawLine( rect.TopLeft(), rect.TopRight(), colour.Value, duration );
        Debug.DrawLine( rect.TopRight(), rect.BottomRight(), colour.Value, duration );
        Debug.DrawLine( rect.BottomRight(), rect.BottomLeft(), colour.Value, duration );
        Debug.DrawLine( rect.BottomLeft(), rect.TopLeft(), colour.Value, duration );
    }

	public static void DrawBox( Vector3 pos, Quaternion? rot = null, Vector3? scale = null, Color? colour = null, float duration = 0.01f )
	{
		rot ??= Quaternion.identity;
		colour ??= new Color( 1.0f, 1.0f, 1.0f, 1.0f );
		scale ??= Vector3.one;

		Matrix4x4 m = new Matrix4x4();
		m.SetTRS( pos, rot.Value, scale.Value );

		var point1 = m.MultiplyPoint( new Vector3( -0.5f, -0.5f, 0.5f ) );
		var point2 = m.MultiplyPoint( new Vector3( 0.5f, -0.5f, 0.5f ) );
		var point3 = m.MultiplyPoint( new Vector3( 0.5f, -0.5f, -0.5f ) );
		var point4 = m.MultiplyPoint( new Vector3( -0.5f, -0.5f, -0.5f ) );

		var point5 = m.MultiplyPoint( new Vector3( -0.5f, 0.5f, 0.5f ) );
		var point6 = m.MultiplyPoint( new Vector3( 0.5f, 0.5f, 0.5f ) );
		var point7 = m.MultiplyPoint( new Vector3( 0.5f, 0.5f, -0.5f ) );
		var point8 = m.MultiplyPoint( new Vector3( -0.5f, 0.5f, -0.5f ) );

		Debug.DrawLine( point1, point2, colour.Value, duration );
		Debug.DrawLine( point2, point3, colour.Value, duration );
		Debug.DrawLine( point3, point4, colour.Value, duration );
		Debug.DrawLine( point4, point1, colour.Value, duration );

		Debug.DrawLine( point5, point6, colour.Value, duration );
		Debug.DrawLine( point6, point7, colour.Value, duration );
		Debug.DrawLine( point7, point8, colour.Value, duration );
		Debug.DrawLine( point8, point5, colour.Value, duration );

		Debug.DrawLine( point1, point5, colour.Value, duration );
		Debug.DrawLine( point2, point6, colour.Value, duration );
		Debug.DrawLine( point3, point7, colour.Value, duration );
		Debug.DrawLine( point4, point8, colour.Value, duration );
	}

	public static GameObject CreateSprite( string path, Vector3 pos, Vector2 scale, Quaternion? rotation = null, string layer = "Default", int order = 0 )
    {
        var sprite = new GameObject();
        sprite.transform.SetPositionAndRotation( pos, rotation ?? Quaternion.identity );
        var spriteRenderer = sprite.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>( path );
        spriteRenderer.sortingOrder = order;
        sprite.transform.localScale = scale.ToVector3( 1.0f );
        sprite.layer = LayerMask.NameToLayer( layer );
        return sprite;
    }

    public static GameObject CreateWorldObjectFromScreenSpaceRect( Rect rect )
    {
        var obj = new GameObject();
        obj.transform.position = rect.position;
        obj.transform.localScale = rect.size;
        return obj;
    }

    public static Vector2 GetMouseOrTouchPos()
    {
        return Input.touchCount > 0 ? Input.GetTouch( 0 ).position : Input.mousePosition.ToVector2();
    }

    public static bool IsMouseDownOrTouchStart()
    {
        return Input.GetMouseButtonDown( 0 ) || ( Input.touches.Length > 0 && Input.GetTouch( 0 ).phase == TouchPhase.Began );
    }

    public static bool IsMouseUpOrTouchEnd()
    {
        return Input.GetMouseButtonUp( 0 ) || ( Input.touches.Length > 0 && Input.GetTouch( 0 ).phase == TouchPhase.Ended );
    }

    public static bool IsMouseOrTouchHeld()
    {
        return Input.GetMouseButton( 0 ) || ( Input.touches.Length > 0 && 
            ( Input.GetTouch( 0 ).phase == TouchPhase.Moved || Input.GetTouch( 0 ).phase == TouchPhase.Stationary ) );
    }

    public static bool IsBackButtonDown()
    {
        if( Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor )
            return Input.GetKeyDown( KeyCode.Space );
        return ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer ) && Input.GetKeyDown( KeyCode.Escape );
    }

    public static bool IsPointerOverGameObject( GameObject gameObject )
    {
        return ForEachObjectOverPointer().Any( x => x.gameObject == gameObject );
    }

    public static bool IsPointerOverObjectWithComponent< T >() where T : MonoBehaviour
    {
        return ForEachObjectOverPointer().Any( x => x.GetComponent< T >() != null );
    }

    public static List<GameObject> GetObjectsOverPointer()
    {
        var eventData = new UnityEngine.EventSystems.PointerEventData( UnityEngine.EventSystems.EventSystem.current )
        {
            position = GetMouseOrTouchPos()
        };
        var raycastResults = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll( eventData, raycastResults );
        return raycastResults.Select( x => x.gameObject ).ToList();
    }

    public static IEnumerable<GameObject> ForEachObjectOverPointer()
    {
        foreach( var result in GetObjectsOverPointer() )
            yield return result;
    }

    public static int GetTextWidth( Text text )
    {
        int totalLength = 0;

        Font font = text.font;
        char[] arr = text.text.ToCharArray();

        foreach( char c in arr )
        {
            font.RequestCharactersInTexture( c.ToString(), text.fontSize, text.fontStyle );
            font.GetCharacterInfo( c, out var characterInfo, text.fontSize );
            totalLength += characterInfo.advance;
        }

        return totalLength;
    }

    public static UInt32 GetHashSHA256( string inputString )
    {
        using HashAlgorithm algorithm = SHA256.Create();
        var bytes = algorithm.ComputeHash( System.Text.Encoding.UTF8.GetBytes( inputString ) );
        return BitConverter.ToUInt32( bytes, 0 );
    }

    public static void Destroy( this UnityEngine.Object gameObject )
    {
        if( Application.isEditor && !Application.isPlaying )
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
        Destroy( component.gameObject );
    }

    public static void DestroyComponent( this MonoBehaviour component )
    {
        Destroy( component );
    }

    public static IEnumerable<Pair<int, Transform>> Enumerate( this Transform collection, int startIndex = 0 )
    {
        foreach( Transform item in collection )
        {
            yield return new Pair<int, Transform>( startIndex++, item );
        }
    }

    public static void DestroyChildren( this Transform transform )
    {
        while( transform.childCount > 0 )
        {
            var child = transform.GetChild( 0 );
            child.parent = null;
            child.gameObject.Destroy();
        }
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

} // Utility namespace end

[Serializable]
public class TransformData
{
    public Vector3 translation;
    public Quaternion rotation;
    public Vector3 scale;
}

public enum EAxis
{
    X,
    Y,
    Z,
    None,
}
