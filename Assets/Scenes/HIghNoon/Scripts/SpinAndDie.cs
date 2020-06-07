using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAndDie : MonoBehaviour
{
    void Update()
    {
        //makes the Ghost spin in circles and float upwards until they die
        transform.eulerAngles += (new Vector3(0,2f,0));
        transform.position += new Vector3(0,0.001f,0);
    }
}
