using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RateLimiter
{
    private readonly int maxCalls;
    private readonly TimeSpan timeFrame;
    private readonly List<DateTime> calls = new List<DateTime>();

    public RateLimiter( int maxCalls, TimeSpan timeFrame )
    {
        this.maxCalls = maxCalls;
        this.timeFrame = timeFrame;
    }

    public bool CheckLimit()
    {
        var currentTime = DateTime.Now;
        calls.RemoveAll( x => x < currentTime.Subtract( timeFrame ) );
        return calls.Count < maxCalls;
    }

    public bool AttemptCall()
    {
        if( !CheckLimit() )
            return false;

        calls.Add( DateTime.Now );
        return true;
    }

    private TimeSpan GetWaitTimeSpan()
    {
        return calls.Front().Add( timeFrame ).Subtract( DateTime.Now );
    }

    public IEnumerator WaitForCall( IEnumerator action )
    {
        if( !CheckLimit() )
        {
            yield return new WaitForSeconds( GetWaitTimeSpan().Seconds );
        }

        calls.Add( DateTime.Now );
        yield return action;
    }

    public async Task WaitForCallAsync()
    {
        if( !CheckLimit() )
        {
            await Task.Delay( GetWaitTimeSpan() );
        }

        calls.Add( DateTime.Now );
    }
}