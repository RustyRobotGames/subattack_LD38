using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const int MAX_TORPEDOS = 25;
    const int MAX_HEALTH = 20;

    Rigidbody body;
    Vector3 force;
    float rotateForce; 

    [Header("Movement")]
    public float Speed = 5;
    public float RotateSpeed = 2;
    public float BoosMultiplier = 3;

    [Header("Health")]
    public int Health = 100;

    [Header("Shoot stuff")]
    public int TorpedoCount = 15;
    public Transform TorpedoSpawn;
    public GameObject TorpedoPrefab;

    [Header("Engine Audio")]
    public float idlePitch = 0.7f;
    public float activePich = 1f;


    FishAI currentFish = null;
    public bool HasFishHookedUp { get { return currentFish != null; } }
    public bool CanRescueFish { get; set; }

    public bool ControlsEnabled { get; set; }

    AudioSource engineSound;

    Vector3 initialPosition;
    Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        ControlsEnabled = false;
        body = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
        engineSound.enabled = false;
        engineSound.pitch = idlePitch;
    }

    public void ResetState()
    {
        Health = 20;
        TorpedoCount = 25;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        Start();
    }

    void Update()
    {
        Vector3 velocity = Vector3.zero;
        force = Vector3.zero;

        engineSound.enabled = ControlsEnabled;

        if (ControlsEnabled)
        {
            if (Input.GetKey(KeyCode.W)) velocity.x += 1;
            if (Input.GetKey(KeyCode.S)) velocity.x -= 1;
            if (Input.GetKey(KeyCode.E)) velocity.y += 1;
            if (Input.GetKey(KeyCode.Q)) velocity.y -= 1;

            rotateForce = 0;
            if (Input.GetKey(KeyCode.D)) rotateForce = 1;
            if (Input.GetKey(KeyCode.A)) rotateForce = -1;

            force = transform.forward * velocity.x + transform.up * velocity.y;

            if (Input.GetKey(KeyCode.LeftShift))
                force *= BoosMultiplier;

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonDown(0))
                FireTorpedo();
        }

        transform.Rotate(new Vector3(0, rotateForce * RotateSpeed, 0));

        var currentRot = transform.rotation;
        var targetRot = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime);

        if (currentFish != null)
        {
            currentFish.transform.position = this.transform.position + new Vector3(0, -1.5f, 0);
            currentFish.transform.rotation = this.transform.rotation;
        }

        UpdateEngineSound(velocity);
    }

    private void UpdateEngineSound(Vector3 velocity)
    {
        if (velocity.x == 0)
            engineSound.pitch = idlePitch;
        else
            engineSound.pitch = activePich;
    }

    void FireTorpedo()
    {
        if(TorpedoCount > 0)
        {
            TorpedoCount--;
            var go = Instantiate(TorpedoPrefab, TorpedoSpawn.position, transform.rotation);
            AudioManager.Instance.PlayAudioClip("FireTorpedo", TorpedoSpawn.position);
            go.GetComponent<Rigidbody>().AddForce(transform.forward * 200);
            go.GetComponent<Torpedo>().Type = TorpedoType.Player;
        }
    }

    public void ReleaseFish()
    {
        if(currentFish != null)
        {
            if (CanRescueFish)
            {
                GameManager.Instance.RescueFish(currentFish);
            }

            if(currentFish != null) // WTF??
            {
                currentFish.moveTimer = 5;
                currentFish.SetEnabled(true);
                currentFish = null;
            }
            
        }

        GameManager.Instance.RescueHelper.SetBlinkingEnabled(false);
    }

    public void HookUpFish(FishAI fish)
    {
        if(currentFish == null)
            AudioManager.Instance.PlayAudioClip("Pickfish", transform.position);

        currentFish = fish;
        var fishGO = currentFish.gameObject;
    
        currentFish.SetEnabled(false);
        fish.GetComponent<Rigidbody>().velocity = Vector3.zero;

        GameManager.Instance.RescueHelper.SetBlinkingEnabled(true);
    }

    private void FixedUpdate()
    {
        body.AddForce(force * Speed * Time.fixedDeltaTime);
        //transform.rotation = Quaternion.Euler(0, body.transform.rotation.y, 0);

        // FIXME: 
        //transform.LookAt(transform.position + transform.forward + new Vector3(0,body.velocity.y,0));
    }

    public void AddTorpedos(int amount)
    {
        TorpedoCount += amount;
        TorpedoCount = Mathf.Clamp(TorpedoCount, 0, MAX_TORPEDOS);
    }

    public void AddHealth(int amount)
    {
        if (amount < 0)
            ReleaseFish();

        Health += amount;
        Health = Mathf.Clamp(Health, 0, MAX_HEALTH);
        if (Health <= 0)
            Die();
    }

    void Die()
    {
        GameManager.Instance.HandleGameOver();

        var bubbles = GetComponentInChildren<ParticleSystem>();
        Destroy(bubbles);

        body.AddTorque(Random.onUnitSphere * 150);
        body.AddTorque(transform.forward * 200);

        body.useGravity = true; // sink?

        engineSound.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && collision.rigidbody.gameObject.tag == "Torpedo")
        {
            var torpedo = collision.rigidbody.gameObject.GetComponent<Torpedo>();
            if (torpedo.Type == TorpedoType.Player) // disable friendly fire
                return;

            Health --;
            GameManager.Instance.UIController.SpawnHealthInfo(-1);
            ReleaseFish();
            AudioManager.Instance.PlayAudioClip("Hit", transform.position);
            if (Health <= 0)
            {
                Die();
            }
        }
    }

}
