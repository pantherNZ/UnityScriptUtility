using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;

public static partial class Utility
{
    public class DestroySelf : MonoBehaviour
    {
        public void DestroyMe()
        {
            Destroy( gameObject );
        }
    }

    // Helper function to get Modulus (not remainder which is what % gives)
    public static int Mod( int a, int b )
    {
        return ( ( a %= b ) < 0 ) ? a + b : a;
    }

    public static float Lerp( float a, float b, float interp ) { return a + ( b - a ) * interp; }
    public static int Lerp( int a, int b, float interp ) { return ( int )( a + ( b - a ) * interp ); }
    public static Vector2 Lerp( Vector2 a, Vector2 b, float interp ) {  return a + ( b - a ) * interp; }
    public static Vector3 Lerp( Vector3 a, Vector3 b, float interp ) {  return a + ( b - a ) * interp; }
    public static Vector4 Lerp( Vector4 a, Vector4 b, float interp ) {  return a + ( b - a ) * interp; }

    public static Vector2 Vector2FromAngle( float angleDegrees )
    {
        return new Vector2( Mathf.Cos( angleDegrees * Mathf.Deg2Rad ), Mathf.Sin( angleDegrees * Mathf.Deg2Rad ) );
    }

    public static IEnumerable<T> GetEnumValues<T>()
    {
        return Enum.GetValues( typeof( T ) ).Cast<T>();
    }

    public static int GetNumEnumValues<T>()
    {
        return GetEnumValues<T>().Count();
    }

    public static T ParseEnum<T>( string value ) where T : struct
    {
        return ( T )Enum.Parse( typeof( T ), value );
    }

    public static bool TryParseEnum<T>( string value, out T result ) where T : struct
    {
        return Enum.TryParse( value, out result );
    }

    // Parse a float, return default if failed
    public static float ParseFloat( string text, float defaultValue )
    {
        if( float.TryParse( text, out float f ) )
            return f;
        return defaultValue;
    }

    // Parse a int, return default if failed
    public static int ParseInt( string text, int defaultValue )
    {
        if( int.TryParse( text, out int i ) )
            return i;
        return defaultValue;
    }

    public static float Distance( GameObject a, GameObject b ) { return Distance( a.transform, b.transform ); }
    public static float Distance( GameObject a, Transform b ) { return Distance( a.transform, b ); }
    public static float Distance( Transform a, GameObject b ) { return Distance( a, b.transform ); }
    public static float Distance( Transform a, Transform b ) { return Mathf.Sqrt( DistanceSq( a, b ) ); }
    public static float Distance( Vector3 a, Vector3 b ) { return Mathf.Sqrt( DistanceSq( a, b ) ); }
    public static float DistanceSq( GameObject a, GameObject b ) { return DistanceSq( a.transform, b.transform ); }
    public static float DistanceSq( GameObject a, Transform b ) { return DistanceSq( a.transform, b ); }
    public static float DistanceSq( Transform a, GameObject b ) { return DistanceSq( a, b.transform ); }
    public static float DistanceSq( Transform a, Transform b ) { return DistanceSq( a.position, b.position ); }
    public static float DistanceSq( Vector3 a, Vector3 b ) { return ( a - b ).sqrMagnitude; }

#if UNITY_EDITOR
    public static string GetResourcePath( UnityEngine.Object @object )
    {
        var resource = AssetDatabase.GetAssetPath( @object );
        var startIdx = resource.IndexOf( "Resources/" ) + 10;
        resource = resource.Substring( startIdx, resource.LastIndexOf( '.' ) - startIdx  );
        return resource;
    }
#endif

    public static Sprite CreateSprite( Texture2D texture )
    {
        if( texture == null )
            return null;

        return Sprite.Create( texture, new Rect( 0.0f, 0.0f, texture.width, texture.height ), new Vector2( 0.5f, 0.5f ) );
    }

