using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lookSpeed = 20f;
    public float jumpForce = 15f;
    public float gravity = 9.81f;
    public Shooter playerShooter;
    public Health playerHealth;
    public List<GameObject> disableWhileDead;

    private CharacterController controller;
    private InputManager inputManager;

    Vector3 moveDirection;

    [Header("Jump Settings")]
    public float jumpTimeLeniency = 0.1f;
    float timeStopBeingLenient = 0;
    bool doubleJumpAvailable = false;

    

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetupCharacterController();
        SetupInputManager();
    }

    void SetupCharacterController(){
        controller = GetComponent <CharacterController>();
        if (controller == null){
            Debug.LogError("The player needs to have the character controller!");
        }
    }

    void SetupInputManager(){
        inputManager = InputManager.instance;
    }
    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Update()
    {   
        if (playerHealth.currentHealth <= 0)
        {
            foreach (GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(false);
            }
        }
        else
        {
            ProcessMovement();
            ProcessRotation();
        }
        
    }

    void ProcessMovement(){
        float leftRightInput = inputManager.horizontalMoveAxis;
        Debug.Log("Left right input: " + leftRightInput);
        float forwardBackInput = inputManager.verticalMoveAxis;
        Debug.Log("Forward back input: " + forwardBackInput);

        bool jumpPressed = inputManager.jumpPressed;
        Debug.Log("Jump pressed: " + jumpPressed);

        if(controller.isGrounded){
            doubleJumpAvailable = true;
            timeStopBeingLenient = Time.time + jumpTimeLeniency;
            moveDirection = new Vector3(leftRightInput * moveSpeed, 0, forwardBackInput * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);

            if(jumpPressed){
                moveDirection.y = jumpForce;
            }
        } else{

            moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y, forwardBackInput * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);

            if(jumpPressed && Time.time < timeStopBeingLenient){
                moveDirection.y = jumpForce;
            }
            else if(jumpPressed && doubleJumpAvailable)
            {
                moveDirection.y = jumpForce;
                doubleJumpAvailable = false;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        if (controller.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -0.3f;   
        }
        controller.Move(moveDirection * Time.deltaTime);
    }

    void ProcessRotation(){
        float horizontalLookInput = inputManager.horizontalLookAxis;
        float verticalLookInput = inputManager.verticalLookAxis;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(playerRotation.x, playerRotation.y + horizontalLookInput * lookSpeed * Time.deltaTime, playerRotation.z));
    }
}
