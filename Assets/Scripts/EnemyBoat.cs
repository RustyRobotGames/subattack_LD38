using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoat : MonoBehaviour
{
    public float Speed = 600;
    public float Health = 2;
    Rigidbody body;

    public Transform TorpedoSpawn;
    public GameObject TorpedoPrefab;

    public float shootRange = 15;
    public float followRange = 10;

    float torpedoTimer = 0;

    void Start ()
    {
        body = GetComponent<Rigidbody>();
        GameManager.Instance.EnemyCount++;
	}
	
	void FixedUpdate ()
    {
        // player is dead, we won!
        if (GameManager.Instance.Player == null || GameManager.Instance.Player.Health <= 0)
            return;

        var playerPosition = GameManager.Instance.Player.transform.position;
        
        Vector3 direction = playerPosition - transform.position;
        float distance = direction.magnitude;

        if (distance < shootRange)
            FireTorpedo();

        if(distance > followRange)
            body.AddForce(direction.normalized * Speed * Time.fixedDeltaTime);

        // make shure y position will be the same at some point to make targeting easier for both factions
        var yDiff = playerPosition.y - transform.position.y;
        //body.AddForce(Vector3.up * yDiff * 10);

        torpedoTimer -= Time.fixedDeltaTime;
        transform.LookAt(playerPosition);
    }

    void FireTorpedo()
    {
        if(torpedoTimer < 0)
        {
            torpedoTimer = Random.Range(2, 10);
            var go = Instantiate(TorpedoPrefab, TorpedoSpawn.position, transform.rotation);
            go.GetComponent<Rigidbody>().AddForce(transform.forward * 500);
            go.GetComponent<Torpedo>().Type = TorpedoType.Enemy;

            AudioManager.Instance.PlayAudioClip("FireTorpedo", TorpedoSpawn.position);
        }
    }

    public void Kill()
    {
        GameManager.Instance.EnemyCount--;
        var bubbles = GetComponentInChildren<ParticleSystem>();
        Destroy(bubbles);

        body.AddTorque(Random.onUnitSphere * 150);
        body.AddTorque(transform.forward * 200);

        enabled = false; // stop doing anything
        body.useGravity = true; // sink?

        transform.Rotate(0, 0, 90);

        //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.rigidbody != null && collision.rigidbody.gameObject.tag == "Torpedo")
        {
            var torpedo = collision.rigidbody.gameObject.GetComponent<Torpedo>();
            if (torpedo.Type == TorpedoType.Enemy) // disable friendly fire
                return;

            Health --;
            if(Health >= 0)
                GameManager.Instance.UIController.SpawnHealthInfo(-1, torpedo.transform);
            AudioManager.Instance.PlayAudioClip("Hit", transform.position);
            if (Health <= 0)
            {
                Kill();
            }
        }
    }
}
