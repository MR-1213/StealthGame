using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    
    public Transform point_1;
    public Transform point_2;
    public Transform point_3;
    public Transform point_4;
    public Transform pointPlayer;

    NavMeshAgent navMeshAgent;
    Animator animator;

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
    }

    State currentState = State.MoveToPoint_1;
    bool stateEnter = false;
    float stateTime = 0;
    bool isMissingPlayer;

    void Start()
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

    public void ChangeMoveToPlayerState()
    {
        currentState = State.MoveToPlayer;
        stateEnter = true;
        isMissingPlayer = false;
        stateTime = 0;
        Debug.Log(currentState.ToString());
    }

    public void MissingPlayer()
    {
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
                
                return;
            }

            case State.stopPoint1: {
                
                if(stateTime >= 5.0f)
                {
                    ChangeState(State.MoveToPoint_2);
                    return; 
                }

                return;
            }

            case State.stopPoint2: {
                
                if(stateTime >= 5.0f)
                {
                    ChangeState(State.MoveToPoint_3);
                    return;
                }

                return;
            }

            case State.stopPoint3: {
                
                if(stateTime >= 3.0f)
                {
                    ChangeState(State.MoveToPoint_4);
                    return;
                }

                return;
            }

            case State.stopPoint4: {
                
                if(stateTime >= 4.0f)
                {
                    ChangeState(State.MoveToPoint_1);
                    return;
                }

                return;
            }

            case State.MoveToPlayer: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(pointPlayer.position);
                    navMeshAgent.speed = 3.0f;
                }

                if(isMissingPlayer)
                {
                    ChangeState(State.MoveToPoint_1);
                    return;
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
