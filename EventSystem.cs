using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBaseEvent { }

public interface IEventReceiver
{
    public abstract void OnEventReceived( IBaseEvent e );
}

public abstract class EventReceiverInstance : MonoBehaviour, IEventReceiver
{
    protected virtual void Start()
    {
        EventSystem.Instance.AddSubscriber( this );
    }

    protected virtual void OnDisable()
    {
        EventSystem.Instance.RemoveSubscriber( this );
    }

    public abstract void OnEventReceived( IBaseEvent e );
}

public class EventSystem
{
    static EventSystem _Instance;
    static public EventSystem Instance
    {
        get
        {
            if( _Instance == null )
                _Instance = new EventSystem();

           return _Instance;
        }

        private set { }
    }

    private readonly List<IEventReceiver> receivers = new List<IEventReceiver>();
    private readonly Queue<Pair<IBaseEvent, IEventReceiver>> queuedEvents = new Queue<Pair<IBaseEvent, IEventReceiver>>();
    private int iterationDepth = 0;

    public void AddSubscriber( IEventReceiver receiver )
    {
        if( !receivers.Contains( receiver ) )
            receivers.Add( receiver );
    }

    public void RemoveSubscriber( IEventReceiver receiver )
    {
        receivers.Remove( receiver );
    }

    public void TriggerEvent( IBaseEvent e )
    {
        TriggerEvent( e, null );
    }

    public void TriggerEvent( IBaseEvent e, IEventReceiver callerToIgnore )
    {
        iterationDepth++;

        foreach( var receiver in receivers )
            if( receiver != null && receiver != callerToIgnore )
                receiver.OnEventReceived( e );

        iterationDepth--;

        if( iterationDepth == 0 && queuedEvents.Count > 0 )
        {
            int safety = 0;
            while( queuedEvents.Count > 0 && safety++ <= 100 )
            {
                var queuedEvent = queuedEvents.Dequeue();
                TriggerEvent( queuedEvent.First, queuedEvent.Second );
            }

            if( safety >= 100 )
                Debug.LogError( "EventSystem hit recursion limit with queue events" );

            queuedEvents.Clear();
        }
    }

    // Waits until all current events are done processing before calling this one (will be instant if not currently iterating an event callback)
    public void QueueEvent( IBaseEvent e )
    {
        QueueEvent( e, null );
    }

    public void QueueEvent( IBaseEvent e, IEventReceiver callerToIgnore )
    {
        if( iterationDepth == 0 )
            TriggerEvent( e, callerToIgnore );
        else
            queuedEvents.Enqueue( new Pair<IBaseEvent, IEventReceiver>( e, callerToIgnore ) );
    }
}