using UnityEngine;

public static partial class Utility
{
	public static bool HasComponent<T>( this GameObject obj ) where T : Component
	{
		return obj.GetComponent<T>() != null;
	}

}
