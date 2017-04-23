using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleStream : MonoBehaviour
{
    public float force = 10;

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null)
            other.attachedRigidbody.AddForce(transform.up * force);
    }

}
