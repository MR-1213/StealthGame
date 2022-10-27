using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 入力の管理を行う */
public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;

    public Vector2 movementInput; //X軸:横方向(AD) Y軸:縦方向(WS)
    public Vector2 cameraInput; //X軸:横方向(AD) Y軸:縦方向(WS)

    public float moveAmount;　//プレイヤーの移動量

    public float cameraInputX;
    public float cameraInputY;
    public float verticalInput; //movementInputのY成分取得のための変数
    public float horizontalInput;　//movementInputのX成分取得のための変数

    public bool shift_Input;
    public bool ctr_Input;
    public bool jump_Input;

    private void Start()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Shift.performed += i => shift_Input = true;
            playerControls.PlayerActions.Shift.canceled += i => shift_Input = false;

            playerControls.PlayerActions.Ctr.performed += i => ctr_Input = true;
            playerControls.PlayerActions.Ctr.canceled += i => ctr_Input = false;

            playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
        }

        playerControls.Enable(); //InputActionの動作をする
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleWalkingInput();
        HandleJumpingInput();
        //HandleActionInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y; //WSボタンのデータ取得
        horizontalInput = movementInput.x; //ADボタンのデータ取得

        cameraInputY = cameraInput.y; //縦方向のマウスの移動量を取得(上下の視点移動)
        cameraInputX = cameraInput.x; //横方向のマウスの移動量を取得(視点の回転)

        /* Clamp01で引数内のX成分+Y成分の絶対値(Math.Abs)を0~1に正規化 */
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting, playerLocomotion.isWalking); //移動量の決定
    }

    private void HandleSprintingInput()
    {
        if(shift_Input && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleWalkingInput()
    {
        if(ctr_Input && moveAmount > 0.01f)
        {
            playerLocomotion.isWalking = true;
        }
        else
        {
            playerLocomotion.isWalking = false;
        }
    }

    private void HandleJumpingInput()
    {
        if(jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }
    }
}
