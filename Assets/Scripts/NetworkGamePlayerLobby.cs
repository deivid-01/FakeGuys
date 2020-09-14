using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar]
    private string displayName ="Loading...";




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
}
