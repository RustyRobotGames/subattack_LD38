using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrateType
{
    Torpedo, Health, Bomb
}

public class Crate : MonoBehaviour
{
    public CrateType Type = CrateType.Torpedo;
    public int Amount = 1;

    public float LifeTime = 15;

    public GameObject DestroyPrefab;

    private void Update()
    {
        LifeTime -= Time.deltaTime;
        if (LifeTime < 0)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if(Type != CrateType.Bomb)
            GetComponent<Rigidbody>().AddForce(Vector3.down * 25f);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.gameObject.tag == "Player"
                        || (other.transform.parent != null && other.transform.parent.tag == "Player");

        if(isPlayer)
        {
            
            switch (Type)
            {
                case CrateType.Torpedo:
                    AudioManager.Instance.PlayAudioClip("Collect", transform.position);
                    GameManager.Instance.Player.AddTorpedos(Amount);
                    GameManager.Instance.UIController.SpawnTorpedoInfo(Amount);
                    break;
                case CrateType.Health:
                    AudioManager.Instance.PlayAudioClip("Collect", transform.position);
                    GameManager.Instance.Player.AddHealth(Amount);
                    GameManager.Instance.UIController.SpawnHealthInfo(Amount);
                    break;
                case CrateType.Bomb:
                    AudioManager.Instance.PlayAudioClip("Boom", transform.position);
                    GameManager.Instance.Player.AddHealth(-Amount);
                    GameManager.Instance.UIController.SpawnHealthInfo(-Amount);
                    // TODO: fancy effect
                    break;
                default:
                    break;
            }

            if (DestroyPrefab != null)
                Instantiate(DestroyPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    
}
