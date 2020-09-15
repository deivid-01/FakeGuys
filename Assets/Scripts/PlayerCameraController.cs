using UnityEngine;
using Mirror;
using Cinemachine;
using System;

public class PlayerCameraController : NetworkBehaviour
{
    [Header("Camera")]
        [SerializeField] private Vector2 maxFollowOffset = new Vector2(-1f,6f);
        [SerializeField] private Vector2 cameraVelocity = new Vector2(4f,0.25f);
        [SerializeField] private Transform playerTransform = null;
        [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    private Controls controls;
    private Controls Controls
    {
        get {
            if ( controls != null )
                return controls;
            return controls=new Controls ();
        }
    }

    private CinemachineTransposer transposer;

    public override void OnStartAuthority () // Each player gets their own camera
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer> ();

        virtualCamera.gameObject.SetActive ( true );

        enabled = true;

  
        Controls.Player.Look.performed += ctx => Look ( ctx.ReadValue<Vector2> () );

    }

    [ClientCallback] //Online gets call on the client

    private void OnEnable () => Controls.Enable ();

    [ClientCallback]

    private void OnDisable () => Controls.Disable ();

    private void Look ( Vector2 lookAxis )
    {
        float deltaTime = Time.deltaTime;

        transposer.m_FollowOffset.y = Mathf.Clamp (
            transposer.m_FollowOffset.y - ( lookAxis.y * cameraVelocity.y * deltaTime ) ,
            maxFollowOffset.x ,
            maxFollowOffset.y );

        playerTransform.Rotate ( 0f , lookAxis.x * cameraVelocity.x * deltaTime , 0f );
    }

}
