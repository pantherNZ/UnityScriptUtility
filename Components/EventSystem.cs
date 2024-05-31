using System;
using System.Collections.Generic;
using System.Linq;
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

    protected virtual void OnEnable()
    {
        if( modifyListenerWithEnableDisable )
            EventSystem.Instance.AddSubscriber( this );
    }

    protected virtual void OnDisable()
    {
        if( modifyListenerWithEnableDisable )
            EventSystem.Instance.RemoveSubscriber( this );
    }

    public abstract void OnEventReceived( IBaseEvent e );

    public bool modifyListenerWithEnableDisable = true;

	// TODO
	//public bool enableDebugLogging = false;
}

public class EventSystem
{
    static EventSystem _Instance;
	public static EventSystem Instance
    {
        get
        {
            if( _Instance == null )
                _Instance = new EventSystem();
           return _Instance;
        }

        private set { }
    }
	public static bool EnableLogging = true;

    private enum QueuedSubscriber
    {
        Add,
        Remove,
    }

	private class EventReceiverData
	{
		public IEventReceiver receiver;
		public Type[] eventTypes;
	}

    private readonly List<EventReceiverData> receivers = new List<EventReceiverData>();
    private readonly Queue<Pair<IBaseEvent, IEventReceiver>> queuedEvents = new Queue<Pair<IBaseEvent, IEventReceiver>>();
    private readonly Queue<Pair<QueuedSubscriber, EventReceiverData>> queuedSubscribers = new Queue<Pair<QueuedSubscriber, EventReceiverData>>();
    private int iterationDepth = 0;

	private void Log( string msg )
	{
		if ( EnableLogging )
		{
			Debug.Log( "[EVENT SYSTEM] " + msg );
		}
	}

	public void AddSubscriber( IEventReceiver receiver )
	{
		AddSubscriber( receiver, null );
	}

	public void AddSubscriber( IEventReceiver receiver, params Type[] eventTypes )
	{
		var newReceiver = new EventReceiverData 
		{
			receiver = receiver,
			eventTypes = eventTypes,
		};

		if ( iterationDepth == 0 )
		{
			if ( !receivers.Any( x => x.receiver == receiver ) )
			{
				receivers.Add( newReceiver );
				var eventTypesInfo = eventTypes != null ? $", event types: {string.Join( ", ", eventTypes.Select( x => x.ToString() ) )}" : string.Empty;
				Log( $"Subscriber added: {receiver}${eventTypesInfo}" );
			}
		}
		else
		{
			queuedSubscribers.Enqueue( new Pair<QueuedSubscriber, EventReceiverData>( QueuedSubscriber.Add, newReceiver ) );
			Log( "Subscriber queued for add: " + receiver.ToString() );
		}
	}

	public void RemoveSubscriber( IEventReceiver receiver )
    {
        if( iterationDepth == 0 )
        {
            receivers.Remove( x => x.receiver == receiver );
			Log( "Subscriber removed: " + receiver.ToString() );
		}
        else
        {
			var removeReceiver = new EventReceiverData { receiver = receiver };
			queuedSubscribers.Enqueue( new Pair<QueuedSubscriber, EventReceiverData>( QueuedSubscriber.Remove, removeReceiver ) );
			Log( "Subscriber queued for remove: " + receiver.ToString() );
		}
    }

    public void TriggerEvent( IBaseEvent e )
    {
        TriggerEvent( e, null );
    }

    public void TriggerEvent( IBaseEvent e, IEventReceiver callerToIgnore )
    {
		Log( "Event triggered start: " + e.ToString() );

		iterationDepth++;

        var numRecievers = receivers.Count;
		var receiversCalled = 0; 
        foreach( var receiverData in receivers )
        {
            if( receiverData != null && receiverData != callerToIgnore &&
				( receiverData.eventTypes == null || receiverData.eventTypes.IsEmpty() || receiverData.eventTypes.Contains( e.GetType() ) ) )
            {
				receiverData.receiver.OnEventReceived( e );
				++receiversCalled;
				Debug.Assert( numRecievers == receivers.Count );
            }
        }

		Log( $"Event triggered complete: {e}, receivers called: {receiversCalled}" );

		iterationDepth--;

        if( iterationDepth == 0 )
        {
            if( queuedEvents.Count > 0 )
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

            if( queuedSubscribers.Count > 0 )
            {
                foreach( var queued in queuedSubscribers )
                {
                    if( queued.First == QueuedSubscriber.Add )
                        AddSubscriber( queued.Second.receiver, queued.Second.eventTypes );
                    else
                        RemoveSubscriber( queued.Second.receiver );
                }

                queuedSubscribers.Clear();
            }
        }
    }

    // Waits until all current events are done processing before calling this one (will be instant if not currently iterating an event callback)
    public void QueueEvent( IBaseEvent e )
    {
        QueueEvent( e, null );
    }

    public void QueueEvent( IBaseEvent e, IEventReceiver callerToIgnore )
    {
		if ( iterationDepth == 0 )
		{
			TriggerEvent( e, callerToIgnore );
		}
		else
		{
			queuedEvents.Enqueue( new Pair<IBaseEvent, IEventReceiver>( e, callerToIgnore ) );
			Log( $"Event queued: {e}" );
		}
    }
}
