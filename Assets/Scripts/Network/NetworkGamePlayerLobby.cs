using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;
public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar]
    public string displayName ="Loading...";


    [Header("UI")]
    [SerializeField] private GameObject ui_qualified = null;
    [SerializeField] private GameObject ui_eliminated = null;
    [SerializeField] private GameObject ui_roundOver = null;
    [SerializeField] private GameObject ui_winner = null;
    [SerializeField] private TMP_Text textWinner = null;


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
        InputManager.Add(ActionMapNames.Player); //Block player movement and look
        InputManager.Controls.Player.Look.Enable(); //Enable look around for the player



        ui_qualified.SetActive(true);

        Room.playerWinner = displayName;

        isFinished = true;
      
    }

    [TargetRpc]
    public void TargetUnFinished(NetworkConnection target)
    {
        InputManager.Add(ActionMapNames.Player); //Block player movement and look


        ui_eliminated.SetActive(true);
        isFinished = false;
  
    }
    [ClientRpc]
    public void RpcShowWinner(string nameWinner)
    {
        textWinner.text = nameWinner;
        ui_winner.SetActive(true);
    }
}