    public static Color GetRainbowColour( float value )
    {
        float inc = 6.0f;
        float x = value * inc;
        float r = 0.0f;
        float g = 0.0f;
        float b = 0.0f;

        if( ( 0 <= x && x <= 1 ) || ( 5 <= x && x <= 6 ) ) r = 1.0f;
        else if( 4 <= x && x <= 5 ) r = x - 4;
        else if( 1 <= x && x <= 2 ) r = 1.0f - ( x - 1 );
        if( 1 <= x && x <= 3 ) g = 1.0f;
        else if( 0 <= x && x <= 1 ) g = x - 0;
        else if( 3 <= x && x <= 4 ) g = 1.0f - ( x - 3 );
        if( 3 <= x && x <= 5 ) b = 1.0f;
        else if( 2 <= x && x <= 3 ) b = x - 2;
        else if( 5 <= x && x <= 6 ) b = 1.0f - ( x - 5 );

        return new Color( r, g, b, 1.0f );
    }

    public static Color InterpolateColour( Color a, Color b, float t, Func< float, float, float, float > interpolator )
    {
        Color.RGBToHSV( a, out float h1, out float s1, out float v1 );
        Color.RGBToHSV( b, out float h2, out float s2, out float v2 );
        float h = interpolator( h1, h2, t );
        float s = interpolator( s1, s2, t );
        float v = interpolator( v1, v2, t );
        var colour = Color.HSVToRGB( h, s, v );
        colour.a = interpolator( a.a, b.a, t );
        return colour;
    }

    public static Color ColourFromHex( int r, int g, int b, int a = 255 )
    {
        return new Color(
              r / 255.0f
            , g / 255.0F
            , b / 255.0f
            , a / 255.0f );
    }

    public static Color ColourFromHex( int rgba )
    {
        return new Color(
              ( ( rgba & 0xff000000 ) >> 0x18 ) / 255.0f
            , ( ( rgba & 0xff0000 ) >> 0x10 ) / 255.0F
            , ( ( rgba & 0xff00 ) >> 0x08 ) / 255.0f
            , ( rgba & 0xff ) / 255.0f );
    }

    public static void DrawCircle( Vector3 position, float diameter, float lineWidth, Color? colour = null )
    {
        colour ??= new Color( 1.0f, 1.0f, 1.0f, 1.0f );
        var newObj = new GameObject();
        newObj.transform.position = position;

        var segments = 20;
        var line = newObj.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;
        line.startColor = line.endColor = colour.Value;

        var pointCount = segments + 1;
        var points = new Vector3[pointCount];

        for( int i = 0; i < pointCount; i++ )
        {
            var rad = Mathf.Deg2Rad * ( i * 360.0f / segments );
            points[i] = new Vector3( Mathf.Sin( rad ) * diameter / 2.0f, Mathf.Cos( rad ) * diameter / 2.0f, -0.1f );
        }

        line.SetPositions( points );

        FunctionTimer.CreateTimer( 5.0f, () => newObj.Destroy() );
    }

    public static void DrawRect( Rect rect, Color? colour = null )
    {
        colour ??= new Color( 1.0f, 1.0f, 1.0f, 1.0f );
        Debug.DrawLine( rect.TopLeft(), rect.TopRight(), colour.Value );
        Debug.DrawLine( rect.TopRight(), rect.BottomRight(), colour.Value );
        Debug.DrawLine( rect.BottomRight(), rect.BottomLeft(), colour.Value );
        Debug.DrawLine( rect.BottomLeft(), rect.TopLeft(), colour.Value );
    }

    public class FunctionComponent : MonoBehaviour
    {
        public static void Create( GameObject obj, Action action )
        {
            var cmp = obj.AddComponent<FunctionComponent>();
            cmp.SetFunction( action );
        }

        public static void Create( GameObject obj, Func<bool> action )
        {
            var cmp = obj.AddComponent<FunctionComponent>();
            cmp.SetFunction( action );
        }

        public void SetFunction( Action action )
        {
            this.action = action;
        }

        public void SetFunction( Func<bool> action )
        {
            actionWithResult = action;
        }

        Action action;
        Func<bool> actionWithResult;

        void Update()
        {
            action?.Invoke();
            if( actionWithResult?.Invoke() ?? false )
                Destroy( this );
        }
    }

