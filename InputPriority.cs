using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static partial class Utility
{
    public class InputPriority
    {
        static InputPriority _Instance;
        static public InputPriority Instance
        {
            get
            {
                if( _Instance == null )
                    _Instance = new InputPriority();
                return _Instance;
            }
            private set { }
        }

        private readonly Dictionary<string, Pair<int, Action>> entries = new();

        public void Request( Func<bool> inputRequest, string key, int priority, Action func )
        {
            if( inputRequest() )
            {
                if( entries.TryGetValue( key, out Pair<int, Action> found ) && found.First < priority )
                {
                    entries[key] = new Pair<int, Action>( priority, func );
                }
                else
                {
                    entries.Add( key, new Pair<int, Action>( priority, func ) );
                }
            }
        }

        private void LateUpdate()
        {
            foreach( var entry in entries.Values )
            {
                entry.Second();
            }

            entries.Clear();
        }
    }
}