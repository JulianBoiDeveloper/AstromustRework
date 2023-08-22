using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class ThirdPersonControllerV2 : MonoBehaviour
{
    [Header("Movement")]
    public float MovementVelocity = 3.8f;
    public AnimationCurve AccelerationCurve;
    public AnimationCurve DecelerationCurve;
    public float JumpPower = 2.0f;
    public float Gravity = 9.86f;
    public float MovementTurnSpeed = 4.5f;

    [HideInInspector]
    public bool canMove = true;

    public Vector2 currentInput {get; private set;} = new Vector2();
    [SerializeField] private Camera mainCamera;

    public float currentSpeed {get; private set;}
    public bool jumped {get; private set;} = false;
    public bool isGrounded {get; private set;} = true;

    [SerializeField] Cinemachine.CinemachineFreeLook cinemachine;

    [SerializeField] GameObject mobileUI;
    [SerializeField] bool isMobileMovement = true;

    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;
    private Vector3 velocity;
   
    

    [SerializeField] public TestCharacterController testCharController;
    [SerializeField] Animator animator;

    public PhotonView view;
    [SerializeField] Camera myCam;
    [SerializeField] GameObject personalUI;

    [SerializeField] GameObject powerUP;
    [SerializeField] GameObject powerUP2;

    public GameObject mngJoystick;
    [SerializeField] AudioSource source;
    
    public AudioSource soundStep;

    WaveSpawner waveSpawner;
    [SerializeField] GameObject startGameWaveButton;
    [SerializeField] Transform groundCheckPosition;
    public Vector3 defaultPos;

    // Declare the event delegate and event
    public delegate void InteractDungeonEvent(int viewID);
    public static event InteractDungeonEvent OnInteractDungeon;

    public void AddScore(int toAdd)
    {
        PhotonNetwork.LocalPlayer.AddScore(toAdd);

        if(TheGameManager.Instance != null && TheGameManager.Instance.playerName != string.Empty) {
            PhotonNetwork.LocalPlayer.NickName = TheGameManager.Instance.playerName;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }

    void Awake() {
        if (mainCamera == null) {
            Debug.LogError("Main camera missing in scene!");
        }

        view = GetComponent<PhotonView>();
        waveSpawner = GameObject.FindAnyObjectByType<WaveSpawner>();
        Debug.Log("Camera id: " + myCam.gameObject.GetInstanceID());


        if (!view.IsMine) {
            myCam.gameObject.SetActive(false);
            personalUI.SetActive(false);
        }
        else myCam.transform.parent = null;

        Debug.Log("Camera id: " + myCam.gameObject.GetInstanceID());
    }

    private void Start()
    {
#if UNITY_STANDALONE_WIN
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
#endif
     
        if(view.IsMine)
            AddScore(5000);

        soundStep = soundStep.GetComponent<AudioSource>();
    }

    public float rotationSpeed = 35f; // Speed at which the character rotates

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
        //    DebugConsole.Instance.Log("Debug message 1");
        }

   //     Debug.Log("CURSOR: " + UnityEngine.Cursor.lockState);
        if (!view.IsMine) return;

        if (testCharController.HasGun())
        {
            // Get the direction from the character to the camera
            Vector3 cameraDirection = mainCamera.transform.position - transform.position;

            // Ignore the vertical (y) component of the camera direction
            cameraDirection.y = 0f;

            // Rotate the character to face the camera direction
            if (cameraDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-cameraDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (transform.position.y < -10f) {
            transform.position = defaultPos;
            transform.rotation = Quaternion.identity;
        }

        UpdateMovement();

        if(waveSpawner != null)
        {
            if(waveSpawner.gameStarted)
            {
                startGameWaveButton.SetActive(false);
            }
            else if(!PhotonNetwork.IsMasterClient)
            {
                startGameWaveButton.SetActive(false);
            }
            else
            {
                startGameWaveButton.SetActive(true);
            }
        }
    //    PhotonNetwork.GetCustomRoomList();
    }

    #region Movement Code
    Vector3 lastMoveDirection;
    Vector3 lastInputDirection;
    float curveT = 0.0f;
    float verticalSpeed = 0.0f;

    public void BlockMovement()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        canMove = false;
        //cinemachine.enabled = false;
        mobileUI.SetActive(false);
    }

    public void UnblockMovement()
    {
#if UNITY_STANDALONE_WIN
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
#endif
        UnityEngine.Cursor.visible = false;
        canMove = true;
    //    cinemachine.enabled = true;
        mobileUI.SetActive(true);
    }

    public void StartGameWaves()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // LOCKS THE ROOM
            bool isRoomOpen = !PhotonNetwork.CurrentRoom.IsOpen;
            PhotonNetwork.CurrentRoom.IsOpen = isRoomOpen;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            Debug.Log("Room is now " + (isRoomOpen ? "open" : "locked") + ".");
#if UNITY_STANDALONE_WIN
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
#endif
            GameObject.FindAnyObjectByType<WaveSpawner>().StartWave();
        }
    }

    int MapToSign(float value)
    {
        if (value > 0)
            return 1;
        else if (value < 0)
            return -1;
        else
            return 0;
    }

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float smoothRotationTime = 0.25f;
    float currentSpeed2;

    float currentVelocity;
    float speedVelocity;

    [SerializeField] bool global;

    //[SerializeField] FixedJoystick joystick;
    [SerializeField] bool enableMobileInputs;

    [SerializeField] Transform cameraTransform;


    void UpdateMovement() {
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.P))
        {
            BlockMovement();
        }
        else 
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BlockMovement();
        }
