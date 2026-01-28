using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpsAdManager : MonoBehaviour
{
    public static OpsAdManager Instance;
    public void InIt()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void ShowRewardedAd(Action onSuccess, Action onFail, string eventName)
    {
        // call rewarded ad
    }
    void OnDestroy()
    {
        Instance = null;
    }
}
