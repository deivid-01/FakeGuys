using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class PlayerDetection : NetworkBehaviour
{
    public int id;
    

    public NetworkGamePlayerLobby dada;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Final")
        {
        
            foreach (var player in (NetworkManager.singleton as NetworkManagerLobby).GamePlayers)
            {
                if (player.connectionToClient.connectionId == id)
                {
                    player.isFinished = true;
                    player.TargetFinished(player.connectionToClient);
                    break;
                }
            }

            (NetworkManager.singleton as NetworkManagerLobby).IsRoundOver();
        }
    }

}
