using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class GetNextPosition : BehaviourTree.BaseNode
{
    private NPC npc;

    public GetNextPosition(string id, NPC _npc) : base(id)
    {
        npc = _npc;
    }

    public override Status Execute()
    {
        if (npc.wanderNodes.Count > 0)
        {
            var nextPos = npc.wanderNodes[Random.Range(0, npc.wanderNodes.Count)];

            // TODO[ulo]: Save in memory
            npc.nextPosition = nextPos;

            return Status.Success;
        }

        return Status.Failure;

    }

}
