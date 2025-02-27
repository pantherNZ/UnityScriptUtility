
using System.Collections.Generic;
using System;
using UnityEngine;

public static partial class Utility
{
	public static void AssertEq<T>( T a, T b, string msg = null ) where T : IComparable
	{
		Debug.Assert( EqualityComparer<T>.Default.Equals( a, b ), $"{msg ?? "AssertEq failed:"} (a:\"{a}\", b:\"{b}\")" );
	}

	public static void AssertNotEq<T>( T a, T b, string msg = null ) where T : IComparable
	{
		Debug.Assert( !EqualityComparer<T>.Default.Equals( a, b ), $"{msg ?? "AssertNotEq failed:"} (a:\"{a}\", b:\"{b}\")" );
	}
}
