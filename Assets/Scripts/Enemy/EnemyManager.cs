using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    
    public Transform point_1;
    public Transform point_2;
    public Transform point_3;
    public Transform point_4;
    public Transform pointPlayer;

    NavMeshAgent navMeshAgent;
    Animator animator;

    public Image flushImage;

    enum State
    {
        MoveToPoint_1,
        MoveToPoint_2,
        MoveToPoint_3,
        MoveToPoint_4,
        MoveToPlayer,
        stopPoint1,
        stopPoint2,
        stopPoint3,
        stopPoint4,
        attackPlayer,
    }

    State currentState = State.MoveToPoint_1;
    State lastState = State.MoveToPoint_1;
    bool stateEnter = false;
    float stateTime = 0;
    bool isMissingPlayer;

    enum Animation_State
    {
        Patrol = 0,
        Shoot = 1,
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        ChangeState(State.MoveToPoint_1);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        stateEnter = true;
        stateTime = 0;
        Debug.Log(currentState.ToString());
    }

    private void ChangeAnimationState(Animation_State state)
    {
        animator.SetInteger("ID", (int)state);
    }

    public void ChangeMoveToPlayerState()
    {
        if(currentState != State.MoveToPlayer && currentState != State.attackPlayer)
        {
            lastState = currentState;
        }
        currentState = State.MoveToPlayer;
        stateEnter = true;
        isMissingPlayer = false;
        stateTime = 0;
        Debug.Log(currentState.ToString());
    }

    public void ChangeAttackPlayerState()
    {
        if(currentState != State.MoveToPlayer && currentState != State.attackPlayer)
        {
            lastState = currentState;
        }
        currentState = State.attackPlayer;
        stateEnter = true;
        isMissingPlayer = false;
        stateTime = 0;
    }

    public IEnumerator MissingPlayer()
    {
        yield return new WaitForSeconds(3.0f);
        isMissingPlayer = true;
    }
    private void Update()
    {
        stateTime += Time.deltaTime;
        float speed = navMeshAgent.velocity.magnitude;

        animator.SetFloat("EnemySpeed", speed);

        switch (currentState)
        {
            case State.MoveToPoint_1: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(point_1.position);
                    navMeshAgent.speed = 1.7f;
                }

                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending)
                {
                    ChangeState(State.stopPoint1);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }
            
            case State.MoveToPoint_2: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(point_2.position);
                    navMeshAgent.speed = 2.5f;
                }

                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending)
                {
                    ChangeState(State.stopPoint2);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.MoveToPoint_3: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(point_3.position);
                    navMeshAgent.speed = 2.5f;
                }

                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending)
                {
                    ChangeState(State.stopPoint3);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.MoveToPoint_4: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(point_4.position);
                    navMeshAgent.speed = 2.5f;
                }

                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending)
                {
                    ChangeState(State.stopPoint4);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.stopPoint1: {
                
                if(stateTime >= 5.0f)
                {
                    ChangeState(State.MoveToPoint_2);
                    return; 
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.stopPoint2: {
                
                if(stateTime >= 5.0f)
                {
                    ChangeState(State.MoveToPoint_3);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.stopPoint3: {
                
                if(stateTime >= 3.0f)
                {
                    ChangeState(State.MoveToPoint_4);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.stopPoint4: {
                
                if(stateTime >= 4.0f)
                {
                    ChangeState(State.MoveToPoint_1);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.MoveToPlayer: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(pointPlayer.position);
                    navMeshAgent.speed = 3.5f;
                }

                if(isMissingPlayer)
                {
                    ChangeState(lastState);
                    return;
                }
                ChangeAnimationState(Animation_State.Patrol);
                return;
            }

            case State.attackPlayer: {

                if(stateEnter)
                {
                    navMeshAgent.speed = 0;
                    ChangeAnimationState(Animation_State.Shoot);
                    
                    Debug.Log("攻撃！");
                }
                return;
            }
        }
    }

    private void LateUpdate() 
    {
        if(stateTime != 0)
        {
            stateEnter = false;
        }    
    }
}
