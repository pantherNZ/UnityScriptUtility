using UnityEngine;
using System;

public class FunctionComponent : MonoBehaviour
{
    public static void Create( GameObject obj, Action action )
    {
        var cmp = obj.AddComponent<FunctionComponent>();
        cmp.SetFunction( action );
    }

    public static void Create( GameObject obj, Func<bool> action )
    {
        var cmp = obj.AddComponent<FunctionComponent>();
        cmp.SetFunction( action );
    }

    public void SetFunction( Action action )
    {
        this.action = action;
    }

    public void SetFunction( Func<bool> action )
    {
        actionWithResult = action;
    }

    Action action;
    Func<bool> actionWithResult;

    void Update()
    {
        action?.Invoke();
        if( actionWithResult?.Invoke() ?? false )
            Destroy( this );
    }
}