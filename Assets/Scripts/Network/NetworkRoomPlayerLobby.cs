using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Steamworks;
[System.Serializable]
public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;


    [SyncVar(hook = nameof(HanldeDisplayNameChanged))]
    public string DisplayName ="Loading...";
    [SyncVar ( hook = nameof ( HandleReadyStatusChanged ) )]

    public bool IsReady = false;

    private bool isLeader;

    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive ( value );
        }
    }

    private NetworkManagerLobby room;

    private NetworkManagerLobby Room
    {
        get
        {
            if ( room == null )
            {
                return room = NetworkManager.singleton as NetworkManagerLobby;
            }
            return room;
        }
    }

    public override void OnStartAuthority ()
    {
        CmdSetDisplayName ( PlayerNameInput.DisplayName );

        lobbyUI.SetActive ( true );
    }

    public override void OnStartClient ()
    {
        Room.RoomPlayers.Add(this);


        RpcUpdateDisplay ();
    }

   
    public override void OnStopClient ()
    {
        Room.RoomPlayers.Remove ( this );
        

        RpcUpdateDisplay ();
    }

    [Server]

    public void SetDisplayName(string displayName)
    {
        DisplayName= displayName;
    }



    public void HandleReadyStatusChanged ( bool oldValue , bool newValue ) => RpcUpdateDisplay ();

    public void HanldeDisplayNameChanged ( string oldValue , string newValue ) => RpcUpdateDisplay ();


    private void RpcUpdateDisplay ()
    {
        if ( !hasAuthority )
        {
            foreach ( var player in Room.RoomPlayers )
            {
                if ( player.hasAuthority )
                {
                    player.RpcUpdateDisplay ();
                    break;
                }
            }
            return;
        }

        for ( int i = 0 ;i < playerNameTexts.Length ;i++ )
        {
            playerNameTexts [i].text = "Waiting for Player...";
            playerReadyTexts [i].text = string.Empty;
        }

        for ( int i = 0 ;i < Room.RoomPlayers.Count ; i++ )
        {
            playerNameTexts [i].text = Room.RoomPlayers [i].DisplayName;
            playerReadyTexts [i].text = Room.RoomPlayers [i].IsReady ?
                    "<color=green>Ready</color>" :
                    "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart ( bool readyToStart )
    {
        if ( isLeader )
        {
            startGameButton.interactable = readyToStart;
        }
    }

    [Command]
    private void CmdSetDisplayName ( string displayName )
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp ()
    {
        IsReady = !IsReady;
        
        Room.NotifyPlayersOfReadyState ();
    }

    [Command]

    public void CmdStartGame ()
    {
        if ( Room.RoomPlayers [0].connectionToClient == connectionToClient )
        {
            Room.StartGame ();
        }
       
    }

    public void InviteFriend ()
    {
        SteamFriends.ActivateGameOverlayInviteDialog ( SteamLobby.cSteamIDLobby );
      
    }

    public void LeaveLobby ()
    {
        SteamMatchmaking.LeaveLobby ( SteamLobby.cSteamIDLobby );
       Destroy ( gameObject );
    
        if ( isLeader )
        {
            Room.StopHost ();
        }
        else
        {
            Room.StopClient ();
            Room.RoomPlayers.Remove ( this );
            //Room.RoomPlayers.Clear ();
        }




    }


}
