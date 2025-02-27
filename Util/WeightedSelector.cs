using System;
using System.Collections.Generic;

public class WeightedSelector<T>
{
	public WeightedSelector( Func<int, int> randomGenerator )
	{
		randomGeneratorPred = randomGenerator;
	}

	public WeightedSelector( Utility.IRandom rng = null )
	{
		this.rng = rng;
	}

	public void AddItem( T item, int weight )
	{
		if ( weight > 0 )
		{
			total += weight;
			items.Add( ( item, total ) );
		}
	}

	private int GetRandom()
	{
		if ( items.Count <= 1 )
			return 0;

		if ( rng != null )
			return rng.Range( 0, total ) + 1;

		if ( randomGeneratorPred != null )
			return randomGeneratorPred( total ) + 1;

		return Utility.DefaultRng.Range( 0, total ) + 1;
	}

	public T GetResult()
	{
		if ( !HasResult() )
			return default;

		if ( items.Count == 1 )
			return items[0].Item1;

		var randomVal = GetRandom();
		var resultIdx = items.BinarySearch( (default( T ), randomVal),
			Comparer<(T, int)>.Create( ( x, y ) => x.Item2 - y.Item2 ) );

		if ( resultIdx < 0 )
		{
			resultIdx = ~resultIdx;
			if ( resultIdx > items.Count )
				return default;
		}

		return items[resultIdx].Item1;
	}

	public bool HasResult()
	{
		return items.Count > 0;
	}

	private readonly List<(T, int)> items = new();
	private int total = 0;
	private readonly Func<int, int> randomGeneratorPred;
	private readonly Utility.IRandom rng;
}
