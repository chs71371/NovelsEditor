using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineReceiver : MonoBehaviour
{
    public void OnCallPause() 
    {
        NovelsManager.Instance.PauseTimeLine();
    }
}
