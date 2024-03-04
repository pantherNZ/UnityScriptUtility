using System;
using System.ComponentModel;

[Serializable]
public class Property<T>
{
	public T property
	{
		get { return _property; }
		set
		{
			if ( (_property != null && _property.Equals( value )) || (_property == null && value == null) )
			{
				return;
			}
			_property = value;
			onChanged?.Invoke( _property );
		}
	}

	public void Bind( Action<T> func )
	{
		onChanged += func;
		func( _property );
	}

	private Action<T> onChanged;
	private T _property;
}
