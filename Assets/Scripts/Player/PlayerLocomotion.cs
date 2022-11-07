using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* プレイヤーの移動に関わることをを管理する */
public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    InputManager inputManager;
    AnimatorManager animatorManager;

    Vector3 moveDirection; //プレイヤーの移動方向および移動量
    Transform cameraObject;
    Rigidbody playerRigidbody;

    [Header("Falling Factors")]
    public float inAirTimer; //空中滞在時間 
    public float fallingVelocity; //落下速度
    public float rayCastHeightOffSet; //地面検知のためのレイの高さ
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isWalking;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Speeds")]
    public float walkingSpeed;
    public float runningSpeed;
    public float sprintingSpeed;
    public float rotationSpeed;

    [Header("Jump Speeds")]
    public float jumpHeight = 3.0f;
    public float gravityIntensity = -15.0f;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        animatorManager = GetComponent<AnimatorManager>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallAndLanding();
        HandleMovement();
        if(playerManager.isInteracting)
        {
            return;　//落下と着地中はこれより下の関数群は呼び出されない        
        }
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput; //Z軸方向の動きを入力(前方が正)
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput; //X軸方向の動きを入力(右が正)
        moveDirection.Normalize(); //方向を維持したまま長さを1にする 
        moveDirection.y = 0; //Y軸方向は0にする

        if(isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed; //全力ダッシュ
        }
        else
        {
            if(inputManager.moveAmount >= 0.5f && !(isWalking))
            {
                moveDirection = moveDirection * runningSpeed; //ダッシュ
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed; //ウォーク
            }
        }
        
        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = new Vector3(movementVelocity.x, playerRigidbody.velocity.y, movementVelocity.z);
    }

    private void HandleRotation()
    {
        if(isJumping)
        {
            return;
        }
        Vector3 targetDirection = Vector3.zero;  //Vector3(0, 0, 0)

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        /* キーを離した際に方向が初期値(0,0,0)へ戻ってしまうことの修正 */
        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward; //向いている方向(正面)を取得
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotatiton = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); //体が回るように方向転換するようにする

        transform.rotation = playerRotatiton;
    }

    public void HandleFallAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition; //プレイヤーの足の疑似的な当たり判定場所
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        targetPosition = transform.position; //初期値はプレイヤーの原点(足元中心)

        if(!(isGrounded))
        {
            if(!(playerManager.isInteracting) && (inAirTimer > 0.5f))
            {
                animatorManager.PlayTargetAnimation("Fall", true); //落下アニメーションに移行
            }
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(Vector3.down * fallingVelocity * inAirTimer); //Vector3.down==(0,-1,0) 落下速度が時間の経過とともに大きくなる
        }

        if(Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, 0.7f, groundLayer)) //真下へレイを飛ばして地面を検知したとき
        {
            if(!(isGrounded) && !(playerManager.isInteracting)) //落下中であるかどうかの判定(だた地面に立っているだけではないかどうか)
            {
                animatorManager.PlayTargetAnimation("Landing", true); //着地アニメーションに移行
            }
            Vector3 rayCastHitPoint = hit.point; //地面にレイを飛ばして当たった場所の座標を取得
            targetPosition.y = rayCastHitPoint.y; 
            inAirTimer = 0; //落下中ではなくなるため0に
            isGrounded = true;　//地面に着地しているためtrueに
        }
        else
        {
            isGrounded = false; //まだ落下中の場合はfalseに
        }

        if(isGrounded && !(isJumping))
        {
            /* transform.positionを常に更新することで地面に立つことを可能にしている */
            if(playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f); //第一引数と第二引数の間で線形補間
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if(isGrounded)
        {
            animatorManager.animator.SetBool("isJumping",true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }
}
