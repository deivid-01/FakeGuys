using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar]
    private string displayName ="Loading...";


    [Header("UI")]
    [SerializeField] private GameObject ui_qualified = null;
    [SerializeField] private GameObject ui_eliminated = null;
    [SerializeField] private GameObject ui_roundOver = null;


    [SerializeField] private  GameObject canvasUI=null;


    [SerializeField] private  Text  id=null;

    

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
        id.text = this.connectionToClient.connectionId.ToString();
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
        StartCoroutine ( DisplayMessage ( ui_roundOver ) );
    }

    public IEnumerator DisplayMessage ( GameObject msg )
    {
        yield return new WaitForSeconds ( 3 );
        ui_eliminated.SetActive ( false );
        ui_qualified.SetActive ( false );
        msg.SetActive ( true );

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
