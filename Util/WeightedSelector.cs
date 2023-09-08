using System;

public class WeightedSelector<T>
{
    public WeightedSelector( Func<int, int, int> randomGenerator )
    {
        randomGeneratorPred = randomGenerator;
    }

    public WeightedSelector()
    {
        randomGeneratorPred = ( int min, int max ) => { return UnityEngine.Random.Range( min, max ); };
    }

    public void AddItem( T item, int weight )
    {
        if( weight <= 0 )
            return;
        total += weight;

        if( randomGeneratorPred( 0, total - 1 ) < weight )
            current = item;
    }

    public T GetResult()
    {
        return current;
    }

    public bool HasResult()
    {
        return total != 0;
    }

    private T current;
    private int total = 0;
    private Func<int, int, int> randomGeneratorPred;
}