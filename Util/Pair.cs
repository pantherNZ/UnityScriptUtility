using System;

[Serializable]
public class Pair<T, U>
{
    public Pair()
    {
    }

    public Pair( T first, U second )
    {
        First = first;
        Second = second;
    }

    public static bool operator ==( Pair<T, U> lhs, Pair<T, U> rhs )
    {
        if( ReferenceEquals( lhs, null ) )
            return ReferenceEquals( rhs, null );
        return lhs.Equals( rhs );
    }

    public static bool operator !=( Pair<T, U> lhs, Pair<T, U> rhs )
    {
        return !( lhs == rhs );
    }

    public override bool Equals( object obj )
    {
        if( ReferenceEquals( obj, null ) )
            return false;

        var rhs = obj as Pair<T, U>;
        return !ReferenceEquals( rhs, null ) && Equals( rhs );
    }

    public bool Equals( Pair<T, U> obj )
    {
        if( ReferenceEquals( obj, null ) )
            return false;

        return First.Equals( obj.First ) && Second.Equals( obj.Second );
    }

    public override int GetHashCode()
    {
        return ( 23 * First.GetHashCode() ) ^ ( 397 * Second.GetHashCode() );
    }

    public void Deconstruct( out T first, out U second )
    {
        first = First;
        second = Second;
    }

    public T First;
    public U Second;
}
