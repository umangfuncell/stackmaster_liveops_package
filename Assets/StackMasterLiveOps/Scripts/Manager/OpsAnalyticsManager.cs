using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpsAnalyticsManager : MonoBehaviour
{
    public static OpsAnalyticsManager Instance;
    public void InIt()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    public void LogEvent(string EventName)
    {
        //write code of actul log event
    }
}
