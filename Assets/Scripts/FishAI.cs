using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour
{
    Rigidbody body;
    public float moveTimer = 0;

    public BoxCollider gameBounds;
    Vector3 destination;

    bool iAmEnabled = true;
    bool canBeHooked = true;

    void Start ()
    {
        body = GetComponent<Rigidbody>();
        destination = transform.position;

        GameManager.Instance.RegisterFish(this);
	}
	
	void Update ()
    {
        if (!iAmEnabled)
            return;

        float distanceToTarget = (destination - transform.position).magnitude;
		if(moveTimer < 0)
        {
            moveTimer = Random.Range(2, 12);

            float o = 5;
            destination = new Vector3(
                Random.Range(gameBounds.bounds.min.x + o, gameBounds.bounds.max.x - o),
                Random.Range(gameBounds.bounds.min.y + o, gameBounds.bounds.max.y - o),
                Random.Range(gameBounds.bounds.min.z + o, gameBounds.bounds.max.z - o)
                );

            var direction = transform.position - destination;
            float dist = direction.magnitude;
            direction.Normalize();
            RaycastHit hit;
            if(Physics.Raycast(new Ray(transform.position, direction), out hit, dist))
            {
                destination = hit.point - direction * 5;
            }
        }

        moveTimer -= Time.deltaTime;
        //Debug.Log(destination + "    " + moveTimer);
        transform.LookAt( transform.position + body.velocity);

        Debug.DrawLine(transform.position, destination, Color.red);
    }

    public void SetEnabled(bool enabled)
    {
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var c in colliders)
        {
            c.enabled = enabled;
        }

        iAmEnabled = enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "arm" && canBeHooked && !GameManager.Instance.Player.HasFishHookedUp)
        {
            Debug.Log("Hook me up!");
            canBeHooked = false;
            GameManager.Instance.Player.HookUpFish(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "arm")
            canBeHooked = true;
    }

    private void FixedUpdate()
    {
        var direction = destination - transform.position;
        if(direction.magnitude > 2)
            body.AddForce(direction.normalized * 3);
    }
}
