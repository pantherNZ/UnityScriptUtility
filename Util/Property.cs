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

		this.value = value;

		if ( triggerCallback )
			TriggerChanged();
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

	internal void OnChanged( T value )
	{
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
