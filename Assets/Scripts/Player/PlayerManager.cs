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

    public bool isInteracting; //落下と着地のモーションのためのフラグ
    public bool disableSamplePosition;

    [Header("Basic Status")]
    public Slider hpSlider; //プレイヤーのHPを表すスライダー
    private int currentHP; //現在のHP
    private int maxHP = 20; //最大HP
    private int damage = 1; //受けるダメージ量

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
        Cursor.lockState = CursorLockMode.Locked; //カーソルを画面中央に固定 => 画面外に出ないようにする
        hpSlider.maxValue = maxHP;
        hpSlider.value = maxHP;
        currentHP = maxHP;
    }

    private void Update()
    {
        inputManager.HandleAllInputs();   
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

    /// <summary>
    /// PlayerのHP(Slider)を減らす。Enemyの攻撃に応じてEnemySearchPlayer.csにより1秒間隔で呼ばれる。
    /// </summary>
    public void DecreaseHP()
    {
        currentHP -= damage;
        hpSlider.value = currentHP;
        if(hpSlider.value == 0) hpSlider.value = 0;
    }

    private void OnCollisionStay(Collision other) 
    {
        if(other.gameObject.CompareTag("Shortcut"))
        {
            disableSamplePosition = true;
        }
        else
        {
            disableSamplePosition = false;
        }
    }
}