using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class isFoodAvailable : BehaviourTree.BaseNode
{
    private NPC npc;

    public isFoodAvailable(string id, NPC _npc) : base(id)
    {
        npc = _npc;
    }

    public override Status Execute()
    {

        return Status.Failure;
        //if (npc.foodNodes.Count > 0) return Status.Success;

    }
}
