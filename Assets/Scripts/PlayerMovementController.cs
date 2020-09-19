using UnityEngine;
using Mirror;

public class PlayerMovementController : NetworkBehaviour //Belongs to someone
{
    
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Ground Detection ")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float sphereRadius=0.4f; //Minisphere radius to detect collision with ground
    private bool isGrounded; // If is touching the ground
    [SerializeField] private LayerMask groundMask;
    [Space]
   
    #region Constanst Values
    [Header(" Constant values ")]
    [Range(-10,-50)] public float gravity = -9.81f;
    [Range (0.5f,3)] public float jumpHight=3f;
    [Range(6,15)] public float speed = 5f;
    #endregion

    [SerializeField] private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Vector3 currentVelocity;

    #region Controls Variables 
    private Vector2 previousInput; //???
    private Controls controls; //Controler
    private Controls Controls
    {
        get
        {
            if ( controls != null  ){ return controls; }
            return controls = new Controls ();
        }
    }

    #endregion 
    public override void OnStartAuthority ()
    {
        enabled = true;

        //Ctx are the values readed by Controls ( KeyBoard Input)
        Controls.Player.Move.performed += ctx => SetMovement ( ctx.ReadValue<Vector2> () ); //When performs movememt Calls SetMovement
        Controls.Player.Move.canceled += ctx => ResetMovement ();
    }

    [ClientCallback]

    private void OnEnable ()
    {
        Controls.Enable ();
    }
    [ClientCallback]

    private void OnDisable ()
    {
        Controls.Disable ();
    }

    [ClientCallback]
    private void Update ()
    {

        isGrounded = Physics.CheckSphere ( groundCheck.position , sphereRadius , groundMask );

        float horizontal = Input.GetAxisRaw ( "Horizontal" );
        float vertical = Input.GetAxisRaw ( "Vertical" );
        //Debug.Log ( $"Horizontal {horizontal} | Vertical {vertical}" );
        //Debug.Log ( $"Previus Input {previousInput}" );

        Vector3 direction = new Vector3(previousInput.x,0f,previousInput.y).normalized;
        if ( direction.magnitude >= 0.1f ) //It is moving 
        {
            Debug.Log  (direction );
            
            Move (direction);
        }


        if ( isGrounded && currentVelocity.y < 0 )
        {
            currentVelocity.y = -2f;

        }

        currentVelocity.y += gravity * Time.deltaTime;

        controller.Move ( currentVelocity * Time.deltaTime );

        /*
        if ( Input.GetButtonDown ( "Jump" ) && isGrounded )
        {
            currentVelocity.y = Mathf.Sqrt ( jumpHight * gravity * -2 );
        }
        */




    } 
    

    [Client]
    private void SetMovement ( Vector2 movement ) => previousInput = movement;

    [Client]
    private void ResetMovement () => previousInput = Vector2.zero;

    [Client]
    private void Move (Vector3 direction)
    {
        float targetAngle = Mathf.Atan2 ( direction.x , direction.z )* Mathf.Rad2Deg+ Camera.main.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle ( transform.eulerAngles.y , targetAngle , ref turnSmoothVelocity , turnSmoothTime );
        transform.rotation = Quaternion.Euler ( 0f , angle , 0f );


        Vector3  moveDir = Quaternion.Euler ( 0f , targetAngle , 0f )*Vector3.forward;

       controller.Move ( moveDir.normalized * speed * Time.deltaTime );

    }
}

