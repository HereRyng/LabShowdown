using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;
using Random = UnityEngine.Random;
using System;

public class DeathMatch : MonoBehaviour
{
    private float timerToGame;
    [SerializeField] private GameObject screenInfoGame;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private Transform[] playerSpawns;

    [SerializeField] private Transform[] respawnPoints;

    [SerializeField] private GameObject[] weapons;

    [SerializeField] private GameObject[] pointsSpawnWeapons;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private int playersLivesQuantity;

    [SerializeField] private float cooldownSpawn;

    [SerializeField] private float timeLifeOfWeapons;

    private float currentTimeSpawn;  

    private List<GameObject> players;
    private float timerSound = 0;

    public static event Action<Sprite, Color,int> OnWinHUD;
    public static event Action<PlayerConfiguration, int> OnCreateHUD;
    public static float TimeLife { get ; private set ; }

    private void Awake()
    {
        AudioManager.Instance.Stop("menumusic");
        
    }


    void Start()
    {

        TimeLife = timeLifeOfWeapons;
        currentTimeSpawn = 0;
        timerToGame = 7f;
        Destroy(screenInfoGame, 3f);
    }


    private void InitializeLevel()
    {
        players = new List<GameObject>();
        var playerConfigs = MainMenuManager.Instance.GetPlayerConfigurations().ToArray();
        MainMenuManager.Instance.PlayersList.Clear();
       

        for (int i = 0; i < playerConfigs.Length; i++)
        {

            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            player.GetComponent<PlayerController>().InitializePlayer(playerConfigs[i]);
            player.GetComponent<StatsController>().SetLifes(playersLivesQuantity);
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            OnCreateHUD?.Invoke(playerConfigs[i], playersLivesQuantity);
            MainMenuManager.Instance.PlayersList.Add(playerConfigs[i]);
            players.Add(player);
        }

        StatsController.OnDie += OnDieHandler;
        StatsController.OnRespawn += OnRespawnHandler;
    }
    private void Update()
    {
        if (timerToGame != 10) 
        {
            if(timerSound <= 0)
            {
                AudioManager.Instance.Play("counter");
                timerSound = 1f;
            }
            else
            {
                timerSound -= Time.deltaTime;
            }
            timerToGame -= Time.deltaTime;
            int temp = (int)timerToGame;
            textCount.text = temp.ToString();
            
            if (timerToGame < 1)
            {
                textCount.text = "FIGHT";
                if (timerToGame <= 0)
                {
                    timerToGame = 10;
                    Destroy(textCount);
                    InitializeLevel();
                    AudioManager.Instance.Play("gameplay");
                }
            }
        }
        else
        {
            currentTimeSpawn -= Time.deltaTime;
            if (currentTimeSpawn <= 0)
            {
                WeaponSpawner();
                currentTimeSpawn = cooldownSpawn;
            }
        }

       
           

    }
    private void OnRespawnHandler(int playerIndex)
    {
        //TODO: PRIORIDAD RB VELOCITY 0. TIMER

        players[playerIndex].transform.position = respawnPoints[playerIndex].transform.position;
        if(players[playerIndex].GetComponent<PlayerModel>().Weapon != null)
        {
            players[playerIndex].GetComponent<PlayerModel>().Weapon.DestroyWeapon();
            players[playerIndex].GetComponent<PlayerModel>().WeaponIsNull();
        }
    }

    private void OnDieHandler(int playerIndex)
    {
        List<PlayerConfiguration> playersList = MainMenuManager.Instance.PlayersList;
        if (playersList.Contains(playersList[playerIndex]) && playersList.Count > 1)
        {
            MainMenuManager.Instance.PlayersList.RemoveAt(playerIndex);
            print("mori");
         
        }
        if (MainMenuManager.Instance.PlayersList.Count < 2)
        {
            int indexWin = MainMenuManager.Instance.PlayersList[0].PlayerIndex + 1;
            OnWinHUD?.Invoke(MainMenuManager.Instance.PlayersList[0].PlayerSkin, MainMenuManager.Instance.PlayersList[0].SkinColor, indexWin);
            Debug.Log("gano el player" + (MainMenuManager.Instance.PlayersList[0].PlayerIndex + 1));
        }

    }
    public void LoadMainMenu()
    {

        var menuManager = GameObject.Find("MenuManager");

        Destroy(menuManager);

        SceneManager.LoadScene(0);
    }
    public void WeaponSpawner()
    {
        
        Instantiate(weapons[Random.Range(0, weapons.Length)], pointsSpawnWeapons[Random.Range(0, pointsSpawnWeapons.Length)].transform.position, Quaternion.Euler(0,0,90));
    }
    private void OnDisable()
    {
        StatsController.OnDie -= OnDieHandler;
        StatsController.OnRespawn -= OnRespawnHandler;
    }
}
