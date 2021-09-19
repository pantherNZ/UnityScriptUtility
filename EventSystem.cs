using System;
using System.Collections.Generic;

public abstract class IBaseEvent { }

public interface IEventReceiver
{
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
        foreach( var receiver in receivers )
            receiver.OnEventReceived( e );
    }

    public void TriggerEvent( IBaseEvent e, IEventReceiver caller )
    {
        foreach( var receiver in receivers )
            if( receiver != caller )
                receiver.OnEventReceived( e );
    }
}