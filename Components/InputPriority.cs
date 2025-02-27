using System;
using System.Collections.Generic;
using UnityEngine;

public class InputPriority : MonoBehaviour
{
    static InputPriority _Instance;
    static public InputPriority Instance
    {
        get { return _Instance; }
        private set { }
    }

    private readonly Dictionary<string, Pair<int, Action>> entries = new Dictionary<string, Pair<int, Action>>();

    private void Start()
    {
        _Instance = this;
    }

    public void Request( Func<bool> inputRequest, string key, int priority, Action func )
    {
        if( inputRequest() )
        {
            if( entries.TryGetValue( key, out Pair<int, Action> found ) )
            {
                if( found.First < priority )
                    entries[key] = new Pair<int, Action>( priority, func );
            }
            else
            {
                entries.Add( key, new Pair<int, Action>( priority, func ) );
            }
        }
    }

    public void Cancel( string key )
    {
        Request( () => true, key, int.MaxValue, null );
    }

    private void LateUpdate()
    {
        foreach( var entry in entries.Values )
        {
            entry.Second?.Invoke();
        }

        entries.Clear();
    }
}
