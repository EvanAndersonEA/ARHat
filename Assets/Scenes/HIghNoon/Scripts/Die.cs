using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    public float timeTillDeath;
    void Awake()
    {
        //starts a cotoutine that destroys the gameobject after waiting for an ammount of time
        StartCoroutine(timetilldeath(timeTillDeath));
    }


    IEnumerator timetilldeath(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
