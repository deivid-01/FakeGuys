﻿using System.Collections;
using UnityEngine;
using Mirror;
public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar]
    private string displayName ="Loading...";


    [Header("UI")]
    [SerializeField] private GameObject ui_qualified = null;
    [SerializeField] private GameObject ui_eliminated = null;
    [SerializeField] private GameObject ui_roundOver = null;


    [SerializeField] private  GameObject canvasUI=null;


    public bool isFinished=false;

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
        canvasUI.SetActive ( true );
        isFinished = false;

    }



    private void Awake ()
    {
        DontDestroyOnLoad ( gameObject );

    }
    public override void OnStartClient ()
    {
        DontDestroyOnLoad ( gameObject );


        Room.GamePlayers.Add ( this );
    }

    public override void OnStopClient ()
    {
        Room.GamePlayers.Remove ( this );


    }

  

    [Server]

    public void SetDisplayName ( string displayName )
    {
        this.displayName = displayName;
    }



    [ClientRpc]
    public void RpcRoundIsOver ()
    {
        ui_eliminated.SetActive(false);
        ui_qualified.SetActive(false);
        ui_roundOver.SetActive(true);

    }


    [TargetRpc]

    public void TargetFinished(NetworkConnection target)
    {
        ui_qualified.SetActive(true);
        isFinished = true;
      
    }

    [TargetRpc]
    public void TargetUnFinished(NetworkConnection target)
    {
        ui_eliminated.SetActive(true);
        isFinished = false;
  
    }
}
