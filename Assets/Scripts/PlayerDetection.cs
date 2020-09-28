using UnityEngine;
using Mirror;
public class PlayerDetection : NetworkBehaviour
{
    public int id;

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



    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Final") //Gets to the final of level
        {

            //IMPROVE: Assign corresponding NetworkManagerLobby OnStartClient

            foreach (var player in Room.GamePlayers)
            {
                if (player.connectionToClient.connectionId == id) //Find the id of corresponding NetworkManagerLobby
                {
                    player.isFinished = true;
                    player.TargetFinished(player.connectionToClient); //Notify winner player
                    Room.playerWinner = player.displayName;
                    break;
                }
            }

            Room.RoundOver(); //Network Manaager Ends Rou
            
        }
    }

}
