using UnityEngine;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using System;
using System.Runtime.InteropServices;



public class FMODBeatCounter : MonoBehaviour
{
    private volatile bool beatHit;

    public EventReference musicEvent;

    public TMP_Text beatText;

    private EventInstance eventInstance;
    private int beatCount = 0;

    // Keep delegate alive
    private EVENT_CALLBACK beatCallback;

    void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(musicEvent);

        GCHandle handle = GCHandle.Alloc(this);
        eventInstance.setUserData(GCHandle.ToIntPtr(handle));

        beatCallback = new EVENT_CALLBACK(OnFMODEventCallback);
        eventInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

        eventInstance.start();
    }

    void OnDestroy()
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance.release();
        eventInstance.getUserData(out IntPtr ptr);
        if (ptr != IntPtr.Zero)
        {
            GCHandle.FromIntPtr(ptr).Free();
        }
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static RESULT OnFMODEventCallback(
        EVENT_CALLBACK_TYPE type,
        IntPtr instancePtr,
        IntPtr parameterPtr)
    {
        if (type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            var instance = new EventInstance(instancePtr);

            instance.getUserData(out IntPtr userDataPtr);
            if (userDataPtr != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(userDataPtr);
                FMODBeatCounter counter = handle.Target as FMODBeatCounter;
                counter?.OnBeat();
            }
        }

        return RESULT.OK;
    }

    private void OnBeat()
    {
        beatCount++;

        if (beatText != null)
            beatText.text = beatCount.ToString();

    }
    void Update()
    {
        if (beatHit)
        {
            beatHit = false;
            beatCount++;

            if (beatText != null)
                beatText.text = beatCount.ToString();
        }
    }

}