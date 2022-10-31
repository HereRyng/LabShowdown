using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("ENABLE ONLY TO TEST")]
    [SerializeField] private bool canTest;
    private SpriteRenderer skin;

    private PlayerConfiguration playerConfig;

    private PlayerModel model;

    private InputActionAsset inputAsset;

    private InputActionMap player;

    private InputAction movement;

   private PlayerInput playerInput;


    public PlayerConfiguration PlayerConfig { get => playerConfig; }

    private InputAction attack;

    private void Awake()
    {
        skin = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        print(skin);
        if(canTest) Test();
    }

    public void InitializePlayer(PlayerConfiguration pc)
    {
        playerConfig = pc;
        skin.sprite = pc.PlayerSkin;
        playerInput = playerConfig.Input;
        inputAsset =playerConfig.Input.actions;
        player = inputAsset.FindActionMap("Player");
        model = GetComponent<PlayerModel>();
        attack = player.FindAction("Attack");
        movement = player.FindAction("Movement");
        player.FindAction("Drop").performed += DropInput;
        player.FindAction("Jump").performed += JumpInput;
        player.FindAction("Jump").canceled += JumpInput;
        player.FindAction("AimUp").performed += AimUpInput;
        player.FindAction("AimUpRelease").performed += AimUpReleaseInput;
        player.Enable();

    }

  

    void Update()
    {
        model.Movement(movement.ReadValue<Vector2>().x);
        model.Attack(attack.ReadValue<float>());
        model.Jump(movement.ReadValue<Vector2>().x);
        model.LimitHeight();
        model.Timer();
        model.Raycasts();
    }

    private void OnDisable()
    {
        player.FindAction("Drop").performed -= DropInput;
        player.FindAction("Jump").performed -= JumpInput;
        player.FindAction("Jump").canceled -= JumpInput;
        player.FindAction("AimUp").performed -= AimUpInput;
        player.FindAction("AimUpRelease").performed -= AimUpReleaseInput;
        player.Disable();
    }
 

    public void Test()
    {
        playerConfig = new PlayerConfiguration(playerInput);
        model = GetComponent<PlayerModel>();
        playerInput = playerConfig.Input;
        inputAsset = playerConfig.Input.actions;
        player = inputAsset.FindActionMap("Player");
        model = GetComponent<PlayerModel>();
        attack = player.FindAction("Attack");
        movement = player.FindAction("Movement");
        player.FindAction("Drop").performed += DropInput;
        player.FindAction("Jump").performed += JumpInput;
        player.FindAction("Jump").canceled += JumpInput;
        player.FindAction("AimUp").performed += AimUpInput;
        player.FindAction("AimUpRelease").performed += AimUpReleaseInput;
        player.Enable();
    }
    private void DropInput(InputAction.CallbackContext context)
    {
        model.DropWeapon();
    }
    private void JumpInput(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
          
            model.CancelledJump();
        }
        else
        {
            model.JumpQueue();
        }
    }

    private void AimUpInput(InputAction.CallbackContext context)
    {
        model.AimUp();
    }

    private void AimUpReleaseInput(InputAction.CallbackContext context)
    {
        model.AimUpRelease();
    }


}
