using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEngine;

public class MoveToPosition : BehaviourTree.BaseNode
{
    private NPC npc;

    public MoveToPosition(string id, NPC _npc) : base(id)
    {
        npc = _npc;
    }

    public override Status Execute()
    {
        if (npc.nextPosition != null)
        {

            npc.agent.SetDestination(npc.nextPosition.position);
            npc.GetComponent<Animator>().SetFloat("MoveSpeed", npc.agent.velocity.magnitude);

            var distance = npc.nextPosition.position - npc.transform.position;
            //Debug.Log(distance);
            distance.y = 0;

            if (distance.sqrMagnitude < 0.5f*0.5f)
            {
                npc.GetComponent<Animator>().SetFloat("MoveSpeed", 0);
                Debug.Log("I get to the node!");
                return Status.Success;
            }

            return Status.Running;
        }

        return Status.Failure;
    }
}
