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

        private Dictionary<Func<KeyCode, bool>, Pair<int, Action>> entries = new Dictionary<Func<KeyCode, bool>, Pair<int, Action>>();

        public void Request( Func<KeyCode, bool> inputRequest, KeyCode code, int priority, Action func )
        {
            if( inputRequest( code ) )
            {
                if( entries.TryGetValue( inputRequest, out Pair<int, Action> found ) && found.First < priority )
                {
                    entries[inputRequest] = new Pair<int, Action>( priority, func );
                }
                else
                {
                    entries.Add( inputRequest, new Pair<int, Action>( priority, func ) );
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