using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class onPlayerHungry : BehaviourTree.BaseNode
{
    public GameObject hungry_bar;


    public onPlayerHungry(string id, GameObject _hungry_bar) : base(id)
    {
        hungry_bar = _hungry_bar;

    }

    public override Status Execute()
    {
        var hungry_bar_transform = hungry_bar.GetComponent<RectTransform>();

        if(hungry_bar_transform.lossyScale.x <= 0.5f)
        {
            Debug.Log("I am Hungry!!!");
            return Status.Success;
        }

        return Status.Failure;
    }
}
