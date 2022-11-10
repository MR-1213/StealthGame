using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;

    public Slider slider;

    public bool isInteracting; //落下と着地のモーションのためのフラグ
    private float tmpHP;

    [Header("Player Status")]
    public int hp = 20;

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
        Cursor.lockState = CursorLockMode.Locked; //カーソルを画面中央に固定 => 画面外に出ないようにする
        slider.value = hp;
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        tmpHP += Time.deltaTime / 5.0f;
        hp = Mathf.FloorToInt(hp);
        slider.value = hp;    
    }

    /* 一定時間(0.02秒)ごとに呼び出されるUpdate関数 Edit,ProjectSetting,Time,FixedTimestepより設定可能 */ 
    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    /* Update関数群で一番最後に呼び出されるUpdate関数 */
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded",playerLocomotion.isGrounded);
    }
}