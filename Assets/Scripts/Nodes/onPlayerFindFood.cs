using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class onPlayerFindFood : BehaviourTree.BaseNode
{
    private NPC npc;
    private float _floatMinimunNodeDistance = float.MaxValue;
    private Transform nodeToGo;
    private const float nodeFoodRechargeTime = 10.0f;


    public onPlayerFindFood(string id, NPC _npc) : base(id)
    {
        npc = _npc;
    }

    public override Status Execute()
    {
        Debug.Log("Works food Nodes");
        // There are food nodes avaidable?
        if(npc.foodNodes.Count > 0)
        {
            // Calculate minor node distance
            foreach (Transform node in npc.foodNodes)
            {
                // Recharge Food Node
                if(node.gameObject.tag == "Recharge")
                {
                    node.gameObject.SetActive(true);

                    var r = Random.Range(0, 2);

                    if (r == 0) node.gameObject.tag = "Apple";
                    else if (r == 1) node.gameObject.tag = "Banana";
                    else if (r == 2) node.gameObject.tag = "Watermelon";
                    //node.gameObject.tag = (Random.Range(0, 2)) == 1 ? "Apple" : "Banana";
                    node.gameObject.name = "Food - " + node.gameObject.tag;
                }

                // Actual minimun distance to food node
                var _floatNodeDistance = Vector3.Distance(node.position, npc.transform.position);

                if (_floatNodeDistance <= _floatMinimunNodeDistance)
                {
                    nodeToGo = node;
                    _floatMinimunNodeDistance = _floatNodeDistance;

                }

            }

            _floatMinimunNodeDistance = float.MaxValue;

            //Food Node Point

            Debug.Log("Hey Look, some food there!: " + nodeToGo.position);
            npc.nextPosition = nodeToGo;

            return Status.Success;
        }
        else { return Status.Failure; }
    }
}
