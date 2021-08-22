using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{
    public float gravity;
    public float gravityRadius;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerLocomotion>())
        {
            other.GetComponent<PlayerLocomotion>().currentGravity = GetComponent<GravityOrbit>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerLocomotion>())
        {
            other.GetComponent<PlayerLocomotion>().currentGravity = null;
        }
    }

}
