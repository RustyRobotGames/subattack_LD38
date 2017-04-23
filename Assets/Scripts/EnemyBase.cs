using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public Transform DoorTransform;
    public float DoorAnimationTime = 2;
    public AnimationCurve DoorAnimationCurve;

    public float RandomWaitTime = 2;

    float timer = 0;

    public GameObject EnemyPrefab;
    public Transform spawnPoint;

    List<EnemyBoat> spawnedEnemies = new List<EnemyBoat>();
    public List<EnemyBoat> Enemies { get { return spawnedEnemies; } }

    public void StartSpawning()
    {
        Cleanup();
        StartCoroutine(OpenDoor());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    void Cleanup()
    {
        Debug.Log("Cleaning up: have " + spawnedEnemies.Count);
        for (int i = 0; i < spawnedEnemies.Count; i++)
            Destroy(spawnedEnemies[i].gameObject);

        spawnedEnemies.Clear();
    }

    IEnumerator OpenDoor()
    {
        timer = 0;

        yield return new WaitForSeconds(Random.Range(0, RandomWaitTime));
          
        while(timer < DoorAnimationTime)
        {
            timer += Time.deltaTime;
            float p = timer / DoorAnimationTime;
            float v = DoorAnimationCurve.Evaluate(p);

            float angle = Mathf.Lerp(0, 110, v);

            DoorTransform.localRotation = Quaternion.Euler(angle,0,0);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        timer = 0;
        if (GameManager.Instance.CanSpanMoreEnemies)
        {

            var go = Instantiate(EnemyPrefab, spawnPoint.position, Quaternion.identity);
            spawnedEnemies.Add(go.GetComponent<EnemyBoat>());

            while (timer < 1.5f)
            {
                timer += Time.deltaTime;
                go.GetComponent<Rigidbody>().AddForce(Vector3.up * 8);

                yield return new WaitForEndOfFrame();
            } 
        }
        StartCoroutine(CloseDoor());
    }

    IEnumerator CloseDoor()
    {
        timer = 0;

        while (timer < DoorAnimationTime)
        {
            timer += Time.deltaTime;
            float p = timer / DoorAnimationTime;
            float v = DoorAnimationCurve.Evaluate(p);

            float angle = Mathf.Lerp(110, 0, v);

            DoorTransform.localRotation = Quaternion.Euler(angle, 0, 0);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(5);
        StartCoroutine(OpenDoor());
    }
}
