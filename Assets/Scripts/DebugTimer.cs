using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimer {

    private string stageName;
    private float startTime;

    private float finalTime;

    public DebugTimer(string stageName)
    {
        this.stageName = stageName;
    }

    public void StartTimer()
    {
        startTime = Time.time;
    }

    public void StopTimer()
    {
        finalTime = Time.time - startTime;
    }

    public void PrintDebug()
    {
        Debug.Log(stageName + " took " + finalTime + " seconds.");
    }
}
