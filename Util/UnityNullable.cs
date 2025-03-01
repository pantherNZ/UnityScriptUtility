﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Serializable Nullable (UnityNullable) Does the same as C# System.Nullable, except it's an ordinary
/// serializable struct, allowing unity to serialize it and show it in the inspector.
/// </summary>
[System.Serializable]
public struct UnityNullable<T> //where T : struct
{
    public T Value
    {
        get
        {
            if( !HasValue )
                throw new System.InvalidOperationException( "Serializable nullable object must have a value." );
            return v;
        }
    }

#nullable enable
	public T? NullableValue => HasValue ? v : default;
#nullable restore

	public bool HasValue { get { return hasValue; } }

	public T ValueOrDefault( T other )
	{
		return hasValue ? v : other;
	}

	public T ValueOrOther( UnityNullable<T> other )
	{
		return hasValue ? v : other.hasValue ? other.Value : default;
	}

	public UnityNullable<T> ThisOrOther( UnityNullable<T> other )
	{
		return hasValue ? this : other;
	}

	[SerializeField]
    private T v;

    [SerializeField]
    private bool hasValue;

    public UnityNullable( bool hasValue, T v )
    {
        this.v = v;
        this.hasValue = hasValue;
    }

    private UnityNullable( T v )
    {
        this.v = v;
        this.hasValue = true;
    }

    public static implicit operator UnityNullable<T>( T value )
    {
        return new UnityNullable<T>( value );
    }

	public static implicit operator bool( UnityNullable<T> value )
	{
		return value.hasValue;
	}

	// public static implicit operator UnityNullable<T>( System.Nullable<T> value )
	// {
	//     return value.HasValue ? new UnityNullable<T>( value.Value ) : new UnityNullable<T>();
	// }
	//
	// public static implicit operator System.Nullable<T>( UnityNullable<T> value )
	// {
	//     return value.HasValue ? ( T? )value.Value : null;
	// }
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( UnityNullable<> ) )]
internal class UnityNullableDrawer : PropertyDrawer
{
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
		EditorGUI.BeginProperty( position, label, property );

        // Draw label
        position = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;

        // Calculate rects
        var setRect = new Rect( position.x, position.y, 15, position.height );

		// Draw fields - pass GUIContent.none to each so they are drawn without labels
		var hasValueProp = property.FindPropertyRelative( "hasValue" );
        EditorGUI.PropertyField( setRect, hasValueProp, GUIContent.none );

		var valueProp = property.FindPropertyRelative( "v" );
		valueProp.isExpanded = hasValueProp.boolValue;
		bool guiEnabled = GUI.enabled;
		GUI.enabled = guiEnabled && hasValueProp.boolValue;
		EditorGUILayout.PropertyField( property.FindPropertyRelative( "v" ), GUIContent.none );
		GUI.enabled = guiEnabled;

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif
