using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    public RescueModule RescueHelper { get; internal set; }

    [Range(5,20)]
    public int MaxActiveEnemies = 12;

    public PlayerController Player;
    public List<GameObject> FishPrefabs;
    public BoxCollider FishBounds;
    List<FishAI> fishList = new List<FishAI>();

    EnemyBase[] enemyBases;

    public bool GameRunning { get; private set; }

    public int FishCount
    {
        get { return fishList.Count; }
    }

    public CrateSpawner CrateSpwaner { get; internal set; }
    public UIController UIController { get; internal set; }
    public CameraController CameraManager { get; internal set; }

    public int EnemyCount { get; set; }

    public bool CanSpanMoreEnemies
    {
        get
        {
            return EnemyCount < MaxActiveEnemies;
        }
    }

    public Color GoodColor;
    public Color BadColor;

    public int FishSpawnCount = 5;

    private void Awake()
    {
        // singleton stuff
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }

    private void Update()
    {
        // camera test
        if (Input.GetKeyUp(KeyCode.Alpha1))
            Application.CaptureScreenshot("ScreenShot_" + System.DateTime.Now.Ticks + ".jpg");
    }

    void Start ()
    {
        GameRunning = false;
        if (Player == null)
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                Player = playerGO.GetComponent<PlayerController>();
        }

        Player.GetComponent<Rigidbody>().useGravity = false;
        enemyBases = FindObjectsOfType<EnemyBase>();
	}


    public void RegisterFish(FishAI fish)
    {
        fishList.Add(fish);
    }

    bool isGameOver = false;

    public void HandleGameOver()
    {
        if (isGameOver)
            return;
        isGameOver = true;

        UIController.GotoGameOver();
        StopGame(true);
        Debug.Log("GameOver");
    }

    public void RescueFish(FishAI currentFish)
    {
        if (isGameOver)
            return;
        

        if (fishList.Count == 0)
        {
            Debug.Log("You already won the game");
            return;
        }


        AudioManager.Instance.PlayAudioClip("Rescue", currentFish.transform.position);
        UIController.SpawnFishInfo(currentFish.transform.position);
        Debug.Log("You just saved a Fish!!");
        fishList.Remove(currentFish);
        Destroy(currentFish.gameObject);

        if(fishList.Count == 0)
        {
            isGameOver = true;

            StopGame(false);
            UIController.GotoWin();
            Debug.Log("You WIN THE GAME");
        }
    }

    public void StartNewGame()
    {
        isGameOver = false;

        SpawnFishes();

        EnemyCount = 0;

        Player.GetComponent<Rigidbody>().useGravity = true;
        foreach (var eb in enemyBases)
        {
            eb.StartSpawning();
        }
        CrateSpwaner.StartSpawning();

        

        GameRunning = true;

        Player.ResetState();
        CameraManager.CurrentMode = CameraMode.Game;
    }


    void SpawnFishes()
    {
        // cleanup fishes
        foreach (var fish in fishList)
        {
            if (fish == null)
                continue;
            Destroy(fish.gameObject);
        }

        fishList.Clear();

        for (int i = 0; i < FishSpawnCount; i++)
        {
            var fishIndex = Random.Range(0, FishPrefabs.Count);
            var go = Instantiate(FishPrefabs[fishIndex], GetRandomFishPosition(), Quaternion.identity);
            go.transform.SetParent(this.transform);

            go.GetComponent<FishAI>().gameBounds = FishBounds;
        }
    }

    Vector3 GetRandomFishPosition()
    {
        float offset = 0;
        var min = FishBounds.bounds.min;
        var max = FishBounds.bounds.max;

        float x = Random.Range(min.x + offset, max.x - offset);
        float y = Random.Range(min.y + offset, max.y - offset);
        float z = Random.Range(min.z + offset, max.z - offset);

        return new Vector3(x, y, z);
    }


    void StopGame(bool gameOver)
    {
        Debug.Log("Game stopped");
        GameRunning = false;

        CameraManager.CurrentMode = CameraMode.Menu;
        CrateSpwaner.StopSpawning();
        if (Player != null)
        {
            Player.ControlsEnabled = false;
            Player.ReleaseFish();
        }
            
        if(!gameOver)
        {
            foreach (var eb in enemyBases)
            {
                eb.StopSpawning();
                foreach (var boat in eb.Enemies)
                    boat.Kill();
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
