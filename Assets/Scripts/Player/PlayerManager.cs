using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;
    StageObjectManager stageObjectManager;

    public bool isInteracting; //落下と着地のモーションのためのフラグ

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
        stageObjectManager = FindObjectOfType<StageObjectManager>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        stageObjectManager.GetClosestObject();
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