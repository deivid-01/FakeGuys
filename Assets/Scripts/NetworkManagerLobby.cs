using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
public class NetworkManagerLobby : NetworkManager
{
    [Header("Added features")]
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField ] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [Scene] [SerializeField ] private string gameScene = string.Empty;


    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action <NetworkConnection> OnServerReadied;


    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby> ();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby> ();

    #region Load From "resources" folder all game objects
    public override void OnStartServer ()
    {
        spawnPrefabs = Resources.LoadAll<GameObject> ( "SpawnablePrefabs" ).ToList ();
    }

    public override void OnStartClient ()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject> ( "SpawnablePrefabs" );

        foreach ( var prefab in spawnablePrefabs )
        {
            ClientScene.RegisterPrefab ( prefab );
        }
    }

    #endregion

    public override void OnClientConnect ( NetworkConnection conn )
    {
        base.OnClientConnect ( conn );
        OnClientConnected?.Invoke ();
    }

    public override void OnClientDisconnect ( NetworkConnection conn )
    {
        base.OnClientDisconnect ( conn );

        OnClientDisconnected?.Invoke ();
    }

    public override void OnServerConnect ( NetworkConnection conn )
    {
        if ( numPlayers >= maxConnections ) // Too  many players make disconnection
        {
            conn.Disconnect ();
            return;
        }

        if ( "Assets/Scenes/" + SceneManager.GetActiveScene ().name + ".unity" != menuScene )
        {
            conn.Disconnect ();
            return;
        }
    }

    public override void OnServerAddPlayer ( NetworkConnection conn )
    {
        if ( "Assets/Scenes/" + SceneManager.GetActiveScene ().name + ".unity" == menuScene )
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate ( roomPlayerPrefab );

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection ( conn , roomPlayerInstance.gameObject );
        }
    }

    public override void OnServerDisconnect ( NetworkConnection conn )
    {

        if ( conn.identity != null )
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby> ();
            RoomPlayers.Remove ( player );

            NotifyPlayersOfReadyState ();
        }

        base.OnServerDisconnect ( conn );
    }

    public override void OnStopServer ()
    {
        RoomPlayers.Clear ();
    }

    public void NotifyPlayersOfReadyState ()
    {
        foreach ( var player in RoomPlayers )
        {
            player.HandleReadyToStart ( IsReadyToStart () );
        }
    }

    private bool IsReadyToStart ()
    {
        if ( numPlayers >= minPlayers )
        {
            foreach ( var player in RoomPlayers )
            {
                if ( !player.IsReady )
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }


    public void StartGame ()
    {
        if ( "Assets/Scenes/" + SceneManager.GetActiveScene ().name + ".unity" == menuScene )
        {
            if ( IsReadyToStart () )
            {
                ServerChangeScene ( gameScene );
            }
        }
   

    }

    public override void ServerChangeScene ( string newSceneName )
    {
        //From Menu to game

        if ( "Assets/Scenes/" + SceneManager.GetActiveScene ().name + ".unity" == menuScene && newSceneName.StartsWith ( "Assets/Scenes/Scene_Map" ) )
        {
            for ( int  i = RoomPlayers.Count -1 ; i >=0  ; i-- )
            {
                var conn = RoomPlayers [i].connectionToClient;
                var   gameplayInstance = Instantiate ( gamePlayerPrefab );
                gameplayInstance.SetDisplayName ( RoomPlayers [i].DisplayName );

                NetworkServer.Destroy ( conn.identity.gameObject );
                NetworkServer.ReplacePlayerForConnection ( conn , gameplayInstance.gameObject , true );
   
            }
        }

        base.ServerChangeScene ( newSceneName );
    }


    public override void OnServerSceneChanged ( string sceneName )
    {
        if ( sceneName.StartsWith ( "Assets/Scenes/Scene_Map" ) )
        {
            Debug.Log ( "oliwis2.0" );
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn ( playerSpawnSystemInstance );
        }
    }

    public override void OnServerReady ( NetworkConnection conn )
    {
        base.OnServerReady ( conn );

        OnServerReadied?.Invoke ( conn );
    }


}
