using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpsHapticManger : MonoBehaviour
{
    public static OpsHapticManger Instance;
    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void OnDestroy()
    {
        Instance = null;
    }
    public void MediumHapticTouch()
    {

    }
}
