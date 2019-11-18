using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSystem
{

    public static bool Normal = true;

    public static void Stop(float duration)
    {
        Time.timeScale = 0.0f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        Normal = false;
        InstancedAction.DelayAction(() =>
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                Normal = true;
            }, duration);
    }

    public static void ChangeScale(float scale, float duration)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        Normal = false;
        InstancedAction.DelayAction(() =>
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Normal = true;
        }, duration);
    }

}
