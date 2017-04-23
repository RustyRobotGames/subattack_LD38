using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    public float minSpawnTime = 5;
    public float maxSpawnTime = 15;

    float nextSpawnTime = -1;

    public GameObject[] CratePrefabs;
    public BoxCollider SpawnBounds;

    private void Start()
    {
        GameManager.Instance.CrateSpwaner = this;
        StopSpawning();
    }

    public void StopSpawning()
    {
        enabled = false;
    }

    public void StartSpawning()
    {
        nextSpawnTime = GetRandomTime();
        enabled = true;
    }

    float GetRandomTime()
    {
        return Random.Range(minSpawnTime, maxSpawnTime);
    }

    Vector3 GetRandomPosition()
    {
        float offset = 0;
        var min = SpawnBounds.bounds.min;
        var max = SpawnBounds.bounds.max;

        float x = Random.Range(min.x + offset, max.x - offset);
        float y = Random.Range(min.y + offset, max.y - offset);
        float z = Random.Range(min.z + offset, max.z - offset);

        return new Vector3(x, y, z);
    }

    private void Update()
    {
        nextSpawnTime -= Time.deltaTime;
        if(nextSpawnTime < 0)
        {
            nextSpawnTime = GetRandomTime();
            var index = Random.Range(0, CratePrefabs.Length);
            var pos = GetRandomPosition();

            var go = Instantiate(CratePrefabs[index], pos, Quaternion.identity);
        }
    }

}
