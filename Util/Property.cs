using System;
using Newtonsoft.Json;

public class Property<T>
{
	[JsonProperty]
	public T value { get; protected set; }

	~Property()
	{
		Unbind();
	}

	// Simple binding for just new value (unbind doesn't work for this)
	public void Bind( Action<T> func, bool andCall = true )
	{
		Bind( ( oldVal, newVal ) => func( newVal ), andCall );
	}

	// Binding for both before value AND new value
	public void Bind( Action<T, T> func, bool andCall = true )
	{
		onChanged += func;
		if ( andCall )
		{
			func( value, value );
		}
	}

	public void Unbind( Action<T, T> func )
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

	protected void TriggerChanged( T oldValue )
	{
		onChanged?.Invoke( oldValue, value );
	}

	protected event Action<T, T> onChanged;
	internal Action<Property<T>> onUnbound;
}

public class ReadWriteProperty<T> : Property<T>
{
	public void SetValue( T value, bool triggerCallback = true )
	{
		if ( ( this.value != null && this.value.Equals( value ) ) || ( this.value == null && value == null ) )
		{
			return;
		}

		var oldValue = this.value;
		this.value = value;

		if ( triggerCallback )
			TriggerChanged( oldValue );
	}
}

[Serializable]
public class Binding<T> : Property<T>
{
	private Action<Binding<T>> _onDestroyed;
	private bool _propagateUnbind;

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

	internal void OnChanged( T oldValue, T value )
	{
		this.value = value;
		TriggerChanged( oldValue );
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
