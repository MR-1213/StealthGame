using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    PlayerLocomotion playerLocomotion;

    int horizontal;
    int vertical;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        horizontal = Animator.StringToHash("Horizontal");//引数内の文字列をIDへ変換
        vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting, bool isWalking)
    {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if((horizontalMovement > 0 && horizontalMovement < 0.55f))
        {
            snappedHorizontal = 0.5f;
        }
        else if(horizontalMovement >= 0.55f)
        {
            snappedHorizontal = 1.0f;
        }
        else if((horizontalMovement < 0 && horizontalMovement > -0.55f))
        {
            snappedHorizontal = -0.5f;
        }
        else if(horizontalMovement <= -0.55f)
        {
            snappedHorizontal = -1.0f;
        }
        else
        {
            snappedHorizontal = 0;
        }
        #endregion

         #region Snapped Vertical
        if((verticalMovement > 0.01f && verticalMovement < 0.55f))
        {
            snappedVertical = 0.5f;
        }
        else if(verticalMovement >= 0.55f)
        {
            snappedVertical = 1.0f;
        }
        else if((verticalMovement < 0 && verticalMovement > -0.55f))
        {
            snappedVertical = -0.5f;
        }
        else if(verticalMovement <= -0.55f)
        {
            snappedVertical = -1.0f;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion

        if(isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2.0f;
        }

        if(isWalking)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 0.5f;
        }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
        //https://answers.unity.com/questions/611667/damptime-and-deltatime-in-setfloat-parameters.html
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting); 
        animator.CrossFade(targetAnimation, 0.2f); //一つ前のアニメーションとブレンドしてtargetAnimationのアニメーションを実行
    }
}
