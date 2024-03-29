﻿using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using TMPro;

public class PlayerSpawnSystem : NetworkBehaviour //Spawned by the server
{
    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    private NetworkManagerLobby room;

    private NetworkManagerLobby Room
    {
        get
        {
            if (room == null)
            {
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
            return room;
        }
    }


    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();

    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

    public override void OnStartClient()
    {
     
        InputManager.Add(ActionMapNames.Player); //Block player movement and look
        InputManager.Controls.Player.Look.Enable(); //Enable look around for the player

    }

    [ServerCallback]

    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]

    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);

        playerInstance.GetComponent<PlayerDetection>().id = conn.connectionId; // Connection between player and NetworkGamePlayer

        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
    }







}
