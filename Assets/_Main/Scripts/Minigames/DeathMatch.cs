using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;
using Random = UnityEngine.Random;
using System;

public class DeathMatch : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawns;
    [SerializeField] private Transform[] respawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private TextMeshProUGUI[] playersLives;
    [SerializeField] private int playersLivesQuantity;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject[] pointsSpawns;
    private float currentTimeSpawn;
    [SerializeField] private float cooldownSpawn;
    [SerializeField] private float timeLifeOfWeapons;
    private List<GameObject> players;
    public static float TimeLife { get ; private set ; }

    void Start()
    {
        TimeLife = timeLifeOfWeapons;
        currentTimeSpawn = 0;
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        players = new List<GameObject>();
        var playerConfigs = PlayerConfigManager.Instance.GetPlayerConfigurations().ToArray();
        PlayerConfigManager.Instance.playersList.Clear();


        for (int i = 0; i < playerConfigs.Length; i++)
        {
            var player = Instantiate(playerPrefab, playerSpawns[i].position, playerSpawns[i].rotation, gameObject.transform);
            player.GetComponent<PlayerController>().InitializePlayer(playerConfigs[i]);
            player.GetComponent<StatsController>().SetLifes(playersLivesQuantity);
            playersLives[i].text = playersLivesQuantity.ToString();
            PlayerConfigManager.Instance.playersList.Add(playerConfigs[i]);
            players.Add(player);
        }

        StatsController.OnDie += OnDieHandler;
        StatsController.OnRespawn += OnRespawnHandler;
    }
    private void Update()
    {
        currentTimeSpawn -= Time.deltaTime;
        if (currentTimeSpawn <= 0)
        {
            Spawner();
            currentTimeSpawn = cooldownSpawn;
        }

    }
    private void OnRespawnHandler(int playerIndex)
    {
        players[playerIndex].transform.position = respawnPoints[playerIndex].transform.position;
    }

    private void OnDieHandler(int playerIndex)
    {
        for (int i = 0; i < PlayerConfigManager.Instance.playersList.Count; i++)
        {
            if(playerIndex == PlayerConfigManager.Instance.playersList[i].PlayerIndex)
            {
                PlayerConfigManager.Instance.playersList.RemoveAt(playerIndex);
            }
        }
        if(PlayerConfigManager.Instance.playersList.Count == 1)
        {          
            Debug.Log("gano el player" + (PlayerConfigManager.Instance.playersList[0].PlayerIndex + 1));
            SceneManager.LoadScene("Menu");

        }
    }

    public void Spawner()
    {
        
        Instantiate(weapons[Random.Range(0, 5)], pointsSpawns[Random.Range(0, 3)].transform.position, Quaternion.Euler(0,0,90));
    }


    private void OnDisable()
    {
        StatsController.OnDie -= OnDieHandler;
        StatsController.OnRespawn -= OnRespawnHandler;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
