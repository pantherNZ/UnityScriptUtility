using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;

internal static class Functions
{
	internal static string FormatValue<T>( T value ) => $"{typeof( T ).FullName}: {value?.ToString()}";
	internal static string FormatValue<T>( object @this, object @base, T value ) =>
		ReferenceEquals( @this, value ) ?
			@base.ToString() :
			$"{typeof( T ).FullName}: {value?.ToString()}";
}

public interface IVariant
{
	object Value { get; }
	int Index { get; }
	abstract Type[] GetVariantTypes();
	Type GetVariantType() { return GetVariantTypes()[Index]; }
}

[Serializable]
public class Variant<T0, T1> : IVariant
{
	[SerializeField] T0 _value0;
	[SerializeField] T1 _value1;
	[SerializeField] int _index;

	public Type[] GetVariantTypes() => new Type[] { typeof( T0 ), typeof( T1 ) };

	public Variant( object t )
	{
		var types = GetVariantTypes();
		foreach( var ( idx, type ) in Utility.Enumerate( types ) )
		{
			if( type == t.GetType() )
			{
				_index = idx;
				switch ( _index )
				{
					case 0: _value0 = ( T0 )t; return;
					case 1: _value1 = ( T1 )t; return;
					default: throw new InvalidOperationException();
				}
			}
		}
	}

	Variant( int index, T0 value0 = default, T1 value1 = default )
	{
		_index = index;
		_value0 = value0;
		_value1 = value1;
	}

	public object Value =>
		_index switch
		{
			0 => _value0,
			1 => _value1,
			_ => throw new InvalidOperationException()
		};

	public int Index => _index;

	public bool IsT0 => _index == 0;
	public bool IsT1 => _index == 1;

	public T0 AsT0 =>
		_index == 0 ?
			_value0 :
			throw new InvalidOperationException( $"Cannot return as T0 as result is T{_index}" );
	public T1 AsT1 =>
		_index == 1 ?
			_value1 :
			throw new InvalidOperationException( $"Cannot return as T1 as result is T{_index}" );

	public static implicit operator Variant<T0, T1>( T0 t ) => new Variant<T0, T1>( 0, value0: t );
	public static implicit operator Variant<T0, T1>( T1 t ) => new Variant<T0, T1>( 1, value1: t );

	public void Switch( Action<T0> f0, Action<T1> f1 )
	{
		if ( _index == 0 && f0 != null )
		{
			f0( _value0 );
			return;
		}
		if ( _index == 1 && f1 != null )
		{
			f1( _value1 );
			return;
		}
		throw new InvalidOperationException();
	}

	public TResult Match<TResult>( Func<T0, TResult> f0, Func<T1, TResult> f1 )
	{
		if ( _index == 0 && f0 != null )
		{
			return f0( _value0 );
		}
		if ( _index == 1 && f1 != null )
		{
			return f1( _value1 );
		}
		throw new InvalidOperationException();
	}

	public static Variant<T0, T1> FromT0( T0 input ) => input;
	public static Variant<T0, T1> FromT1( T1 input ) => input;


	public Variant<TResult, T1> MapT0<TResult>( Func<T0, TResult> mapFunc )
	{
		if ( mapFunc == null )
		{
			throw new ArgumentNullException( nameof( mapFunc ) );
		}
		return _index switch
		{
			0 => mapFunc( AsT0 ),
			1 => AsT1,
			_ => throw new InvalidOperationException()
		};
	}

	public Variant<T0, TResult> MapT1<TResult>( Func<T1, TResult> mapFunc )
	{
		if ( mapFunc == null )
		{
			throw new ArgumentNullException( nameof( mapFunc ) );
		}
		return _index switch
		{
			0 => AsT0,
			1 => mapFunc( AsT1 ),
			_ => throw new InvalidOperationException()
		};
	}

	public bool TryPickT0( out T0 value, out T1 remainder )
	{
		value = IsT0 ? AsT0 : default;
		remainder = _index switch
		{
			0 => default,
			1 => AsT1,
			_ => throw new InvalidOperationException()
		};
		return this.IsT0;
	}

	public bool TryPickT1( out T1 value, out T0 remainder )
	{
		value = IsT1 ? AsT1 : default;
		remainder = _index switch
		{
			0 => AsT0,
			1 => default,
			_ => throw new InvalidOperationException()
		};
		return this.IsT1;
	}

	bool Equals( Variant<T0, T1> other ) =>
		_index == other._index &&
		_index switch
		{
			0 => Equals( _value0, other._value0 ),
			1 => Equals( _value1, other._value1 ),
			_ => false
		};

	public override bool Equals( object obj )
	{
		if ( ReferenceEquals( null, obj ) )
		{
			return false;
		}

		return obj is Variant<T0, T1> o && Equals( o );
	}

	public override string ToString() =>
		_index switch
		{
			0 => Functions.FormatValue( _value0 ),
			1 => Functions.FormatValue( _value1 ),
			_ => throw new InvalidOperationException( "Unexpected index, which indicates a problem in the OneOf codegen." )
		};

	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = _index switch
			{
				0 => _value0?.GetHashCode(),
				1 => _value1?.GetHashCode(),
				_ => 0
			} ?? 0;
			return ( hashCode * 397 ) ^ _index;
		}
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( Variant<,> ), true )]
public class VariantPropertyDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		EditorGUI.BeginProperty( position, label, property );

		position = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );

		var variant = property.boxedValue as IVariant;
		if ( variant == null )
			return;
		
		EditorGUI.indentLevel++;

		var newIdx = EditorGUILayout.Popup( new GUIContent( "SelectableType" ), variant.Index, variant.GetVariantTypes().Select( impl => impl.Name ).ToArray() );
		
		if ( newIdx != variant.Index )
		{
			var newValue = Activator.CreateInstance( variant.GetVariantTypes()[newIdx] );
			variant = Activator.CreateInstance( variant.GetType(), newValue ) as IVariant;
			property.boxedValue = variant;
			property.serializedObject.ApplyModifiedProperties();
		}

		var propertyStr = $"_value{variant.Index}";
		var propertyDisplayTitle = variant.GetVariantType().Name;
		EditorGUILayout.PropertyField( property.FindPropertyRelative( propertyStr ), new GUIContent( propertyDisplayTitle ), true );

		var amountRect = new Rect( position.x, position.y, 30, position.height );

		EditorGUI.indentLevel--;

		EditorGUI.EndProperty();
	}
}
#endif
