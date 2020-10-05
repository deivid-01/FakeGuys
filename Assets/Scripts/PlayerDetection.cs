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
            Debug.Log("Empty: "+Room.playerWinner.Equals(string.Empty));
            //IMPROVE: Assign corresponding NetworkManagerLobby OnStartClient

            foreach (var player in Room.GamePlayers)
            {
                if (player.connectionToClient.connectionId == id && Room.playerWinner.Equals(string.Empty)) //Find the id of corresponding NetworkManagerLobby
                {
                    player.isFinished = true;
                    player.TargetFinished(player.connectionToClient); //Notify winner player
                    Room.playerWinner = player.displayName;
                    Room.RoundOver(); 

                    break;
                }
            }

            
        }
    }

}
