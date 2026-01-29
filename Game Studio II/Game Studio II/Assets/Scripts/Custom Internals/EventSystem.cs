using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static ChannelNames;

public static class EventManager
{
    public static readonly playerEvents cPlayer = new playerEvents();
    public static readonly NetworkEvents cNetwork = new NetworkEvents();

    public static readonly DataRequestManager cDataRequest = new DataRequestManager();
    public static readonly DataResponseManager cDataResponse = new DataResponseManager();

    public class playerEvents
    {
        public class HealthEvent : UnityEvent<Component, int> { }
        public GenericEvent<HealthEvent> eOnHealthChanged = new GenericEvent<HealthEvent>();
    }


    public class DataRequestManager
    {
        public class PostionRquestEvent : UnityEvent<Component, int> { }
        public GenericEvent<PostionRquestEvent> eOnPostionRequest = new GenericEvent<PostionRquestEvent>();
    }

    public class DataResponseManager
    {
        public class PostionResponseEvent : UnityEvent<Transform, Vector3> { }
        public GenericEvent<PostionResponseEvent> eOnPostionResponse = new GenericEvent<PostionResponseEvent>();
    }



    public class NetworkEvents
    {
        public UnityAction onConnect;
        public UnityAction onDisconnect;
    }

}

public class GenericEvent<T> where T : class, new()
{
    private Dictionary<ChannelNames, T> mMap = new Dictionary<ChannelNames, T>();

    public T this[ChannelNames channel]
    {
        get
        {
            mMap.TryAdd(channel, new T());
            return mMap[channel];
        }
    }
    public T Get(ChannelNames channel = Default)
    {
        mMap.TryAdd(channel, new T());
        return mMap[channel];
    }
}