using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InstancedAction : MonoBehaviour
{

    private static InstancedAction mInstance;
    public static InstancedAction Instance
    {
        get
        {
            if (!mInstance)
            {
                var dummy = new GameObject("Instance");
                mInstance = dummy.AddComponent<InstancedAction>();
            }
            return mInstance;
        }
    }

    private static UnityEvent StaticUpdate = new UnityEvent();

    public static void RegisterUpdate(UnityAction act)
    {
        StaticUpdate.AddListener(act);
    }

    public static void UnRegisterUpdate(UnityAction act)
    {
        StaticUpdate.RemoveListener(act);
    }

    private void Update()
    {
        StaticUpdate.Invoke();
    }

    private static int Counter = 0;

    public static TimeDelayer DelayAction(UnityAction act, float time)
    {
        TimeDelayer delayer = new TimeDelayer(time, act);
        return delayer;
    }

    public static void TickAction(UnityAction act, int ticks)
    {
        Instance.StartCoroutine(TickCO(act, ticks));
    }

    public static IEnumerator TickCO(UnityAction act, int ticks)
    {
        var target = ticks + Counter;
        while (Counter < target)
        {
            yield return null;
        }
        act.Invoke();
    }

    public static Coroutine DoRoutine(IEnumerator Co)
    {
        var routine = Instance.StartCoroutine(Co);
        return routine;
    }

    public static int Tick()
    {
        Counter += 1;
        return Counter;
    }
}

public class TimeDelayer
{
    public TimeDelayer(float mtotal, UnityAction action)
    {
        totalTime = mtotal;
        startTime = Time.time;
        act = action;
        delayer = InstancedAction.DoRoutine(DelayCO(action, mtotal));
    }

    public Coroutine delayer;
    public float totalTime;
    private float elapsedOnPause;
    public float startTime;

    private UnityAction act;
    public bool paused = false;
    public bool finished = false;

    public void Pause()
    {
        if (paused)
        {
            return;
        }
        paused = true;
        elapsedOnPause = Time.time - startTime;
        InstancedAction.Instance.StopCoroutine(delayer);
    }

    public void Resume()
    {
        if (!paused)
        {
            return;
        }
        paused = false;
        var delayed = InstancedAction.DelayAction(act, totalTime - elapsedOnPause);
        delayer = delayed.delayer;
    }

    public void Pause(float time)
    {
        if (paused)
        {
            return;
        }
        Pause();
        InstancedAction.DelayAction(Resume, time);
    }

    public void End()
    {
        InstancedAction.Instance.StopCoroutine(delayer);
        act.Invoke();
    }

    public void Kill()
    {
        InstancedAction.Instance.StopCoroutine(delayer);
    }

    IEnumerator DelayCO(UnityAction act, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        finished = true;
        act.Invoke();
    }
}
