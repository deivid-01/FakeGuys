using UnityEngine;
using Mirror;
using Cinemachine;
using System;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
      




        [SerializeField] private Transform playerTransform = null;
        [SerializeField] private CinemachineFreeLook freeLookCamera = null;

    private Controls controls;
    private Controls Controls
    {
        get {
            if ( controls != null )
                return controls;
            return controls=new Controls ();
        }
    }




    public override void OnStartAuthority () // Each player gets their own camera
    {
 


        freeLookCamera.gameObject.SetActive ( true );

        enabled = true;

  
        Controls.Player.Look.performed += ctx => Look ( ctx.ReadValue<Vector2> () );

    }

    [ClientCallback] //Online gets call on the client

    private void OnEnable () => Controls.Enable ();

    [ClientCallback]

    private void OnDisable () => Controls.Disable ();

    private void Look ( Vector2 lookAxis )
    {
        

    }

}
