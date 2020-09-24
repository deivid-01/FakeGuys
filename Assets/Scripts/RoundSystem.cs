using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private Animator animator = null;



    private NetworkManagerLobby room;

    private NetworkManagerLobby Room
    {
        get
        {
            if ( room != null )
                return room;
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }
    }

    public void CountdownEnded ()
    {
        animator.enabled = false;
    }

    #region Server
    public override void OnStartServer ()
    {
        NetworkManagerLobby.OnServerStopped += CleanUpServer;
        NetworkManagerLobby.OnServerReadied += CheckToStartRound; //When Someone is ready on the server
    }

    [ServerCallback]

    private void OnDestroy ()
    {
        CleanUpServer ();
    }

    [Server]

    private void CleanUpServer ()
    {
        NetworkManagerLobby.OnServerStopped -= CleanUpServer;
        NetworkManagerLobby.OnServerReadied -= CheckToStartRound;
    }

    [ServerCallback]

    public void StartRound ()
    {
        RpcStartRound ();
    }

    [Server]

    private void CheckToStartRound ( NetworkConnection conn )
    {
        if ( Room.GamePlayers.Count ( x => x.connectionToClient.isReady ) == Room.GamePlayers.Count)
        { 
            
            animator.enabled = true;
 
            RpcStartCountDown (); //Count down on the server is for actually starting the level | On the client It is just a visualisation
        }
    }
    #endregion

    #region Client
    [ClientRpc]

    private void RpcStartCountDown ()
    {
        animator.enabled = true;
    }

    [ClientRpc]
    private void RpcStartRound ()
    {
        InputManager.Remove ( ActionMapNames.Player ); // The player can now move and look around
    }
    #endregion
}
