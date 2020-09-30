using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleCharacterControlFree))]
public class UserInput : MonoBehaviour
{
    SimpleCharacterControlFree target;

    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<SimpleCharacterControlFree>();
    }

    // Update is called once per frame
    void Update()
    {
        target.VerticalInput = Input.GetAxis("Vertical");
        target.HorizontalInput = Input.GetAxis("Horizontal");
    }
}
