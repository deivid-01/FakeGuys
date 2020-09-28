using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject panelName;
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


    void Start()
    {
        panelName = this.transform.GetChild(1).gameObject;

        Debug.Log(Room.enableMenuUI);

        if (Room.enableMenuUI)
        {
            panelName.SetActive(true);
            Room.enableMenuUI = false;


        }

    }


}
