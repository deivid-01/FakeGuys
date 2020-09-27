using UnityEngine;
using Mirror;
public class PlayerDetection : NetworkBehaviour
{
    public int id;
    
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Final") //Gets to the final of level
        {

            //IMPROVE: Assign corresponding NetworkManagerLobby OnStartClient

            foreach (var player in (NetworkManager.singleton as NetworkManagerLobby).GamePlayers)
            {
                if (player.connectionToClient.connectionId == id) //Find the id of corresponding NetworkManagerLobby
                {
                    player.isFinished = true;
                    player.TargetFinished(player.connectionToClient); //Notify winner player
                    break;
                }
            }

            (NetworkManager.singleton as NetworkManagerLobby).RoundOver(); //Network Manaager Ends Rou
        }
    }

}