#endif
        Vector2 input = Vector2.zero;

        if (enableMobileInputs)
        {
           // input = new Vector2(joystick.input.x, joystick.input.y);
        }
        else
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)
        {
            if(global) {
                if(!testCharController.HasGun()) {
                    float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref currentVelocity, smoothRotationTime);
                }
            }
        }

        float targetSpeed = moveSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, 0.1f);


        if (global)
        {
            if(!testCharController.HasGun()) {
                transform.Translate(transform.forward * (currentSpeed * inputDir.magnitude) * Time.deltaTime, Space.World);
            }
            else
            {
                Vector3 movementDir = transform.forward * inputDir.y + transform.right * inputDir.x;

                transform.Translate(movementDir * currentSpeed * Time.deltaTime, Space.World);
            }
        }
        else
        {
            transform.Translate(transform.forward * (currentSpeed * inputDir.magnitude) * Time.deltaTime, Space.Self);
        }

        if (inputDir != Vector2.zero)
        {
            if (isGrounded)
            {
                if (!soundStep.isPlaying)
                {   
                    soundStep.PlayOneShot(soundStep.clip);
                }
            }
        
        }

        animator.SetInteger("moveX", MapToSign(inputDir.x));
        animator.SetInteger("moveY", MapToSign(inputDir.y));

        if (!testCharController.HasGun()) {
            animator.SetBool("RiffleRun", false);
            animator.SetBool("RiffleIdle", false);
        }
        else
        {
            if (inputDir != Vector2.zero)
            {
                animator.SetBool("RiffleRun",true);
                animator.SetBool("RiffleIdle", false);
            }
            else
            {
                animator.SetBool("RiffleRun", false);
                animator.SetBool("RiffleIdle",true);
            }
        }

        if (inputDir.magnitude > 0.1f) {
            if(Vector3.Dot(inputDir, lastInputDirection) < 0.25f) {
                curveT = 0;
            }
            curveT += Time.deltaTime;
            currentSpeed = MovementVelocity * AccelerationCurve.Evaluate(curveT);
            lastInputDirection = inputDir;
            lastMoveDirection = inputDir * currentSpeed;
            // Debug.Log($"Accelerating: {AccelerationCurve.Evaluate(curveT)}, current speed: ${currentSpeed}");
        }
        else {
            if(Vector3.Dot(inputDir, lastInputDirection) < 0f) {
                curveT -= Time.deltaTime * 1.15f;
            }else {
                curveT -= Time.deltaTime;
            }
            curveT = Mathf.Clamp(curveT, 0, DecelerationCurve.keys[AccelerationCurve.length - 1].time);
            currentSpeed = MovementVelocity * DecelerationCurve.Evaluate(curveT);
            lastMoveDirection = lastMoveDirection.normalized * currentSpeed;
            // Debug.Log($"Deceleration: {DecelerationCurve.Evaluate(curveT)}, current speed: ${currentSpeed}");
        }
        
        if (isGrounded) 
        {
            verticalSpeed = 0;
     //       isGrounded = true;
            if(jumped) 
            {
                verticalSpeed = JumpPower;
                jumped = false;
            }
        }
        
        verticalSpeed -= Gravity * Time.deltaTime;

        // Check for jump input
        if ((Input.GetButtonDown("Jump") || jumpClicked) && isGrounded)
        {
            Jump();  
            jumped = true;
            jumpClicked = false;
        }
        else
        {
            jumpClicked = false;
        }

        IsGrounded();
        
        // Apply gravitys
        ApplyGravity();
    }
    #endregion

    void IsGrounded()
    {
        isGrounded =  Physics.CheckSphere(groundCheckPosition.position, groundDistance, groundLayer);
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void Jump()
    {
        // Apply upward force to simulate the jump
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
    //    isGrounded = false;
        GetComponent<Rigidbody>().velocity = new Vector3(0f, velocity.y, 0f);
    }

    public void SetPowerUP(int type)
    {
        view.RPC("ActivatePowerUp", RpcTarget.AllBuffered, type);
    }

    public void Use()
    {
        OnInteractDungeon?.Invoke(view.ViewID);

        testCharController.TryInteract();
    }
    
    [PunRPC]
    public void SetGunHandle(int gunID)
    {
        for (int i = 0; i <  testCharController.gunsInHandle.Count; i++)
        {
            if (testCharController.gunsInHandle[i].ID == gunID)
            {
                testCharController.gunsInHandle[i].gameObject.SetActive(true);
            }
        }
    }

    [PunRPC]
    public void PlayShotSound(int gunID)
    {
        source.clip = testCharController.gunsInHandle[gunID].shotSound;
        source.Play();
    }

    public void PlayShot(int gunID)
    {
        view.RPC("PlayShotSound", RpcTarget.All, gunID);
    }

    public void AttachGun(int gunID)
    {
        view.RPC("SetGunHandle", RpcTarget.AllBuffered, gunID);
    }

    public void SetInteractingPlayer(RandomGunBox box)
    {
        box.SetInteractingPlayerPUN(view.ViewID);
    }

    [PunRPC]
    public void PlayGunVFX(int i)
    {
        testCharController.gunsInHandle[i].transform.GetChild(0).GetComponent<ParticleSystem>().Play();
    }

    public void GunVFX(int i)
    {
        view.RPC("PlayGunVFX", RpcTarget.AllBuffered, i);
    }

    [PunRPC]
    public void ActivatePowerUp(int type)
    {
        if(type == 0) {
            powerUP.SetActive(true);
            testCharController.extraDamage = 2;
        }
        else if(type == 1)
        {
            powerUP2.SetActive(true);
            GetComponent<HungerSystem>().maxHealth *= 2f;
        }
    }

    bool jumpClicked = false;
    public void JumpPayer()
    {
        jumpClicked = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!view.IsMine) return;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!view.IsMine) return;

    }

        #region InputSystemMessages
        public void OnMove(InputValue value) {

        if (!view.IsMine) return;

        if (canMove) {
            currentInput = value.Get<Vector2>();
        }
        else
        {
            currentInput = Vector2.zero;
        }
    }
    #endregion
}
