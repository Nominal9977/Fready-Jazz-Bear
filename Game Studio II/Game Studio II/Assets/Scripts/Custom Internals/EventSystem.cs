using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ChannelNames;

public static class EventManager
{
    public static readonly playerEvents Player = new playerEvents();
    public static readonly NetworkEvents Network = new NetworkEvents();

    public static readonly DataRequestManager DataRequest = new DataRequestManager();
    public static readonly DataResponseManager DataResponse = new DataResponseManager();

    public class playerEvents
    {
        public class HealthEvent : UnityEvent<Component, int> { }
        public GenericEvent<HealthEvent> OnHealthChanged = new GenericEvent<HealthEvent>();
    }


    public class DataRequestManager
    {
        public class PostionRquestEvent : UnityEvent<Component, int> { }
        public GenericEvent<PostionRquestEvent> OnPostionRequest = new GenericEvent<PostionRquestEvent>();
    }

    public class DataResponseManager
    {
        public class PostionResponseEvent : UnityEvent<Transform, Vector3> { }
        public GenericEvent<PostionResponseEvent> OnPostionResponse = new GenericEvent<PostionResponseEvent>();
    }



    public class NetworkEvents
    {
        public UnityAction onConnect;
        public UnityAction onDisconnect;
    }

}

public class GenericEvent<T> where T : class, new()
{
    private Dictionary<ChannelNames, T> map = new Dictionary<ChannelNames, T>();

    public T this[ChannelNames channel]
    {
        get
        {
            map.TryAdd(channel, new T());
            return map[channel];
        }
    }
    public T Get(ChannelNames channel = default)
    {
        map.TryAdd(channel, new T());
        return map[channel];
    }
}