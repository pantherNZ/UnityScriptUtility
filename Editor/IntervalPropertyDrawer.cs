﻿using UnityEngine;
using UnityEditor;

public class IntervalRangeAttribute : PropertyAttribute
{
	public float Min { get; set; }
	public float Max { get; set; }
	public bool DataFields { get; set; } = true;
	public bool FlexibleFields { get; set; } = true;
	public bool Bound { get; set; } = true;
	public bool Round { get; set; } = true;

	public IntervalRangeAttribute() : this( 0, 1 )
	{
	}

	public IntervalRangeAttribute( float min, float max )
	{
		Min = min;
		Max = max;
	}
}

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( IntervalRangeAttribute ) )]
internal class IntervalRangeDrawer : PropertyDrawer
{
	private const string kMinName = "First";
	private const string kMaxName = "Second";
	private const float kFloatFieldWidth = 50f;
	private const float kSpacing = 2f;
	private const float kRoundingValue = 1000f;

	private static readonly int controlHash = "Foldout".GetHashCode();
	private static readonly GUIContent unsupported = EditorGUIUtility.TrTextContent( "Unsupported field type" );

	private bool pressed;
	private float pressedMin;
	private float pressedMax;

	private float Round( float value, float roundingValue )
	{
		return roundingValue == 0 ? value : Mathf.Round( value * roundingValue ) / roundingValue;
	}

	private float FlexibleFloatFieldWidth( float min, float max )
	{
		var n = Mathf.Max( Mathf.Abs( min ), Mathf.Abs( max ) );
		return 28.0f + ( Mathf.Floor( Mathf.Log10( Mathf.Abs( n ) ) + 1 ) * 2.5f );
	}

	private void SetVectorValue( SerializedProperty property, ref float min, ref float max, bool round )
	{
		if ( !pressed || ( pressed && !Mathf.Approximately( min, pressedMin ) ) )
		{
			using var x = property.FindPropertyRelative( kMinName );
			SetValue( x, ref min, round );
		}

		if ( !pressed || ( pressed && !Mathf.Approximately( max, pressedMax ) ) )
		{
			using var y = property.FindPropertyRelative( kMaxName );
			SetValue( y, ref max, round );
		}
	}

	private void SetValue( SerializedProperty property, ref float v, bool round )
	{
		switch ( property.propertyType )
		{
			case SerializedPropertyType.Float:
				{
					if ( round )
					{
						v = Round( v, kRoundingValue );
					}
					property.floatValue = v;
				}
				break;
			case SerializedPropertyType.Integer:
				{
					property.intValue = Mathf.RoundToInt( v );
				}
				break;
			default:
				break;
		}
	}

	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		label = EditorGUI.BeginProperty( position, label, property );

		var minProp = property.FindPropertyRelative( kMinName );
		var maxProp = property.FindPropertyRelative( kMaxName );
		var min = minProp.floatValue;
		var max = maxProp.floatValue;

		var attr = attribute as IntervalRangeAttribute;

		float ppp = EditorGUIUtility.pixelsPerPoint;
		float spacing = kSpacing * ppp;
		float fieldWidth = ppp * ( attr.DataFields && attr.FlexibleFields ?
			FlexibleFloatFieldWidth( attr.Min, attr.Max ) :
			kFloatFieldWidth );

		var indent = EditorGUI.indentLevel;

		int id = GUIUtility.GetControlID( controlHash, FocusType.Keyboard, position );
		var r = EditorGUI.PrefixLabel( position, id, label );

		Rect sliderPos = r;

		if ( attr.DataFields )
		{
			sliderPos.x += fieldWidth + spacing;
			sliderPos.width -= ( fieldWidth + spacing ) * 2;
		}

		if ( Event.current.type == EventType.MouseDown &&
			sliderPos.Contains( Event.current.mousePosition ) )
		{
			pressed = true;
			min = Mathf.Clamp( min, attr.Min, attr.Max );
			max = Mathf.Clamp( max, attr.Min, attr.Max );
			pressedMin = min;
			pressedMax = max;
			SetVectorValue( property, ref min, ref max, attr.Round );
			GUIUtility.keyboardControl = 0; // TODO keep focus but stop editing
		}

		if ( pressed && Event.current.type == EventType.MouseUp )
		{
			if ( attr.Round )
			{
				SetVectorValue( property, ref min, ref max, true );
			}
			pressed = false;
		}

		EditorGUI.BeginChangeCheck();
		EditorGUI.indentLevel = 0;
		EditorGUI.MinMaxSlider( sliderPos, ref min, ref max, attr.Min, attr.Max );
		EditorGUI.indentLevel = indent;
		if ( EditorGUI.EndChangeCheck() )
		{
			SetVectorValue( property, ref min, ref max, false );
		}

		if ( attr.DataFields )
		{
			Rect minPos = r;
			minPos.width = fieldWidth;

			var vectorMinProp = property.FindPropertyRelative( kMinName );
			EditorGUI.showMixedValue = vectorMinProp.hasMultipleDifferentValues;
			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel = 0;
			min = EditorGUI.DelayedFloatField( minPos, min );
			EditorGUI.indentLevel = indent;
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( attr.Bound )
				{
					min = Mathf.Max( min, attr.Min );
					min = Mathf.Min( min, max );
				}
				SetVectorValue( property, ref min, ref max, attr.Round );
			}
			vectorMinProp.Dispose();

			Rect maxPos = position;
			maxPos.x += maxPos.width - fieldWidth;
			maxPos.width = fieldWidth;

			var vectorMaxProp = property.FindPropertyRelative( kMaxName );
			EditorGUI.showMixedValue = vectorMaxProp.hasMultipleDifferentValues;
			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel = 0;
			max = EditorGUI.DelayedFloatField( maxPos, max );
			EditorGUI.indentLevel = indent;
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( attr.Bound )
				{
					max = Mathf.Min( max, attr.Max );
					max = Mathf.Max( max, min );
				}
				SetVectorValue( property, ref min, ref max, attr.Round );
			}
			vectorMaxProp.Dispose();

			EditorGUI.showMixedValue = false;
		}

		EditorGUI.EndProperty();
	}
}

#endif
