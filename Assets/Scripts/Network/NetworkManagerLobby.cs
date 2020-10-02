using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using Steamworks;
public class NetworkManagerLobby : NetworkManager
{

    [Header("Added features")]
    [Header("------------------------------")]
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;
    [SerializeField] private string levelsSceneRoot = "Assets/Scenes/Levels/";

    [Header("Maps")]
    [SerializeField] private int numberOfRounds = 1;
    [SerializeField] private MapSet mapSet = null;
    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject roundSystem = null;

    public PlayerInfoDisplay playerInfoDisplay;


    private MapHandler mapHandler;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;



    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

    public bool enableMenuUI=true;

    public string playerWinner;

    #region Load From "resources" folder all game objects
    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
        Debug.Log("Server se inició");

    }


    public override void OnStartClient()
    {
       

        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }

        Debug.Log("Client Starting");

    }

    #endregion

    public override void OnClientConnect(NetworkConnection conn)
    {


        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }



    public override void OnStopClient()
    {

        //base.OnStopClient ();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();


    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections) // Too  many players make disconnection
        {
            conn.Disconnect();
            return;
        }

        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);

            #region Steam configuration

            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.cSteamIDLobby, numPlayers - 1); //Grab id from steam


            var playerInfoDisplay = conn.identity.GetComponent<PlayerInfoDisplay>(); //grab the component of that game object

            playerInfoDisplay.SetSteamId(steamId.m_SteamID); //Sets id

            #endregion 
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {



        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStartGame());
        }
    }


    public void RoundOver()
    {
        StartCoroutine(NotifyRoundOver());
    }
    IEnumerator NotifyRoundOver()
    {
        NotifyEliminatedPlayers();
        yield return new WaitForSeconds(3f);
        NotifyRoundIsOver();
        yield return new WaitForSeconds(1f);
        ShowWinner();
        yield return new WaitForSeconds(10f);
        ServerChangeScene(menuScene);


    }

    public void ShowWinner()
    {
        foreach (var player in GamePlayers)
        {
            player.RpcShowWinner(playerWinner);
        }
    }

  


    public void NotifyRoundIsOver()
    {
        foreach (var player in GamePlayers)
        {
            player.RpcRoundIsOver();
        }
    }

    private void NotifyEliminatedPlayers()
    {
        foreach (var player in GamePlayers)
        {
            if (!player.isFinished)
            {
                player.TargetUnFinished(player.connectionToClient);
            }
        }

    }


    private bool IsReadyToStartGame()
    {
        if (numPlayers >= minPlayers)
        {
            foreach (var player in RoomPlayers)
            {
                if (!player.IsReady)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }


    public void StartGame()
    {
        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene)
        {
            if (IsReadyToStartGame())
            {

                mapHandler = new MapHandler(mapSet, numberOfRounds);

                ServerChangeScene(mapHandler.NextMap);
            }
        }


    }

    public override void ServerChangeScene(string newSceneName)
    {
        //From Menu to game

        if ("Assets/Scenes/" + SceneManager.GetActiveScene().name + ".unity" == menuScene && newSceneName.StartsWith(levelsSceneRoot + "Scene_Map"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayInstance = Instantiate(gamePlayerPrefab);
                gameplayInstance.SetDisplayName(RoomPlayers[i].playerInfoDisplay.displayNameText);



                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayInstance.gameObject, true);

            }
        }
        else if (newSceneName == menuScene)
        {
           //RoomPlayers.Clear();
            for (int i = GamePlayers.Count - 1; i >= 0; i--)
            {
                var conn = GamePlayers[i].connectionToClient;
                var roomPlayerInstance = Instantiate(roomPlayerPrefab);
                


                NetworkServer.Destroy(conn.identity.gameObject);
              //  NetworkServer.ReplacePlayerForConnection(conn, roomPlayerInstance.gameObject, true);

            }
        }

        base.ServerChangeScene(newSceneName);
    }


    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith(levelsSceneRoot + "Scene_Map"))
        {

            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);

            //Spawn round system
            GameObject roundSystemInstance = Instantiate(roundSystem);
            NetworkServer.Spawn(roundSystemInstance);
        }


    }


    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }


}
