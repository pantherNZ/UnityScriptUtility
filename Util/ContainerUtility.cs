
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public static partial class Utility
{
	public static IEnumerable<T> ToIEnumerable<T>( this IEnumerator<T> enumerator )
	{
		while ( enumerator.MoveNext() )
		{
			yield return enumerator.Current;
		}
	}

	public static IEnumerable ToIEnumerable( this IEnumerator enumerator )
	{
		while ( enumerator.MoveNext() )
		{
			yield return enumerator.Current;
		}
	}

	public static void Resize<T>( this List<T> list, int size, T value = default )
    {
        int cur = list.Count;
        if( size < cur )
            list.RemoveRange( size, cur - size );
        else if( size > cur )
            list.AddRange( Enumerable.Repeat( value, size - cur ) );
    }

    public static void Resize<T>( this List<T> list, int size, Func<T> generateValueFunc )
    {
        if( size < list.Count )
            list.RemoveRange( size, list.Count - size );
        else
        {
            list.Capacity = size;
            while( size > list.Count )
                list.Add( generateValueFunc() );
        }
    }

    // Be careful using this that you pass it IEnumerator<T> and not List<T>.Enumerator
    // https://codeblog.jonskeet.uk/2010/07/27/iterate-damn-you/
    public static T MoveNextGet<T>( this IEnumerator<T> enumerator )
    {
        bool success = enumerator.MoveNext();
        if( !success )
            throw new System.OverflowException( "MoveNextGet failed to move to the next item" );
        return enumerator.Current;
    }

    public static TValue GetOrAdd<TKey, TValue>( this IDictionary<TKey, TValue> dict, TKey key ) where TValue : new()
    {
        if( !dict.TryGetValue( key, out TValue val ) )
        {
            val = new TValue();
            dict.Add( key, val );
        }

        return val;
    }

	public static void SetOrAddValue<TKey, TValue>( this IDictionary<TKey, TValue> dict, TKey key, TValue tValue ) where TValue : new()
	{
		if ( !dict.TryGetValue( key, out TValue val ) )
			dict.Add( key, tValue );
		else
			dict[key] = tValue;
	}

	public static Pair<U, V> FindPairFirst<U, V>( this List<Pair<U, V>> list, U item )
    {
        return list.Find( x => x.First.Equals( item ) );
    }

    public static Pair<U, V> FindPairSecond<U, V>( this List<Pair<U, V>> list, U item )
    {
        return list.Find( x => x.Second.Equals( item ) );
    }

    public static void RemoveBySwap<T>( this IList<T> list, int index )
    {
        list[index] = list[list.Count - 1];
        list.RemoveAt( list.Count - 1 );
    }

    public static bool Remove<T>( this IList<T> list, Predicate<T> match )
    {
        foreach( var (idx, x) in Enumerate( list ) )
        {
            if( match( x ) )
            {
                list.RemoveAt( idx );
                return true;
            }
        }

        return false;
    }

    public static void RemovePairFirst<U, V>( this List<Pair<U, V>> list, U item )
    {
        RemoveBySwap( list, x => x.First.Equals( item ) );
    }

    public static void RemovePairSecond<U, V>( this List<Pair<U, V>> list, U item )
    {
        RemoveBySwap( list, x => x.Second.Equals( item ) );
    }

    public static bool RemoveBySwap<T>( this List<T> list, Func<T, bool> predicate )
    {
        if( list.IsEmpty() )
            return false;

        var end = list.Count;

        for( int i = 0; i < end; ++i )
        {
            if( predicate( list[i] ) )
            {
                if( i != end - 1 )
                    list[i] = list[end - 1];

                if( end > 0 )
                    end--;
            }
        }

        bool removed = end < list.Count;
        list.Resize( end );
        return removed;
    }

    public static IList<T> Swap<T>( this IList<T> list, int indexA, int indexB )
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        return list;
    }

    public static Dictionary<TKey, TValue> Swap<TKey, TValue>( this Dictionary<TKey, TValue> dict, TKey keyA, TKey keyB )
    {
        (dict[keyA], dict[keyB]) = (dict[keyB], dict[keyA]);
        return dict;
    }

    public static IList<T> Rotate<T>( this IList<T> list, int offset )
    {
        if( offset == 0 )
            return list;
        offset = list.Count - Utility.Mod( offset, list.Count );
        offset = ( offset < 0 ? list.Count + offset : offset );
        return list.Skip( offset ).Concat( list.Take( offset ) ).ToList();
    }

    public static T PopFront<T>( this IList<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use PopFront on an empty list!" );

        var last = list[0];
        list.RemoveAt( 0 );
        return last;
    }

    public static T PopBack<T>( this IList<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use PopBack on an empty list!" );

        var last = list[list.Count - 1];
        list.RemoveAt( list.Count - 1 );
        return last;
    }

    public static T Front<T>( this IList<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use Front on an empty list!" );

        var first = list[0];
        return first;
    }

    public static T Back<T>( this IList<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use Back on an empty list!" );

        var last = list[list.Count - 1];
        return last;
    }

    public static bool IsEmpty<T>( this T list ) where T : ICollection
    {
        return list.Count == 0;
    }

    public static bool IsEmpty<T>( this ICollection<T> list )
    {
        return list.Count == 0;
    }

	public static bool IsEmpty( this string str )
	{
		return str.Length == 0;
	}

	public static T RandomItem<T>( this IList<T> list, T defaultValue = default, IRandom rng = null )
    {
        if( list.IsEmpty() )
            return defaultValue;
        return list[( rng ?? DefaultRng ).Range( 0, list.Count )];
    }

	public static T TakeRandomItem<T>( this IList<T> list, T defaultValue = default, IRandom rng = null )
	{
		if ( list.IsEmpty() )
			return defaultValue;
		var idx = ( rng ?? DefaultRng ).Range( 0, list.Count );
		var item = list[idx];
		list.RemoveAt( idx );
		return item;
	}

	public static KeyValuePair<TKey, TValue> RandomItem<TKey, TValue>( this Dictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> defaultValue = default, IRandom rng = null )
    {
        if( dict.IsEmpty() )
            return defaultValue;
        return dict.ElementAt( ( rng ?? DefaultRng ).Range( 0, dict.Count ) );
    }

	public static KeyValuePair<TKey, TValue> TakeRandomItem<TKey, TValue>( this Dictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> defaultValue = default, IRandom rng = null )
	{
		if ( dict.IsEmpty() )
			return defaultValue;
		var item = dict.ElementAt( ( rng ?? DefaultRng ).Range( 0, dict.Count ) );
		dict.Remove( item.Key );
		return item;
	}

	public static TKey RandomItem<TKey>( this HashSet<TKey> dict, TKey defaultValue = default, IRandom rng = null )
    {
        if( dict.IsEmpty() )
            return defaultValue;
        return dict.ElementAt( ( rng ?? DefaultRng ).Range( 0, dict.Count ) );
    }

    public static IList<T> RandomShuffle<T>( this IList<T> list, IRandom rng = null )
    {
        for( int i = 0; i < list.Count; i++ )
        {
            T temp = list[i];
            int randomIndex = ( rng ?? DefaultRng ).Range( i, list.Count );
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    public static T RemoveAndGet<T>( this List<T> list, Predicate<T> match )
    {
        var idx = list.FindIndex( match );

        if( idx == -1 )
            return default;

        return RemoveAndGet( list, idx );
    }

    public static T RemoveAndGet<T>( this List<T> list, int idx )
    {
        var found = list[idx];
        list.RemoveBySwap( idx );
        return found;
    }

    public static bool Any( this BitArray bitArray )
    {
        for( int i = 0; i < bitArray.Length; ++i )
            if( bitArray.Get( i ) )
                return true;
        return false;
    }

    public static int CountBits( this BitArray bitArray )
    {
        int count = 0;
        for( int i = 0; i < bitArray.Length; ++i )
            if( bitArray.Get( i ) )
                count++;
        return count;
    }

    public static BitArray ToBitArray( this Int64 numeral )
    {
        var array = new int[2];
        array[0] = ( int )numeral;
        array[1] = ( int )( numeral >> 32 );
        return new BitArray( array );
    }

    public static Int64 ToNumeral( this BitArray binary )
    {
        if( binary == null )
            throw new ArgumentNullException( "binary" );
        if( binary.Length > 64 )
            throw new ArgumentException( "must be at most 64 bits long" );

        var array = new int[2];
        binary.CopyTo( array, 0 );
        Int64 output = ( Int64 )array[0];
        output |= ( ( Int64 )array[1] ) << 32;
        return output;
    }

    public static IEnumerable<Pair<int, T>> Enumerate<T>( this IEnumerable<T> collection )
    {
        int counter = 0;
        foreach( var item in collection )
        {
            yield return new Pair<int, T>( counter, item );
            counter++;
        }
    }

	public static T Next<T>( this IEnumerable<T> collection, int count )
	{
		int counter = 0;
		foreach ( var item in collection )
		{
			if ( counter == count )
				return item;
			counter++;
		}

		throw new ArgumentOutOfRangeException( "count cannot be larger than the collection" );
	}

	public static IEnumerable<Pair<int, T>> Enumerate<T>( this IEnumerable<T> collection, int startIndex = 0 )
    {
        foreach( var item in collection )
        {
            yield return new Pair<int, T>( startIndex++, item );
        }
    }

    public static IEnumerable<Pair<int, object>> EnumerateObj<T>( this T collection, int startIndex = 0 ) where T : IEnumerable
    {
        foreach( var item in collection )
        {
            yield return new Pair<int, object>( startIndex++, item );
        }
    }

    public static T RandomItem<T>( this IEnumerable<T> collection, T defaultValue = default, IRandom rng = null )
    {
        var length = collection.Count();
        if( length == 0 )
            return defaultValue;
        return collection.ElementAtOrDefault( ( rng ?? DefaultRng ).Range( 0, length ) );
    }

    public static IEnumerable<( A, B )> Zip<A, B>( this IEnumerable<A> a, IEnumerable<B> b )
    {
        using var iteratorA = a.GetEnumerator();
        using var iteratorB = b.GetEnumerator();
        while( iteratorA.MoveNext() && iteratorB.MoveNext() )
        {
            yield return ( iteratorA.Current, iteratorB.Current );
        }
    }

    public static IEnumerable<( A, B, C )> Zip<A, B, C>( this IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c )
    {
        using var iteratorA = a.GetEnumerator();
        using var iteratorB = b.GetEnumerator();
        using var iteratorC = c.GetEnumerator();
        while( iteratorA.MoveNext() && iteratorB.MoveNext() && iteratorC.MoveNext() )
        {
            yield return ( iteratorA.Current, iteratorB.Current, iteratorC.Current );
        }
    }

	public static IEnumerable<T> ToEnumerable<T>( this T[,] target )
	{
		foreach ( var item in target )
			yield return item;
	}

	public static IEnumerable<T> ToEnumerable<T>( this T[,] target, int firstDimensionIdx )
	{
		for ( var i = 0; i < target.GetLength( 0 ); ++i )
			yield return target[firstDimensionIdx, i];
	}

	public static TSource MinElement<TSource>( this IEnumerable<TSource> source, Func<TSource, float> selector )
	{
		if ( source is IList<TSource> list )
		{
			if ( list.Count == 0 )
			{
				throw new System.ArgumentException( "You cannot use MinElement on an empty list!" );
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			if ( !enumerator.MoveNext() || enumerator.Current == null )
				throw new System.ArgumentException( "You cannot use MinElement on an empty list!" );
		}

		var values = source.Select( selector );
		float f = float.MaxValue;
		int minIdx = -1;

		foreach ( var (idx, v) in values.Enumerate() )
		{
			if ( v < f )
			{
				minIdx = idx;
				f = v;
			}
		}

		return source.ElementAt( minIdx );
	}

	public static TSource MaxElement<TSource>( this IEnumerable<TSource> source, Func<TSource, float> selector )
	{
		if ( source is IList<TSource> list )
		{
			if ( list.Count == 0 )
			{
				throw new System.ArgumentException( "You cannot use MinElement on an empty list!" );
			}
		}
		else
		{
			using IEnumerator<TSource> enumerator = source.GetEnumerator();
			if ( !enumerator.MoveNext() || enumerator.Current == null )
				throw new System.ArgumentException( "You cannot use MinElement on an empty list!" );
		}

		var values = source.Select( selector );
		float f = float.MinValue;
		int minIdx = -1;

		foreach ( var (idx, v) in values.Enumerate() )
		{
			if ( v > f )
			{
				minIdx = idx;
				f = v;
			}
		}

		return source.ElementAt( minIdx );
	}
}
