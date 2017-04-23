using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aquarium : MonoBehaviour
{

    public float waterDrag = 1;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (rb.gameObject.tag == "Player")
                GameManager.Instance.Player.ControlsEnabled = true;

            rb.useGravity = false;
            rb.drag = waterDrag;
            rb.angularDrag = waterDrag;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (rb.gameObject.tag == "Player")
                GameManager.Instance.Player.ControlsEnabled = false;

            rb.useGravity = true;
            rb.drag = 0;
            rb.angularDrag = 0.05f;
        }
    }
}
