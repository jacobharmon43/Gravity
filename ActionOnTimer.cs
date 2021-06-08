using System;
using UnityEngine;

public class ActionOnTimer : MonoBehaviour
{
    //Entire class exists to segment out all timers from code

    //Timer generic class
    private Action timerCall;
    private float timer;

    //Sets the time, and function to run when the time expires
    public void SetTimer(float timer, Action timerCall)
    {
        this.timer = timer;
        this.timerCall = timerCall;
    }

    //Does the timing.
    private void Update()
    {
        if(timer > 0f)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                timerCall();
            }
        }

    }
}
