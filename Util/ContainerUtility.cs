
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Extensions
{
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

    public static Pair<U, V> FindPairFirst<U, V>( this List<Pair<U, V>> list, U item )
    {
        return list.Find( x => x.First.Equals( item ) );
    }

    public static Pair<U, V> FindPairSecond<U, V>( this List<Pair<U, V>> list, U item )
    {
        return list.Find( x => x.Second.Equals( item ) );
    }

    public static void RemoveBySwap<T>( this List<T> list, int index )
    {
        list[index] = list[list.Count - 1];
        list.RemoveAt( list.Count - 1 );
    }

    public static bool Remove<T>( this List<T> list, Predicate<T> match )
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

    public static List<T> Rotate<T>( this List<T> list, int offset )
    {
        if( offset == 0 )
            return list;
        offset = list.Count - Utility.Mod( offset, list.Count );
        offset = ( offset < 0 ? list.Count + offset : offset );
        return list.Skip( offset ).Concat( list.Take( offset ) ).ToList();
    }

    public static T PopFront<T>( this List<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use PopFront on an empty list!" );

        var last = list[0];
        list.RemoveAt( 0 );
        return last;
    }

    public static T PopBack<T>( this List<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use PopBack on an empty list!" );

        var last = list[list.Count - 1];
        list.RemoveAt( list.Count - 1 );
        return last;
    }

    public static T Front<T>( this List<T> list )
    {
        if( list.IsEmpty() )
            throw new System.ArgumentException( "You cannot use Front on an empty list!" );

        var first = list[0];
        return first;
    }

    public static T Back<T>( this List<T> list )
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

    public static T RandomItem<T>( this List<T> list, T defaultValue = default )
    {
        if( list.IsEmpty() )
            return defaultValue;
        return list[UnityEngine.Random.Range( 0, list.Count )];
    }

    public static KeyValuePair<TKey, TValue> RandomItem<TKey, TValue>( this Dictionary<TKey, TValue> dict, KeyValuePair<TKey, TValue> defaultValue = default )
    {
        if( dict.IsEmpty() )
            return defaultValue;
        return dict.ElementAt( UnityEngine.Random.Range( 0, dict.Count ) );
    }

    public static TKey RandomItem<TKey>( this HashSet<TKey> dict, TKey defaultValue = default )
    {
        if( dict.IsEmpty() )
            return defaultValue;
        return dict.ElementAt( UnityEngine.Random.Range( 0, dict.Count ) );
    }

    public static List<T> RandomShuffle<T>( this List<T> list )
    {
        for( int i = 0; i < list.Count; i++ )
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range( i, list.Count );
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

    public static IEnumerable<Pair<int, Transform>> Enumerate( this Transform collection, int startIndex = 0 )
    {
        foreach( Transform item in collection )
        {
            yield return new Pair<int, Transform>( startIndex++, item );
        }
    }

    public static T RandomItem<T>( this IEnumerable<T> collection, T defaultValue = default )
    {
        var length = collection.Count();
        if( length == 0 )
            return defaultValue;
        return collection.ElementAtOrDefault( UnityEngine.Random.Range( 0, length ) );
    }
}

public static partial class Utility
{
    public static IEnumerable<Pair<A, B>> Zip<A, B>( this IEnumerable<A> a, IEnumerable<B> b )
    {
        using var iteratorA = a.GetEnumerator();
        using var iteratorB = b.GetEnumerator();
        while( iteratorA.MoveNext() && iteratorB.MoveNext() )
        {
            yield return new Pair<A, B>( iteratorA.Current, iteratorB.Current );
        }
    }

    public static IEnumerable<Tuple<A, B, C>> Zip<A, B, C>( this IEnumerable<A> a, IEnumerable<B> b, IEnumerable<C> c )
    {
        using var iteratorA = a.GetEnumerator();
        using var iteratorB = b.GetEnumerator();
        using var iteratorC = c.GetEnumerator();
        while( iteratorA.MoveNext() && iteratorB.MoveNext() && iteratorC.MoveNext() )
        {
            yield return new Tuple<A, B, C>( iteratorA.Current, iteratorB.Current, iteratorC.Current );
        }
    }
}