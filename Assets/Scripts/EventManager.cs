using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDict;
    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if(!eventManager)
            {
                eventManager = FindObjectOfType<EventManager>() as EventManager;
                if(!eventManager)
                {
                    Debug.LogError("There needs to be one active Eventmanager script on a gamebject in your scene.");

                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    private void Init()
    {
        if (eventDict == null)
        {
            eventDict = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {

        if (instance.eventDict.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDict.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    { 
        if(eventManager == null) { return; }

        if (instance.eventDict.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }   

    public static void TriggerEvent(string eventName)
    {

        if (instance.eventDict.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            Debug.Log("event: " + eventName);
            thisEvent.Invoke();
        }
    }
}
