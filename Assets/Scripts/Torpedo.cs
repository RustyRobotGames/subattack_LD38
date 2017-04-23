using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TorpedoType
{
    Player, Enemy
}

public class Torpedo : MonoBehaviour
{
    public float Speed = 150;

    public TorpedoType Type { get; set; }

    Rigidbody body;

    Transform target;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.AddForce(transform.forward * Speed);
    }

    private void Update()
    {
        body.angularDrag = 0;
        body.drag = 0;
        transform.LookAt(transform.position + body.velocity);

    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            Vector3 direction = target.position - transform.position;
            body.AddForce(direction.normalized * Speed * 0.25f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && target == null)
        {
            // if we are a player torpedo and found an enemy or if we are an enemy torpedo and found a player
            if((Type == TorpedoType.Player && other.attachedRigidbody.gameObject.tag == "Enemy"))// || (Type == TorpedoType.Enemy && other.attachedRigidbody.gameObject.tag == "Player"))
                target = other.attachedRigidbody.transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.PlayAudioClip("Hit", transform.position);
        Destroy(this.gameObject);
    }

}
