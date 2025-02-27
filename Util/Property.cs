using System;
using System.Diagnostics;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Property<T>
{
	[JsonProperty]
	[field: SerializeField]
	[DebuggerDisplay( "Property : {value}" )]
	public T value { get; protected set; }

	~Property()
	{
		Unbind();
	}

	public void Bind( Action<T> func, bool andCall = true )
	{
		onChanged += func;
		if ( andCall )
		{
			func( value );
		}
	}

	public void Unbind( Action<T> func )
	{
		onChanged -= func;
	}

	public void Unbind()
	{
		onUnbound?.Invoke( this );
		onChanged = null;
	}

	internal void Unbind( Binding<T> binding )
	{
		onChanged -= binding.OnChanged;
	}

	protected void TriggerChanged()
	{
		onChanged?.Invoke( value );
	}

	protected event Action<T> onChanged;
	public event Action<Property<T>> onUnbound;
}

[JsonConverter( typeof( ReadWritePropertyJsonConverter ) )]
[Serializable]
[DebuggerDisplay( "ReadWriteProperty : {value}" )]
public class ReadWriteProperty<T> : Property<T>
{
	public void SetValue( T value, bool triggerCallback = true )
	{
		if ( ( this.value != null && this.value.Equals( value ) ) || ( this.value == null && value == null ) )
		{
			return;
		}

		this.value = value;

		if ( triggerCallback )
			TriggerChanged();
	}

	public ReadWriteProperty<T> Clone()
	{
		ReadWriteProperty<T> clone = new();
		clone.SetValue( value );
		return clone;
	}
}

[Serializable]
[DebuggerDisplay( "Binding : {value}" )]
public class Binding<T> : Property<T>
{
	private Action<Binding<T>> _onDestroyed;
	private bool _propagateUnbind;
	private Func<T, T, bool> _predicate;

	public Binding( Property<T> property, bool propagateUnbind = true )
	{
		_onDestroyed += property.Unbind;
		_propagateUnbind = propagateUnbind;
		property.Bind( OnChanged );
		property.onUnbound += OnSourceUnbound;
	}

	~Binding()
	{
		UnbindFromSource();
	}

	public void SetPredicate( Func<T, T, bool> predicate )
	{
		_predicate = predicate;
	}

	internal void OnChanged( T value )
	{
		if ( _predicate != null && !_predicate( this.value, value ) )
			return;
		this.value = value;
		TriggerChanged();
	}

	public void UnbindFromSource()
	{
		_onDestroyed?.Invoke( this );
	}

	internal void OnSourceUnbound( Property<T> source )
	{
		_onDestroyed = null;
		if ( _propagateUnbind )
			Unbind();
	}
}

[Serializable]
[DebuggerDisplay( "ConstantProperty : {value}" )]
public class ConstantProperty<T> : Property<T>
{
	public ConstantProperty( T value )
	{
		this.value = value;
	}
}

public class ReadWritePropertyJsonConverter : JsonConverter
{
	public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
	{
		Type propertyType = typeof( ReadWriteProperty<> );
		Type inputType = value.GetType();
		Type[] inputTypeArgs = inputType.GetGenericArguments();
		Type genericType = propertyType.MakeGenericType( inputTypeArgs );
		dynamic property = Convert.ChangeType( value, genericType );
		serializer.Serialize(writer, property.value);
	}

	public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
	{
		dynamic newProperty = Activator.CreateInstance( objectType );
		if ( existingValue != null )
			newProperty = Convert.ChangeType( existingValue, objectType );

		Type wrappedType = objectType.GetGenericArguments()[0];
		object genericDeserialized = serializer.Deserialize( reader, wrappedType );
		dynamic deserialized = Convert.ChangeType( genericDeserialized, wrappedType );
		newProperty.SetValue( deserialized );

		return newProperty;
	}

	public override bool CanConvert( Type objectType )
	{
		return objectType.GetGenericTypeDefinition().Equals(typeof( ReadWriteProperty<> ));
	}
}


#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( Property<> ) )]
internal class UtilityPropertyDrawer : PropertyDrawer
{
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
	{
		SerializedProperty prop = property.FindPropertyRelative( "<value>k__BackingField" );
		EditorGUILayout.PropertyField( prop, label );
	}
}
#endif
