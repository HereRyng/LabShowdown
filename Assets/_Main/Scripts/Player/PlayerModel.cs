using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{

    private bool cancelledJump;
    private Transform weaponPrefab;
   
    [SerializeField] private int speedYWallSlide; //TODO: pasarlo a stats
    [SerializeField] private int speedYFalling; //TODO: pasarlo a stats

    [SerializeField] private LayerMask floor;

    [SerializeField] private float coyoteTimeSet;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private float jumpTime;

    [SerializeField] private float inputBufferTimeSet;

    [SerializeField] private Transform hand;

    [SerializeField] private Transform arm;

    [SerializeField] private Transform dropPosition;

    [SerializeField] private float raycastHorizontalDistance;

    [SerializeField] private float raycastFloorDistance;

    [SerializeField] private Transform floorOffset;

    [SerializeField] private Transform hitOffsetLeft;

    [SerializeField] private Transform hitOffsetRight;

    private bool weaponReady;

    private Queue<string> inputBuffer = new Queue<string>();

    private RaycastHit2D floorRaycast;
    private RaycastHit2D sideLeftRaycast;
    private RaycastHit2D sideRightRaycast;
    private float coyoteTime;
    private float jumpCounter;
    private bool alreadyJumped;

    private Vector2 gravity;

    private StatsController statsController;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerView playerView;
    private Fists fists;


    public IWeapon Weapon { get; private set; }

    public bool AlreadyJumped { get => alreadyJumped;  set => alreadyJumped = value; }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        statsController = GetComponent<StatsController>();
        rb = GetComponent<Rigidbody2D>();
        playerView = GetComponent<PlayerView>();
        fists = GetComponentInChildren<Fists>();
    }

    private void Start()
    {
        gravity = new Vector3(0, -Physics2D.gravity.y);
        Weapon = null;
    }

    private void Update()
    {
        
    }

    public void Raycasts()
    {
        floorRaycast = Physics2D.Raycast(floorOffset.position, Vector2.down, raycastFloorDistance, floor);
        sideRightRaycast = Physics2D.Raycast(hitOffsetRight.position, Vector2.down, raycastHorizontalDistance , floor);
        sideLeftRaycast = Physics2D.Raycast(hitOffsetLeft.position, Vector2.down, raycastHorizontalDistance , floor);
    }

    public void Movement(float x)
    {
        
        if (!sideRightRaycast && !sideLeftRaycast)
        {
            rb.velocity = new Vector3(x * statsController.Speed, rb.velocity.y, 0f);
            playerView.Anim.SetFloat("Speed", x);
            AudioManager.Instance.Play("running");

        }
       
        //if(!floorRaycast && !alreadyJumped && !sideLeftRaycast && !sideRightRaycast)
        //{
        //    rb.velocity = new Vector3(rb.velocity.x, speedYFalling, 0f);
            
        //}
        //if (sideLeftRaycast && !alreadyJumped || sideRightRaycast && !alreadyJumped)
        //{ 
        //    rb.velocity = new Vector3(x * statsController.Speed, speedYWallSlide, 0f);
        //}

        if (x < 0)
        {
            var ang = transform.rotation.eulerAngles;
            ang.y = 180;
            transform.rotation = Quaternion.Euler(ang);
            playerView.Anim.SetFloat("Speed", -x);
            AudioManager.Instance.Play("running");

        }
        if (x > 0)
        {
            var ang = transform.rotation.eulerAngles;
            ang.y = 0;
            transform.rotation = Quaternion.Euler(ang);
           
        }

        
      
    }

    public void Jump(float x)
    {
        //TODO: REWORK JUMP
        if ((floorRaycast == true || sideRightRaycast && !floorRaycast || sideLeftRaycast && !floorRaycast) && alreadyJumped == false)
        {
            
            if (inputBuffer.Count > 0)
            {
                if (inputBuffer.Peek() == "jump")
                {
                    if (!sideLeftRaycast && !sideRightRaycast)
                    {
                        rb.velocity = new Vector3(rb.velocity.x, statsController.JumpHeight, 0f);
                        inputBuffer.Dequeue();
                        alreadyJumped = true;
                        jumpCounter = 0;
                        AudioManager.Instance.Play("jump");
                        return;
                    }
                    if (coyoteTime < coyoteTimeSet && alreadyJumped == false)
                    {
                        if (!sideLeftRaycast && !sideRightRaycast)
                        {
                            rb.velocity = new Vector3(rb.velocity.x, statsController.JumpHeight, 0f);
                        }
                        inputBuffer.Dequeue();
                        alreadyJumped = true;
                        jumpCounter = 0;
                        AudioManager.Instance.Play("jump");

                    }



                }

            }
        }     
    }
    //public void CancelledJump()
    //{
    //    cancelledJump = true;
    //    if (rb.velocity.y > 0 && !sideLeftRaycast && !sideRightRaycast)
    //    {
    //        rb.velocity = new Vector3(rb.velocity.x, speedYFalling, 0f);
          
    //    }         
    //}

    public void VariableJump()
    {
        if (rb.velocity.y > 0 && alreadyJumped)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime) alreadyJumped = false;

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if (t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rb.velocity += gravity * currentJumpM * Time.deltaTime;
        }
    }
    public void FallingSpeedIncrease()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity -= fallMultiplier * Time.deltaTime * gravity;
        }
    }

    public void JumpQueue()
    {
        inputBuffer.Enqueue("jump");
        Invoke("RemoveInput", inputBufferTimeSet);
    }

    public void AimUp()
    {
        arm.Rotate(0f, 0f, 90f, Space.Self);
    }

    public void AimUpRelease()
    {
        arm.Rotate(0, 0, -90f, Space.Self);
    }

    public void Attack(float input)
    {
        if (Weapon != null && input > 0 && (weaponReady == true || Weapon.IsFullAuto == true))
        {
            Weapon.Attack();
            weaponReady = false;

            if (Weapon.Ammo <= 0)
            {
                print("si");
                Weapon.DestroyWeapon();
                WeaponIsNull();
            }
        }

        if (Weapon != null && input < 1)
        {
            weaponReady = true;
        }

        if (Weapon == null && input > 0) 
        {
            fists.Attack();
            
        }
    }
    public void WeaponIsNull()
    {
        //gameObject.layer = 7;
        fists.OffRenderFists(true);
        Weapon = null;
      
       
    }
    public void DropWeapon()
    {
        if (Weapon != null)
        {
            fists.OffRenderFists(true);
            ////TODO: layer 7 es "player" 
            Weapon._Transform.SetParent(null);
            Weapon._Transform.position = dropPosition.transform.position;
            Weapon._Collider2D.enabled = true;
            Weapon.Rigidbody2D.isKinematic = false;
            Weapon.Rigidbody2D.simulated = true;
            Weapon._SpriteRenderer.sortingLayerName = "Weapon";
            WeaponIsNull();
            
        }
    }
 
    public void Timer()
    {
        if (floorRaycast == true)
        {
            return;
        }
        else
        {
            coyoteTime += Time.deltaTime;
        }
    }


    private void RemoveInput()
    {
        if (inputBuffer.Count > 0)
        {
            inputBuffer.Dequeue();
        }
        else
        {
            return;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == 6)
        {
           
            alreadyJumped = false;
            coyoteTime = 0;
        }


    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        //if (hand.childCount >= 1)//TODO: CAMBIAR MAS ADELANTE
        //{
        //    gameObject.layer = default; 
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colisione contra algo");
        if (Weapon == null && collision.gameObject.layer == 8)
        {
            Debug.Log("Colisione con el arma");
            Weapon = collision.GetComponent<IWeapon>();
            collision.GetComponent<Collider2D>().enabled = false;
            var rigid = collision.GetComponent<Rigidbody2D>();
            rigid.isKinematic = true;
            rigid.simulated = false;
            rigid.velocity = Vector2.zero;
            collision.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
            weaponPrefab = collision.GetComponent<Transform>();
            GrabWeapon();
        }



        //if(collision.gameObject.layer == 6)
        //{
        //    Instantiate(dust, transform.position, dust.transform.rotation);
        //}
    }


    private void GrabWeapon()
    {
        Debug.Log("Agarre el arma");
        fists.OffRenderFists(false);
        spriteRenderer.sortingOrder = 1;
        weaponPrefab.position = hand.position;
        weaponPrefab.rotation = hand.rotation;
        weaponPrefab.SetParent(hand);
        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(floorOffset.position, Vector2.down * raycastFloorDistance);
        Gizmos.DrawRay(hitOffsetRight.position, Vector2.down * raycastHorizontalDistance);
        Gizmos.DrawRay(hitOffsetLeft.position, Vector2.down * raycastHorizontalDistance);
    }

   
}
