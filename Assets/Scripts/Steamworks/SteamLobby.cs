using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SteamLobby : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private  GameObject buttons = null;


    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameJoinLobbyRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<GameRichPresenceJoinRequested_t> gameRichPresenceJoinRequested;

    public static CSteamID cSteamIDLobby;



    private const string HostAddressKey ="HostAddress";

    private NetworkManagerLobby networkManager;


    private void Start ()
    {
        networkManager = GetComponent<NetworkManagerLobby> ();

        if ( SteamManager.Initialized )
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create ( OnLobbyCreated );
            gameJoinLobbyRequested = Callback<GameLobbyJoinRequested_t>.Create ( OnGameJoinLobbyRequested );
            lobbyEntered = Callback<LobbyEnter_t>.Create ( OnLobbyEntered );
        }
    }
    public void HostLobby ()
    {
        SteamMatchmaking.CreateLobby ( ELobbyType.k_ELobbyTypeFriendsOnly , networkManager.maxConnections );


    }

    private void OnLobbyCreated ( LobbyCreated_t callback )
    {
        if ( callback.m_eResult != EResult.k_EResultOK )
        {
            Debug.LogError ( "Fail lobby creation" );
            return;
        }
        networkManager.RoomPlayers.Clear ();
        networkManager.StartHost ();

        DisableActualUI ();


        SteamMatchmaking.SetLobbyData (
            new CSteamID ( callback.m_ulSteamIDLobby ) , //Id of Lobby
            HostAddressKey , // Key 
            SteamUser.GetSteamID ().ToString () ); //Our id

        cSteamIDLobby = new CSteamID ( callback.m_ulSteamIDLobby );
    }


    private void OnGameJoinLobbyRequested ( GameLobbyJoinRequested_t callback )
    {

        SteamMatchmaking.JoinLobby ( callback.m_steamIDLobby );


    }



    private void OnLobbyEntered ( LobbyEnter_t callback )
    {
        if ( !NetworkServer.active )
        {
            string hostAddress=SteamMatchmaking.GetLobbyData(
                new CSteamID ( callback.m_ulSteamIDLobby ),
                HostAddressKey);
            networkManager.networkAddress = hostAddress;

            networkManager.RoomPlayers.Clear ();

            networkManager.StartClient ();

            DisableActualUI ();

        }


    }



    public void DisableActualUI ()
    {
        landingPagePanel.SetActive ( false );
    }

    public void ShowFriends ()
    {
        SteamFriends.ActivateGameOverlay ( "Friends" );
    }
}
   