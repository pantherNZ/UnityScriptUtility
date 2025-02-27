using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IEventReceiver 
{ 
	bool receivesEvents { get; set; }
	void AddSubscription( IEventSystem system );
	void RemoveSubscription( IEventSystem system );
}

public class EventReceiver : IEventReceiver
{
	bool IEventReceiver.receivesEvents { get => _receivesEvents; set => _receivesEvents = value; }
	private bool _receivesEvents;
	private List<IEventSystem> subscribedEvents = new();
	public void AddSubscription( IEventSystem system ) => subscribedEvents.Add( system );
	public void RemoveSubscription( IEventSystem system ) => subscribedEvents.Remove( system ); 
	public void UnsubscribeFromAll()
	{
		while ( !subscribedEvents.IsEmpty() )
			subscribedEvents.Last().RemoveSubscriber( this, true );
		subscribedEvents.Clear();
	}
	public EventReceiver()
	{
		_receivesEvents = true;
	}
	~EventReceiver()
	{
		UnsubscribeFromAll();
	}
}

public class MonoEventReceiver : MonoBehaviour, IEventReceiver
{
	public bool receivesEvents { get => receivesEventsOverride ?? isActiveAndEnabled; set => receivesEventsOverride = value; }
	public bool? receivesEventsOverride;
	private List<IEventSystem> subscribedEvents = new();
	public void AddSubscription( IEventSystem system ) => subscribedEvents.Add( system );
	public void RemoveSubscription( IEventSystem system ) => subscribedEvents.Remove( system );
	public void UnsubscribeFromAll()
	{
		while( !subscribedEvents.IsEmpty() )
			subscribedEvents.Last().RemoveSubscriber( this, true );
		subscribedEvents.Clear();
	}
	protected virtual void OnDestroy()
	{
		UnsubscribeFromAll();
	}
}

public interface IEvent { }
public abstract class BaseEvent<EventType> : IEvent
{
	public static void Subscribe( IEventReceiver instance, Action<EventType> callback ) 
		=> Instance.AddSubscriber( instance, callback );
	public static void Unsubscribe( IEventReceiver receiver ) 
		=> Instance.RemoveSubscriber( receiver );
	public static void Trigger( EventType eventData, IEventReceiver callerToIgnore = null )
		=> Instance.TriggerEvent( eventData, callerToIgnore );
	public static void Queue( EventType eventData, IEventReceiver callerToIgnore = null )
		=> Instance.QueueEvent( eventData, callerToIgnore );

	static EventSystem<EventType> _Instance;
	private static EventSystem<EventType> Instance
	{
		get
		{
			if ( _Instance == null )
				_Instance = new EventSystem<EventType>();
			return _Instance;
		}
	}
}

public interface IEventSystem 
{
	void RemoveSubscriber( IEventReceiver receiver, bool force = false );

	public static bool EnableLogging = true;
}

public class EventSystem<EventType> : IEventSystem
{
	private enum QueuedSubscriber
    {
        Add,
        Remove,
    }

	private class EventReceiverData
	{
		public WeakReference<IEventReceiver> receiver;
		public Action<EventType> callback;
	}

    private readonly List<EventReceiverData> receivers = new List<EventReceiverData>();
	private readonly Queue<(EventType, IEventReceiver)> queuedEvents = new Queue<(EventType, IEventReceiver)>();
	private readonly Queue<(QueuedSubscriber, EventReceiverData)> queuedSubscribers = new Queue<(QueuedSubscriber, EventReceiverData)>();
    private int iterationDepth = 0;

	private void Log( string msg )
	{
		if ( IEventSystem.EnableLogging )
		{
			Debug.Log( "[EVENT SYSTEM] " + msg );
		}
	}

	public void AddSubscriber( IEventReceiver receiver, Action<EventType> callback )
	{
		if ( !Application.isPlaying )
			return;

		CleanupNullSubscribers();

		var newReceiver = new EventReceiverData
		{
			receiver = new WeakReference<IEventReceiver>( receiver, false ),
			callback = callback,
		};

		receiver.AddSubscription( this );

		if ( iterationDepth == 0 )
		{
			if ( !receivers.Any( x => x.receiver.TryGetTarget( out var targ ) && targ == receiver ) )
			{
				receivers.Add( newReceiver );
				Log( $"Subscriber added: {receiver}" );
			}
		}
		else
		{
			queuedSubscribers.Enqueue( ( QueuedSubscriber.Add, newReceiver ) );
			Log( "Subscriber queued for add: " + receiver.ToString() );
		}
	}

	public void RemoveSubscriber( IEventReceiver receiver, bool force = false )
    {
		CleanupNullSubscribers();

		if ( force || iterationDepth == 0 )
        {
            receivers.Remove( x => !x.receiver.TryGetTarget( out var targ ) || targ == receiver );
			receiver.RemoveSubscription( this );
			Log( "Subscriber removed: " + receiver.ToString() );
		}
        else
        {
			var removeReceiver = new EventReceiverData { receiver = new WeakReference<IEventReceiver>( receiver ) };
			queuedSubscribers.Enqueue( ( QueuedSubscriber.Remove, removeReceiver ) );
			Log( "Subscriber queued for remove: " + receiver.ToString() );
		}
    }

    public void TriggerEvent( EventType e, IEventReceiver callerToIgnore = null )
    {
		Log( "Event triggered start: " + e.ToString() );

		CleanupNullSubscribers();

		iterationDepth++;

        var numRecievers = receivers.Count;
		var receiversCalled = 0; 
        foreach( var receiverData in receivers )
        {
            if( receiverData != null && 
				receiverData.receiver != null &&
				receiverData.receiver.TryGetTarget( out var receiver ) &&
				receiver != null &&
				receiver != callerToIgnore &&
				receiver.receivesEvents )
            {
				receiverData.callback( e );
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
                    TriggerEvent( queuedEvent.Item1, queuedEvent.Item2 );
                }

                if( safety >= 100 )
                    Debug.LogError( "EventSystem hit recursion limit with queue events" );

                queuedEvents.Clear();
            }

            if( queuedSubscribers.Count > 0 )
            {
                foreach( var queued in queuedSubscribers )
                {
					if ( queued.Item2.receiver.TryGetTarget( out var receiver ) )
					{
						if ( queued.Item1 == QueuedSubscriber.Add )
							AddSubscriber( receiver, queued.Item2.callback );
						else
							RemoveSubscriber( receiver );
					}
                }

                queuedSubscribers.Clear();
            }
        }
    }

	private void CleanupNullSubscribers()
	{
		var toRemove = receivers.Where( x => x == null ||
			x.receiver == null || 
			x.callback == null || 
			!x.receiver.TryGetTarget( out var receiver ) ||
			receiver == null ).ToList();

		foreach ( var remove in toRemove )
		{
			if( remove.receiver.TryGetTarget( out var targ ) )
				targ.RemoveSubscription( this );
			receivers.Remove( remove );
		}
	}

    // Waits until all current events are done processing before calling this one (will be instant if not currently iterating an event callback)
    public void QueueEvent( EventType e )
    {
        QueueEvent( e, null );
    }

    public void QueueEvent( EventType e, IEventReceiver callerToIgnore )
    {
		if ( iterationDepth == 0 )
		{
			TriggerEvent( e, callerToIgnore );
		}
		else
		{
			queuedEvents.Enqueue( ( e, callerToIgnore ) );
			Log( $"Event queued: {e}" );
		}
    }
}
