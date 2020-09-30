using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodRecharge : MonoBehaviour
{

    Transform foodNode;
    // Start is called before the first frame update
    void Start()
    {
        foodNode = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (foodNode.gameObject.tag == "Recharge")
        //    StartCoroutine(onFoodRecharge());
        //}
    }

    public IEnumerator onFoodRecharge()
    {
        Debug.Log("Food Coroutine starts");
        yield return new WaitForSeconds(3);
        Debug.Log("Food Coroutine ends");
    }
}
