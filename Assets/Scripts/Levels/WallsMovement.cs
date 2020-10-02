
using UnityEngine;

public class WallsMovement : MonoBehaviour
{
    // Start is called before the first frame update

   [SerializeField] private  Collider groundCollider;
    [Range(0,10)]
    [SerializeField] private  float speed = 1 ;
    [Range(0, 10)]
    [SerializeField] private float boundaryUp;
    [SerializeField] private float boundaryDown;

    Vector3 initialPosition;
    [SerializeField] private bool upMove;

    void Start()
    {
        Physics.IgnoreCollision ( groundCollider , GetComponent<Collider> () );

         initialPosition = transform.position;
        
       // upMove = Random.value<=0.5; //Set Random tu Up or Down
    }
    void Update()
    {
        Move ();
    }

    public void Move ()
    {

        if ( upMove )
        {
            transform.position += Time.deltaTime * speed * Vector3.up;
            if ( transform.position.y >= initialPosition.y+ boundaryUp )
            {
                upMove = false;
            }
        }
        else if ( !upMove )
        {

            transform.position -= Time.deltaTime * speed * Vector3.up;
            if ( transform.position.y <= initialPosition.y - boundaryDown)
            {
                upMove = true;
            }
        }
    }

    
}
