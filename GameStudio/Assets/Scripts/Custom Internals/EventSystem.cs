using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using static ChannelNames;

public static class EventManager
{
    public static readonly playerEvents cPlayer = new playerEvents();
    public static readonly BossEvents cBoss = new BossEvents();


    //General Events so that each channels doesnt have things like heatlh and postion events repeatedly
    public class UniveralUnityEvents
    {
        public class Postion : UnityEvent<Component, Vector3> { }
        public class HealthEvent : UnityEvent<Component, int> { }
    }

    public class BossEvents
    {
        public GenericEvent<UniveralUnityEvents.Postion> ePostionChange = new GenericEvent<UniveralUnityEvents.Postion>();
    }

    public class CammeraEvents
    {
        public GenericEvent<UniveralUnityEvents.Postion> ePostionChange = new GenericEvent<UniveralUnityEvents.Postion>();
    }

    public class CroudEvents
    {
        // figure we want a channel no idea what should go here yet
    }

    public class playerEvents
    {
        public GenericEvent<UniveralUnityEvents.HealthEvent> eOnHealthChanged = new GenericEvent<UniveralUnityEvents.HealthEvent>();
        public GenericEvent<UniveralUnityEvents.Postion> ePostionChange = new GenericEvent<UniveralUnityEvents.Postion>();
    }

    public class ScoreEvents
    {
        // figure we want a channel no idea what should go here yet
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