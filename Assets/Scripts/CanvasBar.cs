using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DentedPixel;

public class CanvasBar : MonoBehaviour
{

    public GameObject hungry_bar;
    public int time;
    //private bool onSet = true;

    // Start is called before the first frame update
    void Start()
    {
        AnimateBar();
    }    

    public void AnimateBar()
    {
        LeanTween.scaleX(hungry_bar, 0, time).setOnComplete(onGameOver);
    }

    public void onGameOver()
    {
        Time.timeScale = 0;
    }

    //public bool BarOnSet()
    //{
    //    return onSet;
    //}
}