    public static GameObject CreateSprite( string path, Vector3 pos, Vector2 scale, Quaternion? rotation = null, string layer = "Default", int order = 0 )
    {
        var sprite = new GameObject();
        sprite.transform.position = pos;
        sprite.transform.rotation = rotation ?? Quaternion.identity;
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

    public static IEnumerable<Pair<int, T>> Enumerate<T>( this IEnumerable<T> collection, int startIndex = 0 )
    {
        foreach( var item in collection ) 
        { 
            yield return new Pair<int, T>( startIndex++, item ); 
        }
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

    public static IEnumerable<GameObject> ForEachObjectOverPointer()
    {
        var eventData = new UnityEngine.EventSystems.PointerEventData( UnityEngine.EventSystems.EventSystem.current )
        {
            position = GetMouseOrTouchPos()
        };
        var raycastResults = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll( eventData, raycastResults );

        foreach( var result in raycastResults )
            yield return result.gameObject;
    }

    public static int GetTextWidth( Text text )
    {
        int totalLength = 0;

        Font font = text.font; //text is my UI text
        char[] arr = text.text.ToCharArray();

        foreach( char c in arr )
        {
            font.RequestCharactersInTexture( c.ToString(), text.fontSize, text.fontStyle );
            font.GetCharacterInfo( c, out var characterInfo, text.fontSize );
            totalLength += characterInfo.advance;
        }

        return totalLength;
    }
}

public class WeightedSelector< T >
{
    public WeightedSelector( Func<int, int, int> randomGenerator )
    {
        randomGeneratorPred = randomGenerator;
    }

    public WeightedSelector()
    {
        randomGeneratorPred = ( int min, int max ) => { return UnityEngine.Random.Range( min, max ); };
    }

    public void AddItem( T item, int weight )
    {
        if( weight <= 0 )
            return;
        total += weight;

        if( randomGeneratorPred( 0, total - 1 ) < weight )
            current = item;
    }

    public T GetResult()
    {
        return current;
    }

    public bool HasResult()
    {
        return total != 0;
    }

    private T current;
    private int total = 0;
    private Func<int, int, int> randomGeneratorPred;
}

public class TransformData
{
    public Vector3 translation;
    public Quaternion rotation;
    public Vector3 scale;
}

public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair( T first, U second )
    {
        First = first;
        Second = second;
    }

    public static bool operator ==( Pair<T, U> lhs, Pair<T, U> rhs )
    {
        if( ReferenceEquals( lhs, null ) )
            return ReferenceEquals( rhs, null );
        return lhs.Equals( rhs );
    }

    public static bool operator !=( Pair<T, U> lhs, Pair<T, U> rhs )
    {
        return !( lhs == rhs );
    }

    public override bool Equals( object obj )
    {
        if( ReferenceEquals( obj, null ) )
            return false;

        var rhs = obj as Pair<T, U>;
        return !ReferenceEquals( rhs, null ) && Equals( rhs );
    }

    public bool Equals( Pair<T, U> obj )
    {
        if( ReferenceEquals( obj, null ) )
            return false;

        return First.Equals( obj.First ) && Second.Equals( obj.Second );
    }

    public override int GetHashCode()
    {
        return ( 23 * First.GetHashCode() ) ^ ( 397 * Second.GetHashCode() );
    }

    public void Deconstruct( out T first, out U second )
    {
        first = First;
        second = Second;
    }

    public T First;
    public U Second;
}

public class Interval : Pair<float, float>
{
    public Interval( float min, float max )
        : base( min, max )
    {

    }

    public float Range() { return Second - First; }
    public bool Contains( float value ) { return value >= First && value <= Second; }
    public float Random() { return UnityEngine.Random.Range( First, Second ); }
}

public enum EAxis
{
    X,
    Y,
    Z,
    None,
}

public static partial class Utility
{
    public static IEnumerable<Pair<A, B>> Zip<A, B>( this IEnumerable<A> a, IEnumerable<B> b )
    {
        using var iteratorA = a.GetEnumerator();
        using var iteratorB = b.GetEnumerator();
        while( iteratorA.MoveNext() && iteratorB.MoveNext() )
        {
            yield return new Pair<A, B>( iteratorA.Current, iteratorB.Current );
        }
    }
}

#if UNITY_EDITOR
    public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
    {
        return EditorGUI.GetPropertyHeight( property, label, true );
    }

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        GUI.enabled = false;
        EditorGUI.PropertyField( position, property, label, true );
        GUI.enabled = true;
    }
}
#endif