using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class Wait : BaseNode
{
    private float waitingTime = 0;

    private bool started = false;
    private float lastTime = 0;

    public Wait(string id, float t) : base(id)
    {
        waitingTime = t / 1000.0f;
        //Debug.Log("Wait: " + t);
    }

    public override Status Execute()
    {
        if (!started)
        {
            started = true;
            lastTime = Time.time;

            return Status.Running;
        }

        var wt = Time.time - lastTime;
        if (wt > waitingTime)
        {
            started = false;
            return Status.Success;
        }

       // Debug.Log("Time waiting: " + wt);

        return Status.Running;
    }

    public override void Reset()
    {
        started = false;
        lastTime = 0;

        base.Reset();
    }
}
