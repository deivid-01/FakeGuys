using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Player : NetworkBehaviour
{
    [SerializeField] private Vector3 movement = new Vector3();

    [Client]
    private void Update ()
    {
        if ( hasAuthority )
        {
            if ( Input.GetKeyDown ( KeyCode.Space ) )
            {
                //transform.Translate ( movement );
                CmdMove ();
            }
        }


    }

    [Command]
    private void CmdMove ()
    {
        //Validate logic here by the serve

        RpcMove ();
    }

    [ClientRpc]
    private void RpcMove ()
    {
        transform.Translate ( movement );
    }
}
