using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class PlayerNameInput : MonoBehaviour // Class to set player name
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null; //Set the name
    [SerializeField] private Button continueButton = null;


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



    public static string DisplayName { get; private set; }


    public void SetPlayerName ( string name )
    {
        continueButton.interactable = !string.IsNullOrEmpty ( name ); 
    }

    public void SavePlayerName () {
        DisplayName = nameInputField.text;

    }
}
