using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static partial class Utility
{
    public static IEnumerator FadeToTransparent( CanvasGroup group, float fadeDurationSec, EasingFunction easingFunction = null, bool disableObjectOnFinish = false )
    {
        if( fadeDurationSec <= 0.0f )
        {
            Debug.LogError( "FadeToTransparent called with a negative or 0 duration" );
            yield return null;
        }

        if( group == null )
        {
            Debug.LogError( "FadeToTransparent called with a null CanvasGroup" );
            yield return null;
        }

        group.gameObject.SetActive( true );

        float t = 0.0f;

        while( group != null && t < 1.0f )
        {
            t = Mathf.Min( 1.0f, t + Time.deltaTime * ( 1.0f / fadeDurationSec ) );
            group.alpha = 1.0f - ( easingFunction != null ? easingFunction( t ) : t );
            yield return null;
        }

        if( group != null )
        {
            group.SetVisibility( false );
            if( disableObjectOnFinish )
                group.gameObject.SetActive( false );
        }
    }

    public static void FadeToTransparent( this MonoBehaviour mono, float fadeDurationSec, EasingFunction easingFunction = null, bool disableObjectOnFinish = false )
    {
        mono.StartCoroutine( FadeToTransparent( mono.GetComponent<CanvasGroup>(), fadeDurationSec, easingFunction, disableObjectOnFinish ) );
    }

    public static void FadeToTransparent( this MonoBehaviour mono, CanvasGroup group, float fadeDurationSec, EasingFunction easingFunction = null, bool disableObjectOnFinish = false )
    {
        mono.StartCoroutine( FadeToTransparent( group, fadeDurationSec, easingFunction, disableObjectOnFinish ) );
    }

    public static void FadeToTransparent( this MonoBehaviour mono, GameObject group, float fadeDurationSec, EasingFunction easingFunction = null, bool disableObjectOnFinish = false )
    {
        mono.StartCoroutine( FadeToTransparent( group.GetComponent<CanvasGroup>(), fadeDurationSec, easingFunction, disableObjectOnFinish ) );
    }

    public static IEnumerator FadeFromTransparent( CanvasGroup group, float fadeDurationSec, EasingFunction easingFunction = null )
    {
        if( fadeDurationSec <= 0.0f )
        {
            Debug.LogError( "FadeFromTransparent called with a negative or 0 duration" );
            yield return null;
        }

        if( group == null )
        {
            Debug.LogError( "FadeFromTransparent called with a null CanvasGroup" );
            yield return null;
        }

        group.gameObject.SetActive( true );
        float t = 0.0f;

        while( group != null && t < 1.0f )
        {
            t = Mathf.Min( 1.0f, t + Time.deltaTime * ( 1.0f / fadeDurationSec ) );
            group.alpha = easingFunction != null ? easingFunction( t ) : t;
            yield return null;
        }

        if( group != null )
            group.SetVisibility( true );
    }

    public static void FadeFromTransparent( this MonoBehaviour mono, float fadeDurationSec, EasingFunction easingFunction = null )
    {
        mono.StartCoroutine( FadeFromTransparent( mono.GetComponent<CanvasGroup>(), fadeDurationSec, easingFunction ) );
    }

    public static void FadeFromTransparent( this MonoBehaviour mono, CanvasGroup group, float fadeDurationSec, EasingFunction easingFunction = null )
    {
        mono.StartCoroutine( FadeFromTransparent( group, fadeDurationSec, easingFunction ) );
    }

    public static void FadeFromTransparent( this MonoBehaviour mono, GameObject group, float fadeDurationSec, EasingFunction easingFunction = null )
    {
        mono.StartCoroutine( FadeFromTransparent( group.GetComponent<CanvasGroup>(), fadeDurationSec, easingFunction ) );
    }

    public static void FadeToColour( this MonoBehaviour mono, Color colour, float fadeDurationSec, EasingFunction easingFunction = null, bool disableObjectOnFinish = false )
    {
        mono.FadeToColour( mono.GetComponent<Graphic>(), colour, fadeDurationSec, easingFunction, disableObjectOnFinish );
    }

    public static void FadeToColour( this MonoBehaviour mono, Graphic image, Color colour, float fadeDurationSec, EasingFunction easingFunction = null, bool disableObjectOnFinish = false )
    {
        mono.StartCoroutine( FadeToColour( image, colour, fadeDurationSec, easingFunction, disableObjectOnFinish ) );
    }

    public static IEnumerator FadeToColour( Graphic image, Color colour, float fadeDurationSec, EasingFunction easingFunction = null, bool disableObjectOnFinish = false )
    {
        if( fadeDurationSec <= 0.0f )
        {
            Debug.LogError( "FadeToColour called with a negative or 0 duration" );
            yield return null;
        }

        if( image == null )
        {
            Debug.LogError( "FadeToColour called with a null image Graphic" );
            yield return null;
        }

        image.gameObject.SetActive( true );

        var startColour = image.color;

        for( float interp = 0.0f; interp < 1.0f && image != null; interp += Time.deltaTime * ( 1.0f / fadeDurationSec ) )
        {
            image.color = InterpolateColour( startColour, colour, interp, easingFunction );
            yield return null;
        }

        if( image != null )
        {
            image.color = colour;
            if( disableObjectOnFinish )
                image.gameObject.SetActive( false );
        }
    }

    public static void InterpolateScale( this MonoBehaviour mono, Vector3 targetScale, float durationSec, EasingFunction easingFunction = null )
    {
        mono.InterpolateScale( mono.transform, targetScale, durationSec, easingFunction );
    }

    public static void InterpolateScale( this MonoBehaviour mono, Transform transform, Vector3 targetScale, float durationSec, EasingFunction easingFunction = null )
    {
        mono.StartCoroutine( InterpolateScale( transform, targetScale, durationSec, easingFunction ) );
    }

    public static IEnumerator InterpolateScale( Transform transform, Vector3 targetScale, float durationSec, EasingFunction easingFunction = null )
    {
        if( durationSec <= 0.0f )
        {
            Debug.LogError( "InterpolateScale called with a negative or 0 duration" );
            yield return null;
        }

        var startScale = transform.localScale;
        float t = 0.0f;

        while( transform != null && t < 1.0f )
        {
            t = Mathf.Min( 1.0f, t + Time.deltaTime * ( 1.0f / durationSec ) );
            float interpValue = easingFunction != null ? easingFunction( t ) : t;
            transform.localScale = Utility.Lerp( startScale, targetScale, interpValue );
            yield return null;
        }

        if( transform != null )
            transform.localScale = targetScale;
    }

    private static Vector3 GetPosition( Transform t, bool local )
    {
        return local ? t.localPosition : t.position;
    }

    private static void SetPosition( Transform t, bool local, Vector3 position )
    {
        if( t != null )
        {
            if( local )
                t.localPosition = position;
            else
                t.position = position;
        }
    }

    public static void InterpolatePosition( this MonoBehaviour mono, Vector3 targetPosition, float durationSec, bool localPosition = false, EasingFunction easingFunction = null/*= EasingFunction.Linear*/ )
    {
        mono.InterpolatePosition( mono.transform, targetPosition, durationSec, localPosition, easingFunction );
    }

    public static void InterpolatePosition( this MonoBehaviour mono, Transform transform, Vector3 targetPosition, float durationSec, bool localPosition = false, EasingFunction easingFunction = null )
    {
        mono.StartCoroutine( InterpolatePosition( transform, targetPosition, durationSec, localPosition, easingFunction ) );
    }

    public static IEnumerator InterpolatePosition( Transform transform, Vector3 targetPosition, float durationSec, bool localPosition = false, EasingFunction easingFunction = null/*= EasingFunction.Linear*/ )
    {
        if( durationSec <= 0.0f )
        {
            Debug.LogError( "InterpolatePosition called with a negative or 0 duration" );
            yield return null;
        }

        var startPos = GetPosition( transform, localPosition );
        float t = 0.0f;

        while( transform != null && t < 1.0f )
        {
            t = Mathf.Min( 1.0f, t + Time.deltaTime * ( 1.0f / durationSec ) );
            float interpValue = easingFunction != null ? easingFunction( t ) : t;
            SetPosition( transform, localPosition, Lerp( startPos, targetPosition, interpValue ) );
            yield return null;
        }

        SetPosition( transform, localPosition, targetPosition );
    }

    private static Quaternion GetRotation( Transform t, bool local )
    {
        return local ? t.localRotation : t.rotation;
    }

    private static void SetRotation( Transform t, bool local, Quaternion rotation )
    {
        if( t != null )
        {
            if( local )
                t.localRotation = rotation;
            else
                t.rotation = rotation;
        }
    }

    public static void InterpolateRotation( this MonoBehaviour mono, Vector3 rotation, float durationSec, bool localRotation = true, EasingFunction easingFunction = null/*= EasingFunction.Linear*/)
    {
        mono.InterpolateRotation( mono.transform, rotation, durationSec, localRotation, easingFunction );
    }

    public static void InterpolateRotation( this MonoBehaviour mono, Transform transform, Vector3 rotation, float durationSec, bool localRotation = true, EasingFunction easingFunction = null/*= EasingFunction.Linear*/ )
    {
        mono.StartCoroutine( InterpolateRotation( transform, rotation, durationSec, localRotation, easingFunction ) );
    }

    public static IEnumerator InterpolateRotation( Transform transform, Vector3 rotation, float durationSec, bool localRotation = true, EasingFunction easingFunction = null/*= EasingFunction.Linear*/)
    {
        if( durationSec <= 0.0f )
        {
            Debug.LogError( "InterpolateRotation called with a negative or 0 duration" );
            yield return null;
        }

        var startRot = GetRotation( transform, localRotation );
        var goalRot = Quaternion.Euler( startRot.eulerAngles + rotation );
        float t = 0.0f;

        while( transform != null && t < 1.0f)
        {
            t = Mathf.Min( 1.0f, t + Time.deltaTime * ( 1.0f / durationSec ) );
            float interpValue = easingFunction != null ? easingFunction( t ) : t;
            SetRotation( transform, localRotation, Quaternion.Slerp( startRot, goalRot, interpValue ) );
            yield return null;
        }
    }

#if PATH_CREATOR_PACKAGE
    public static void InterpolateAlongPath( this MonoBehaviour mono, PathCreation.PathCreator path, float durationSec )
    {
        mono.InterpolateAlongPath( mono.transform, path, durationSec );
    }

    public static void InterpolateAlongPath( this MonoBehaviour mono, Transform transform, PathCreation.PathCreator path, float durationSec )
    {
        mono.StartCoroutine( InterpolateAlongPath( transform, path, durationSec ) );
    }

    public static IEnumerator InterpolateAlongPath( Transform transform, PathCreation.PathCreator path, float durationSec )
    {
        if( durationSec <= 0.0f )
        {
            Debug.LogError( "InterpolateAlongPath called with a negative or 0 duration" );
            yield return null;
        }

        float interp = 0.0f;
        var startPos = transform.position;

        while( interp < 1.0f && transform != null )
        {
            interp += Time.deltaTime / durationSec;
            transform.position = startPos + path.path.GetPointAtTime( interp, PathCreation.EndOfPathInstruction.Stop ) - path.path.GetPoint( 0 );
            transform.rotation = Quaternion.LookRotation( Vector3.forward, path.path.GetDirection( interp, PathCreation.EndOfPathInstruction.Stop ) );
            yield return null;
        }
    }
#endif // PATH_CREATOR_PACKAGE

    public static void Shake( this MonoBehaviour mono, float durationSec, float amplitudeStart, float amplitudeEnd, float frequency, float yMultiplier )
    {
        mono.Shake( mono.transform, durationSec, amplitudeStart, amplitudeEnd, frequency, yMultiplier );
    }

    public static void Shake( this MonoBehaviour mono, Transform target, float durationSec, float amplitudeStart, float amplitudeEnd, float frequency, float yMultiplier )
    {
        mono.StartCoroutine( Shake( target, durationSec, amplitudeStart, amplitudeEnd, frequency, yMultiplier ) );
    }

    public static IEnumerator Shake( Transform transform, float durationSec, float amplitudeStart, float amplitudeEnd, float frequency, float yMultiplier )
    {
        if( durationSec <= 0.0f )
        {
            Debug.LogError( "Shake called with a negative or 0 duration" );
            yield return null;
        }

        var elapsed = 0.0f;
        var originalPos = transform.localPosition;

        while( elapsed < durationSec && transform != null )
        {
            elapsed += Time.deltaTime;

            var dynamicAmplitude = Mathf.Lerp( amplitudeStart, amplitudeEnd, elapsed / durationSec );
            transform.localPosition = originalPos + new Vector3(
                Mathf.Sin( elapsed * frequency ) * dynamicAmplitude,
                Mathf.Sin( elapsed * frequency * yMultiplier ) * dynamicAmplitude / yMultiplier,
                0.0f );

            yield return null;
        }

        if( transform != null )
            transform.localPosition = originalPos;
    }

    public static void CallWithDelay( this MonoBehaviour mono, float delaySec, Action action)
    {
        mono.StartCoroutine( CallWithDelay( delaySec, action ) );
    }

    public static IEnumerator CallWithDelay( float delaySec, Action action )
    {
        yield return new WaitForSeconds( delaySec );
        action();
    }
}