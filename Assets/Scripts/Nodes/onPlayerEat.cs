using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using System.Threading;

public class onPlayerEat : BehaviourTree.BaseNode
{
    private NPC npc;
    private Animator eatAnimation;
    private GameObject hungryBar;

    //external Scripts
    public MoveToPosition movePos;

    public onPlayerEat(string id, NPC _npc, Animator _eatAnimation, GameObject _hungryBar) : base(id)
    {
        npc = _npc;
        eatAnimation = _eatAnimation;
        hungryBar = _hungryBar;
    }

    public override Status Execute()
    {
        Debug.Log("Eat Running Anim.");


        if (!AnimatorIsPlaying())
        {
            Debug.Log("I finished eating, now I am full");

            // Fill the hungry Bar + Play teh animation

            Debug.Log("Fruit tag: " + npc.nextPosition.gameObject.tag);
            eatAnimation.ResetTrigger("onEat");

            // Get Fruit Tag

            switch (npc.nextPosition.gameObject.tag)
            {
                case "Apple": hungryBar.LeanScaleX(1, 3).setOnComplete(onEatFinished); break;
                case "Banana": hungryBar.LeanScaleX(0.6f, 3).setOnComplete(onEatFinished); break;
                case "Watermelon": hungryBar.LeanScaleX(0.8f, 3).setOnComplete(onEatFinished); break;
                default: break;
            }

            // disable food node during 10s
            npc.nextPosition.gameObject.SetActive(false);
            npc.nextPosition.gameObject.tag = "Recharge";
            npc.nextPosition.gameObject.name = "food" + npc.nextPosition.gameObject.tag;

            return Status.Success;
        }

        if (npc.nextPosition.gameObject.tag == "Recharge") return Status.Failure;

        // set animation trigger to player animator
        eatAnimation.SetTrigger("onEat");
        hungryBar.LeanCancel();

        return Status.Running;
        

    }

    bool AnimatorIsPlaying()
    {
        return eatAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
    }

    public void onEatFinished()
    {
        hungryBar.LeanCancel();
        LeanTween.scaleX(hungryBar, 0, 50);
    }

}
